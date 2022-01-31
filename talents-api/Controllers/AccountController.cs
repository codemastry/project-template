using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TalentsApi.Models;
using TalentsApi.Models.Account;
using TalentsApi.Services;

namespace TalentsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AccountService _accountService;

        public AccountController(UserService userService,
            AccountService accountService)
        {
            _userService = userService;
            _accountService = accountService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var result = await _accountService.Login(model);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel data)
        {
            var result = await _accountService.Register(data);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("emailverification")]
        public async Task<IActionResult> EmailVerification([FromBody] TokenModel model)
        {
            var result = await _accountService.VerifyEmail(model.Token);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("resendemailverification")]
        public async Task<IActionResult> ResendEmailVerification([FromBody] TokenModel model)
        {
            var result = await _accountService.ResendEmailVerification(model.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("requestresetpassword")]
        public async Task<IActionResult> RequestResetPassword([FromBody] TokenModel model)
        {
            var result = await _accountService.RequestResetPassword(model.Email);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("resetpassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            var result = await _accountService.ResetPassword(model);
            return Ok(result);
        }


        [HttpGet("checkauth")]
        [Authorize]
        public ActionResult<bool> CheckAuth()
        {
            return Ok(true);
        }
    }
}