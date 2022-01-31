using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TalentsApi.Access;
using TalentsApi.DataAccess;
using TalentsApi.Models;
using TalentsApi.Models.AppSettings;
using TalentsApi.Models.Auth;
using TalentsApi.Models.Entities;
using TalentsApi.Models.UserRole;

namespace TalentsApi.Services
{
    public class UserRoleService
    {
        private readonly ILogger<UserRoleService> _logger;
        private readonly AppSettingsModel _appSettingsModel;
        private readonly TalentsDbContext _context;
        private readonly IHttpContextAccessor _httpContext;
        private readonly UserClaims _userClaims;
        private readonly IMapper _mapper;

        public UserRoleService(AppSettingsModel appSettingsModel,
            TalentsDbContext context,
            ILogger<UserRoleService> logger,
            IHttpContextAccessor httpContext,
            IMapper mapper)
        {
            _context = context;
            _appSettingsModel = appSettingsModel;
            _logger = logger;
            _httpContext = httpContext;
            _userClaims = (UserClaims)_httpContext.HttpContext.Items["UserClaims"];
            _mapper = mapper;
        }

        public async Task<List<UserRoleModel>> All(string search)
        {
            var roles = new List<UserRole>();
            if (string.IsNullOrWhiteSpace(search))
            {
                roles = await _context.UserRoles
                            .Where(m => m.CompanyId == _userClaims.CompanyId)
                            .OrderBy(m => m.Name)
                            .ToListAsync();
            }
            else
            {
                search = search.ToUpper();
                roles = await _context.UserRoles
                            .Where(m => m.CompanyId == _userClaims.CompanyId && m.Name.ToUpper().Contains(search))
                            .OrderBy(m => m.Name)
                            .ToListAsync();
            }
            return _mapper.Map<List<UserRoleModel>>(roles);
        }

        public async Task<UserRoleModel> ById(int id)
        {
            var role = await _context.UserRoles.FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == _userClaims.CompanyId);
            return _mapper.Map<UserRoleModel>(role);
        }

        public async Task<ResultModel<List<UserRoleModel>>> ByUserId(int id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
                return new ResultModel<List<UserRoleModel>> { Message = "Invalid user id" };

            // var role = await _context.UserRoles.FirstOrDefaultAsync(m => m.Id == user.RoleId);
            var userRoleIds = await _context.UserRoleIds.Where(m => m.UserId == id).Select(m => m.Id).ToListAsync();
            var roles = await _context.UserRoles.Where(m => userRoleIds.Any(a => m.Id == a)).ToListAsync();
            if (!roles.Any())
                return new ResultModel<List<UserRoleModel>> { Message = "User is not assigned to any role" };

            return new ResultModel<List<UserRoleModel>>
            {
                Data = _mapper.Map<List<UserRoleModel>>(roles)
            };
        }

