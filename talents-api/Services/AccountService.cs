using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Access;
using TalentsApi.DataAccess;
using TalentsApi.Helpers.Auth;
using TalentsApi.Helpers.Security;
using TalentsApi.Models;
using TalentsApi.Models.Account;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Entities;

namespace TalentsApi.Services
{
    public class AccountService
    {
        private readonly ILogger<AccountService> _logger;
        private readonly AppSettingsModel _appSettingsModel;
        private readonly TalentsDbContext _context;
        private readonly EmailService _emailService;
        private readonly UserRoleService _userRoleService;

        public AccountService(AppSettingsModel appSettingsModel,
            TalentsDbContext context,
            ILogger<AccountService> logger,
            EmailService emailService,
            UserRoleService userRoleService)
        {
            _context = context;
            _appSettingsModel = appSettingsModel;
            _logger = logger;
            _emailService = emailService;
            _userRoleService = userRoleService;
        }

        public async Task<ResultModel<string>> Login(LoginModel credentials)
        {
            var token = string.Empty;
            try
            {
                var encryptedPassword = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, credentials.Password);
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == credentials.Email && m.Password == encryptedPassword);
                if (user == null)
                    return new ResultModel<string> { Message = $"Invalid email or password." };
                if (user.IsLocked)
                    return new ResultModel<string> { Message = $"User account is locked. Please contact your administrator." };
                if (!user.IsEmailVerified && user.EmailVerificationRequired)
                    return new ResultModel<string> { Message = $"Please verify your email address first." };

                if (user.ShouldSetPasswordOnNextLogin)
                {
                    token = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{user.Email}");
                    user.ResetPasswordToken = token;
                    user.ResetPasswordExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours);
                }

                user.LastLogin = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                if (user.ShouldSetPasswordOnNextLogin)
                    return new ResultModel<string>() { IsSuccess = true, Message = "should-set-password", Data = token };

                token = JwtHelper.GenerateJwt(user, _appSettingsModel.JwtConfig.Secret, _appSettingsModel.JwtConfig.AccessTokenExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }

            return new ResultModel<string> { IsSuccess = true, Data = token };
        }

        public async Task<ResultModel<string>> Register(RegisterModel data)
        {
            try
            {
                var sameEmail = await _context.Users.FirstOrDefaultAsync(m => m.Email.ToUpper() == data.Email.ToUpper());
                if (sameEmail != null)
                    return new ResultModel<string>() { Message = $"Email address { data.Email } already registered" };

                var encryptedPassword = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, data.Password);
                var verificationToken = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{data.Email}");
                var user = new User
                {
                    Email = data.Email,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    RegistrationDate = DateTime.UtcNow,
                    Password = encryptedPassword,
                    EmailVerificationToken = verificationToken,
                    EmailVerificationExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours),
                    SpecialPermissions = JsonConvert.SerializeObject(PermissionList.Permissions.Select(m =>m.Id).ToList())
                };

                var company = new Company
                {
                    Name = data.CompanyName,
                    Size = data.CompanySize
                };

                using (var transaction = _context.Database.BeginTransaction())
                {
                    await _context.Companies.AddAsync(company);
                    await _context.SaveChangesAsync();

                    var defaultRoles = _userRoleService.GenerateDefaultRoles(company.Id);
                    await _context.UserRoles.AddRangeAsync(defaultRoles);
                    await _context.SaveChangesAsync();

                    user.CompanyId = company.Id;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();

                    var userRoleId = new UserRoleIds
                    {
                        CompanyId = company.Id,
                        UserId = user.Id,
                        RoleId = defaultRoles.FirstOrDefault(m => m.Name == "Admin").Id
                    };
                    await _context.UserRoleIds.AddAsync(userRoleId);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    await SendVerificationEmail(user);
                    return new ResultModel<string> { IsSuccess = true };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> VerifyEmail(string verificationToken)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.EmailVerificationToken == verificationToken);
                if (user == null)
                    return new ResultModel<string> { Message = "Invalid verification link" };

                if (user.EmailVerificationExpiration < DateTime.UtcNow)
                    return new ResultModel<string> { Message = "Verification link expired" };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    user.EmailVerificationExpiration = null;
                    user.EmailVerificationToken = null;
                    user.IsEmailVerified = true;

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                return new ResultModel<string> { IsSuccess = true, Message = "Email address successfully verified" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> ResendEmailVerification(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
                if (user == null)
                    return new ResultModel<string> { Message = "Email address not found" };

                var verificationToken = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{email}");
                user.EmailVerificationToken = verificationToken;
                user.EmailVerificationExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours);
                await _context.SaveChangesAsync();
                await SendVerificationEmail(user);

                return new ResultModel<string> { IsSuccess = true, Message = "Email verification sent, please check your email address" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> RequestResetPassword(string email)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.Email == email);
                if (user == null)
                    return new ResultModel<string> { Message = "Email address not found" };

                var resetToken = Encryption.sha256encrypt(_appSettingsModel.JwtConfig.Secret, $"{Guid.NewGuid()}{email}");
                user.ResetPasswordToken = resetToken;
                user.ResetPasswordExpiration = DateTime.UtcNow.AddHours(_appSettingsModel.Config.TokenValidityHours);
                await _context.SaveChangesAsync();
                await SendResetPasswordEmail(user);

                return new ResultModel<string> { IsSuccess = true, Message = "Email verification sent, please check your email address" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> ResetPassword(ResetPasswordModel model)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(m => m.ResetPasswordToken == model.Token);
                if (user == null)
                    return new ResultModel<string> { Message = "Invalid request" };

                user.ResetPasswordExpiration = null;
                user.ResetPasswordToken = null;
                user.IsLocked = false;
                user.Password = Encryption.sha256encrypt(_appSettingsModel.Config.EncryptionSalt, model.Password);
                await _context.SaveChangesAsync();
                await SendPasswordChangedEmail(user);

                var token = JwtHelper.GenerateJwt(user, _appSettingsModel.JwtConfig.Secret, _appSettingsModel.JwtConfig.AccessTokenExpiration);
                return new ResultModel<string> { Data = token, IsSuccess = true, Message = "Password successfully changed!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        private async Task SendVerificationEmail(User user)
        {
            var body = $"Thank you for registration <a href='{_appSettingsModel.Config.UiProjectUrl}/emailverification/{user.EmailVerificationToken}'>click here to verify your email address</a>";
            await _emailService.Send(new Models.Email.MailMessageModel
            {
                Body = body,
                HtmlBody = body,
                RecepientEmail = user.Email,
                RecepientFullname = $"{user.FirstName} {user.LastName}",
                Subject = "Confirm your registration"
            });
        }

        private async Task SendResetPasswordEmail(User user)
        {
            var body = $"You requested to reset your password. Please <a href='{_appSettingsModel.Config.UiProjectUrl}/resetpassword/{user.ResetPasswordToken}'>click here to change your password</a>";
            await _emailService.Send(new Models.Email.MailMessageModel
            {
                Body = body,
                HtmlBody = body,
                RecepientEmail = user.Email,
                RecepientFullname = $"{user.FirstName} {user.LastName}",
                Subject = "Forgot Password"
            });
        }

        private async Task SendPasswordChangedEmail(User user)
        {
            var body = $"You have successfully changed your password. If you don't recognize this request please do something.";
            await _emailService.Send(new Models.Email.MailMessageModel
            {
                Body = body,
                HtmlBody = body,
                RecepientEmail = user.Email,
                RecepientFullname = $"{user.FirstName} {user.LastName}",
                Subject = "Password Changed"
            });
        }
    }
}
