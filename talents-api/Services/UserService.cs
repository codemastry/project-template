using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.DataAccess;
using TalentsApi.Helpers;
using TalentsApi.Helpers.Auth;
using TalentsApi.Helpers.Security;
using TalentsApi.Models;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Auth;
using TalentsApi.Models.Entities;
using TalentsApi.Models.User;

namespace TalentsApi.Services
{
    public class UserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly AppSettingsModel _appSettingsModel;
        private readonly TalentsDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;
        private readonly EmailService _emailService;
        private readonly FilesService _filesService;

        public UserService(AppSettingsModel appSettingsModel,
            TalentsDbContext context,
            ILogger<UserService> logger,
            EmailService emailService,
            IHttpContextAccessor httpContext,
            IMapper mapper,
            FilesService filesService)
        {
            _context = context;
            _appSettingsModel = appSettingsModel;
            _logger = logger;
            _httpContext = httpContext;
            _userClaims = (UserClaims)_httpContext.HttpContext.Items["UserClaims"];
            _mapper = mapper;
            _emailService = emailService;
            _filesService = filesService;
        }

        public async Task<List<UserModel>> All(string search)
        {
            var result = new List<UserModel>();

            try
            {
                var users = new List<User>();
                if (!string.IsNullOrWhiteSpace(search))
                {
                    search = search.ToUpper();
                    users = await _context.Users.Where(m =>
                                m.CompanyId == _userClaims.CompanyId &&
                                (
                                    m.FirstName.ToUpper().Contains(search) ||
                                    m.LastName.ToUpper().Contains(search) ||
                                    m.Email.ToUpper().Contains(search)
                                )
                            ).ToListAsync();
                }
                else
                {
                    users = await _context.Users.Where(m => m.CompanyId == _userClaims.CompanyId).ToListAsync();
                }
                result = _mapper.Map<List<UserModel>>(users);

                // attach role names to user
                var companyRoles = await _context.UserRoles.Where(m => m.CompanyId == _userClaims.CompanyId).ToListAsync();
                var userRoleIdsByCompany = await _context.UserRoleIds.Where(m => m.CompanyId == _userClaims.CompanyId).ToListAsync();

                foreach (var user in result)
                {
                    var userRoleIds = userRoleIdsByCompany.Where(m => m.UserId == user.Id).Select(m => m.RoleId).ToList();
                    var userRoles = companyRoles.Where(m => userRoleIds.Any(a => a == m.Id)).ToList();
                    user.Roles = userRoles.Select(m => m.Name).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new List<UserModel>();
            }

            return result;
        }

        public async Task<ResultModel<CreateUpdateUserModel>> ById(int id)
        {
            var result = new ResultModel<CreateUpdateUserModel>();
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == _userClaims.CompanyId);
                if (user == null)
                    return new ResultModel<CreateUpdateUserModel> { Message = "User not found" };

                var userRoleIds = _context.UserRoleIds.Where(m => m.UserId == user.Id).Select(b => b.RoleId).ToList();
                var companyRoles = await _context.UserRoles.Where(m => m.CompanyId == _userClaims.CompanyId).OrderBy(m => m.Name).ToListAsync();
                var roles = companyRoles.Where(m => userRoleIds.Any(a => a == m.Id)).ToList();

                user.Password = string.Empty;
                var data = _mapper.Map<CreateUpdateUserModel>(user);
                foreach (var role in companyRoles)
                {
                    data.Roles.Add(new CreateUpdateUserRoleItem
                    {
                        Name = role.Name,
                        IsChecked = roles.FirstOrDefault(m => m.Id == role.Id) != null,
                        RoleId = role.Id
                    });
                }
                return new ResultModel<CreateUpdateUserModel> { Data = data, IsSuccess = true };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<CreateUpdateUserModel>();
            }
        }

        public async Task<ResultModel<string>> Create(CreateUpdateUserModel model)
        {
            try
            {
                // if user is not setting random password, validate if password field has value
                if (string.IsNullOrWhiteSpace(model.Password) && !model.SetRandomPassword)
                    return new ResultModel<string> { Message = "Invalid password" };

                // validate existing email
                var sameEmail = await _context.Users.FirstOrDefaultAsync(m => m.Email == model.Email);
                if (sameEmail != null)
                    return new ResultModel<string> { Message = $"Email address {model.Email} already registered." };
                
                // validate uploaded user picture
                if (model.UploadedPicture != null)
                {
                    var pictureValidation = ValidatePicture(model.UploadedPicture);
                    if (!pictureValidation.IsSuccess) return pictureValidation;
                }

                // map the model to user entity and set the companyid
                var user = _mapper.Map<User>(model);
                user.CompanyId = _userClaims.CompanyId;
                user.RegistrationDate = DateTime.UtcNow;

                if (model.SetRandomPassword)
                    model.Password = Utilities.RandomString(10);

                if (model.SendActivationEmail)
                {
                    var verificationToken = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{model.Email}");
                    user.EmailVerificationToken = verificationToken;
                    user.EmailVerificationExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours);
                }
                else
                    user.IsEmailVerified = true;

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var encryptedPassword = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, model.Password);
                    user.Password = encryptedPassword;

                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    // save profile picture
                    if (model.UploadedPicture != null)
                    {
                        var savePictureResult = await _filesService.SaveToDisk(model.UploadedPicture, user.Id);
                        if (savePictureResult.IsSuccess)
                            user.Picture = savePictureResult.Data;
                    }

                    var userRoleIds = model.Roles.Select(m => new UserRoleIds
                    {
                        CompanyId = _userClaims.CompanyId,
                        RoleId = m.RoleId,
                        UserId = user.Id
                    });

                    await _context.UserRoleIds.AddRangeAsync(userRoleIds);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    if (model.SendActivationEmail)
                        await SendActivationEmail(user, model);

                    return new ResultModel<string> { IsSuccess = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string>();
            }
        }

        public async Task<ResultModel<string>> Update(CreateUpdateUserModel model)
        {
            try
            {
                var userFromDb = await _context.Users.FirstOrDefaultAsync(m => m.Id == model.Id && m.CompanyId == _userClaims.CompanyId);
                if (userFromDb == null)
                    return new ResultModel<string> { Message = "Invalid user" };

                // validate uploaded user picture
                if (model.UploadedPicture != null)
                {
                    var pictureValidation = ValidatePicture(model.UploadedPicture);
                    if (!pictureValidation.IsSuccess) return pictureValidation;
                }

                // map the model to user entity and set the companyid
                userFromDb.Email = model.Email;
                userFromDb.FirstName = model.FirstName;
                userFromDb.LastName = model.LastName;
                userFromDb.IsLocked = model.IsLocked;
                userFromDb.ShouldSetPasswordOnNextLogin = model.ShouldSetPasswordOnNextLogin;
                userFromDb.IsActive = model.IsActive;
                userFromDb.EmailVerificationRequired = model.EmailVerificationRequired;

                if (model.SetRandomPassword)
                {
                    model.Password = Utilities.RandomString(10);
                    var encryptedPassword = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, model.Password);
                    userFromDb.Password = encryptedPassword;
                }
                else if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    var encryptedPassword = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, model.Password);
                    userFromDb.Password = encryptedPassword;
                }

                if (model.SendActivationEmail)
                {
                    var verificationToken = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{model.Email}");
                    userFromDb.EmailVerificationToken = verificationToken;
                    userFromDb.EmailVerificationExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours);
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    // save profile picture
                    if (model.UploadedPicture != null)
                    {
                        var savePictureResult = await _filesService.SaveToDisk(model.UploadedPicture, userFromDb.Id);
                        if (savePictureResult.IsSuccess)
                            userFromDb.Picture = savePictureResult.Data;
                    }

                    var userRoleIdsFromDb = await _context.UserRoleIds.Where(m => m.UserId == userFromDb.Id).ToListAsync();
                    _context.UserRoleIds.RemoveRange(userRoleIdsFromDb);
                    var userRoleIds = model.Roles.Select(m => new UserRoleIds
                    {
                        CompanyId = userFromDb.CompanyId,
                        RoleId = m.RoleId,
                        UserId = userFromDb.Id
                    });

                    await _context.UserRoleIds.AddRangeAsync(userRoleIds);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    if (model.SendActivationEmail)
                        await SendActivationEmail(userFromDb, model);

                    return new ResultModel<string> { IsSuccess = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string>();
            }
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == _userClaims.CompanyId);
                if (user == null)
                    return new ResultModel<string> { Message = "Invalid user" };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var userRoleIds = await _context.UserRoleIds.Where(m => m.UserId == id).ToListAsync();
                    _context.Users.Remove(user);
                    _context.UserRoleIds.RemoveRange(userRoleIds);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();

                    return new ResultModel<string>() { IsSuccess = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string>();
            }
        }

        public async Task<ResultModel<string>> ToggleLock(int id, bool isLocked)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == _userClaims.CompanyId);
                if (user == null)
                    return new ResultModel<string> { Message = "Invalid user" };

                user.IsLocked = isLocked;
                var affectedRows = await _context.SaveChangesAsync();
                return new ResultModel<string> { IsSuccess = affectedRows > 0 };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string>();
            }
        }

        public async Task<ResultModel<string>> Impersonate(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId && m.CompanyId == _userClaims.CompanyId);
            if (user == null)
                return new ResultModel<string> { Message = "User not found" };

            var token = JwtHelper.GenerateJwt(user, _appSettingsModel.JwtConfig.Secret, _appSettingsModel.JwtConfig.AccessTokenExpiration);
            return new ResultModel<string> { IsSuccess = true, Data = token };
        }

        public async Task<List<CreateUpdateUserRoleItem>> RoleItems(int userId)
        {
            var companyRoles = await _context.UserRoles.Where(m => m.CompanyId == _userClaims.CompanyId).OrderBy(m => m.Name).ToListAsync();
            var userRoleIds = await _context.UserRoleIds.Where(m => m.UserId == userId).ToListAsync();
            var result = new List<CreateUpdateUserRoleItem>();

            foreach (var companyRole in companyRoles)
            {
                var isChecked = false;
                if (userId > 0)
                    isChecked = userRoleIds.FirstOrDefault(m => m.RoleId == companyRole.Id) != null;
                else
                    isChecked = companyRole.IsDefault;

                result.Add(new CreateUpdateUserRoleItem
                {
                    RoleId = companyRole.Id,
                    IsChecked = isChecked,
                    Name = companyRole.Name
                });
            }
            return result;
        }

        public async Task<ResultModel<List<string>>> GetSpecialPermissions(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId && m.CompanyId == _userClaims.CompanyId);

            if (user == null)
                return new ResultModel<List<string>> { Message = "User not found" };

            if (string.IsNullOrWhiteSpace(user.SpecialPermissions))
                return new ResultModel<List<string>> { IsSuccess = true, Data = new List<string>() };

            var permissions = JsonConvert.DeserializeObject<List<string>>(user.SpecialPermissions);
            return new ResultModel<List<string>> { IsSuccess = true, Data = permissions };
        }

        public async Task<ResultModel<string>> SaveSpecialPermissions(int userId, List<string> grantedPermissions)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == userId && m.CompanyId == _userClaims.CompanyId);

            if (user == null)
                return new ResultModel<string> { Message = "User not found" };

            user.SpecialPermissions = JsonConvert.SerializeObject(grantedPermissions);
            var result = await _context.SaveChangesAsync();
            return new ResultModel<string> { IsSuccess = result > 0 };
        }

        private async Task SendActivationEmail(User user, CreateUpdateUserModel model)
        {
            string body;
            if (model.SetRandomPassword)
                body = $"Your Talents account has been created. <a href='{_appSettingsModel.Config.UiProjectUrl}/emailverification/{user.EmailVerificationToken}'>Please click here to activate your account</a>.<br /><br/>Please use this temporary password: {model.Password}";
            else
                body = $"Your Talents account has been created. <a href='{_appSettingsModel.Config.UiProjectUrl}/emailverification/{user.EmailVerificationToken}'>Please click here to activate your account</a>";

            await _emailService.Send(new Models.Email.MailMessageModel
            {
                Body = body,
                HtmlBody = body,
                RecepientEmail = user.Email,
                RecepientFullname = $"{user.FirstName} {user.LastName}",
                Subject = "Activate your account"
            });
        }

        private ResultModel<string> ValidatePicture(IFormFile file)
        {
            var validExtension = _filesService.ValidateImageExtension(file);
            if (!validExtension.IsSuccess) return validExtension;

            return _filesService.ValidateImageSize(file);
        }
    }
}