        public async Task<ResultModel<string>> Create(CreateUpdateUserRoleModel model)
        {
            try
            {
                var sameName = await _context.UserRoles.FirstOrDefaultAsync(m => m.Name.ToUpper() == model.Name.ToUpper() && m.Id != model.Id && m.CompanyId == _userClaims.CompanyId);
                if (sameName != null)
                    return new ResultModel<string> { Message = $"User role with name '{model.Name}' already exists!" };

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    var userRole = new UserRole
                    {
                        Name = model.Name,
                        CompanyId = _userClaims.CompanyId,
                        DateCreated = DateTime.UtcNow,
                        IsDefault = model.IsDefault,
                        Permission = JsonConvert.SerializeObject(model.GrantedPermissions)
                    };

                    //if (model.IsDefault)
                    //{
                    //    var roles = await _context.UserRoles.Where(m => m.CompanyId == _userClaims.CompanyId).ToListAsync();
                    //    roles.ForEach(role => role.IsDefault = false);
                    //}
                    await _context.UserRoles.AddAsync(userRole);
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ResultModel<string> { IsSuccess = true, Message = "User role created successfully!" };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> Update(CreateUpdateUserRoleModel model)
        {
            try
            {
                var sameName = await _context.UserRoles.FirstOrDefaultAsync(m => m.Name.ToUpper() == model.Name.ToUpper() && m.Id != model.Id && m.CompanyId == _userClaims.CompanyId);
                if (sameName != null)
                    return new ResultModel<string> { Message = $"User role with name '{model.Name}' already exists!" };

                var userRoleFromDb = await _context.UserRoles.FirstOrDefaultAsync(m => m.Id == model.Id && m.CompanyId == _userClaims.CompanyId);
                if (userRoleFromDb == null)
                    return new ResultModel<string> { Message = $"User role not found!" };

                // validate if the user is editing the admins permission to manage roles
                if (!model.GrantedPermissions.Contains(Access.UserRoleAccess.Create) ||
                    !model.GrantedPermissions.Contains(Access.UserRoleAccess.Update) ||
                    !model.GrantedPermissions.Contains(Access.UserRoleAccess.Delete))
                {
                    if (userRoleFromDb.IsAdmin)
                        return new ResultModel<string> { Message = "You can not remove User/Role permission editing from the admin role" };
                }

                using (var transaction = await _context.Database.BeginTransactionAsync())
                {
                    //if (model.IsDefault)
                    //{
                    //    var roles = await _context.UserRoles.Where(m => m.CompanyId == _userClaims.CompanyId).ToListAsync();
                    //    roles.ForEach(role => role.IsDefault = false);
                    //}

                    userRoleFromDb.Permission = JsonConvert.SerializeObject(model.GrantedPermissions);
                    userRoleFromDb.IsDefault = model.IsDefault;
                    userRoleFromDb.Name = model.Name;
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return new ResultModel<string> { IsSuccess = true, Message = "User role updated!" };
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public async Task<ResultModel<string>> Delete(int id)
        {
            try
            {
                var userRoleFromDb = await _context.UserRoles.FirstOrDefaultAsync(m => m.Id == id && m.CompanyId == _userClaims.CompanyId);
                if (userRoleFromDb == null)
                    return new ResultModel<string> { Message = $"User role not found!" };

                var result = _context.UserRoles.Remove(userRoleFromDb);
                await _context.SaveChangesAsync();
                return new ResultModel<string> { IsSuccess = true, Message = "User role deleted!" };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, string.Empty);
                return new ResultModel<string> { Message = $"An error occured" };
            }
        }

        public List<UserRole> GenerateDefaultRoles(int companyId)
        {
            var result = new List<UserRole>();
            result.Add(new UserRole
            {
                IsAdmin = true,
                CompanyId = companyId,
                DateCreated = DateTime.UtcNow,
                IsDefault = false,
                Name = "Admin",
                Permission = JsonConvert.SerializeObject(PermissionList.Permissions.Select(m => m.Id).ToList())
            });
            result.Add(new UserRole
            {
                CompanyId = companyId,
                DateCreated = DateTime.UtcNow,
                IsDefault = true,
                Name = "User",
                Permission = string.Empty
            });
            return result;
        }

        public List<UserRoleTreeItemModel> GetPermissionsForTreeView()
        {
            // stucture ng tree item
            //{
            //      title: 'parent 1-0',
            //      key: '0-0-0',
            //      children: [
            //          {
            //              title: 'leaf',
            //              key: '0-0-0-0',
            //          },
            //          {
            //              title: 'leaf',
            //              key: '0-0-0-1',
            //          },
            //      ],
            // }
            var permissions = PermissionList.Permissions;
            var rootParent = permissions.FirstOrDefault(m => m.ParentId == null);
            return new List<UserRoleTreeItemModel>
            {
                new UserRoleTreeItemModel
                {
                    Children = GetChildren(permissions, rootParent.Id),
                    Key = rootParent.Id,
                    Title = rootParent.Name,
                    ParentKey = null
                }
            };
        }

        private List<UserRoleTreeItemModel> GetChildren(List<Permission> permissions, string parentId)
        {
            return permissions.Where(m => m.ParentId == parentId)
                                .Select(m => new UserRoleTreeItemModel
                                {
                                    Key = m.Id,
                                    Title = m.Name,
                                    ParentKey = parentId,
                                    Children = GetChildren(permissions, m.Id)
                                })
                                .ToList();
        }
    }
}
