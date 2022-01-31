using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentsApi.Access;
using TalentsApi.Models.UserRole;
using TalentsApi.Services;

namespace TalentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly UserRoleService _userRoleService;

        public UserRoleController(UserRoleService userRoleService)
        {
            _userRoleService = userRoleService;
        }

        [HttpGet]
        [Authorize(UserRoleAccess.View)]
        public async Task<IActionResult> Get(string search)
        {
            var result = await _userRoleService.All(search);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(UserRoleAccess.View)]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _userRoleService.ById(id);
            return Ok(result);
        }

        [HttpPost]
        [Authorize(UserRoleAccess.Create)]
        public async Task<IActionResult> Post([FromBody] CreateUpdateUserRoleModel model)
        {
            var result = await _userRoleService.Create(model);
            return Ok(result);
        }

        [HttpPut]
        [Authorize(UserRoleAccess.Update)]
        public async Task<IActionResult> Put([FromBody] CreateUpdateUserRoleModel model)
        {
            var result = await _userRoleService.Update(model);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [Authorize(UserRoleAccess.Delete)]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _userRoleService.Delete(id);
            return Ok(result);
        }

        [HttpGet("allpermissionstree")]
        [AllowAnonymous]
        public async Task<IActionResult> PermissionsForTreeView()
        {
            var items = _userRoleService.GetPermissionsForTreeView();
            return Ok(items);
        }
    }
}