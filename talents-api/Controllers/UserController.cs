using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentsApi.Access;
using TalentsApi.Models.Auth;
using TalentsApi.Models.User;
using TalentsApi.Services;

namespace TalentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize(UserAccess.View)]
        public async Task<IActionResult> Get(string search)
        {
            var result = await _userService.All(search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(UserAccess.View)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _userService.ById(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(UserAccess.Create)]
        public async Task<IActionResult> Post([FromForm] CreateUpdateUserModel model)
        {
            var result = await _userService.Create(model);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(UserAccess.Update)]
        public async Task<IActionResult> Put([FromForm] CreateUpdateUserModel model)
        {
            var result = await _userService.Update(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(UserAccess.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userService.Delete(id);
            return Ok(result);
        }

        [HttpPost("togglelock")]
        [Authorize(UserAccess.Lock)]
        public async Task<IActionResult> ToggleLock([FromBody]ToggleLockModel model)
        {
            var result = await _userService.ToggleLock(model.Id, model.IsLocked);
            return Ok(result);
        }

        [HttpGet("roleitems/{id}")]
        [Authorize]
        public async Task<IActionResult> RoleItems(int id)
        {
            var result = await _userService.RoleItems(id);
            return Ok(result);
        }

        [HttpPost("impersonate/{userId}")]
        [Authorize(UserAccess.Impersonate)]
        public async Task<IActionResult> Impersonate(int userId)
        {
            var result = await _userService.Impersonate(userId);
            return Ok(result);
        }

        [HttpGet("specialpermissions/{userId}")]
        [Authorize(UserAccess.SpecialPermission)]
        public async Task<IActionResult> GetSpecialPermissions(int userId)
        {
            var result = await _userService.GetSpecialPermissions(userId);
            return Ok(result);
        }

        [HttpPost("specialpermissions/{userId}")]
        [Authorize(UserAccess.SpecialPermission)]
        public async Task<IActionResult> PostSpecialPermissions([FromBody] List<string> grantedPermissions, int userId)
        {
            var result = await _userService.SaveSpecialPermissions(userId, grantedPermissions);
            return Ok(result);
        }

        [HttpGet("permissions")]
        [Authorize]
        public IActionResult Permissions()
        {
            var userClaims = (UserClaims)HttpContext.Items["UserClaims"];
            var result = userClaims.Permissions;
            return Ok(result);
        }
    }
}