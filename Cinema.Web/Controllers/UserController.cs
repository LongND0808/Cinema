using Cinema.Core.IService;
using Cinema.Core.RequestModel.User;
using Cinema.Core.ResponseModel;
using Cinema.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
        {
            var response = await _userService.Login(request);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestModel request)
        {
            var response = await _userService.Register(request);
            return Ok(response);
        }

        [HttpPost("authenticate-registration")]
        public async Task<IActionResult> AuthenticateRegistration([FromBody] AuthenticateRegistrationRequestModel request)
        {
            var response = await _userService.AuthenticateRegistration(request);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestModel request)
        {
            var response = await _userService.ForgotPassword(request);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestModel request)
        {
            var response = await _userService.ResetPassword(request);
            return Ok(response);
        }

        [HttpPost("resend-active-code")]
        public async Task<IActionResult> ResendActiveCode([FromBody] ResendActiveCodeRequestModel request)
        {
            var response = await _userService.ResendActiveCode(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestModel request)
        {
            var response = await _userService.ChangePassword(request);
            return Ok(response);
        }
    }
}
