using Cinema.Core.IService;
using Cinema.Core.RequestModel;
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
        public async Task<IActionResult> Login(LoginRequestModel request)
        {
            var response = await _userService.Login(request);
            return Ok(response);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestModel request)
        {
            var response = await _userService.Register(request);
            return Ok(response);
        }

        [HttpPost("authenticate-registration")]
        public async Task<IActionResult> AuthenticateRegistration(AuthenticateRegistrationRequestModel request)
        {
            var response = await _userService.AuthenticateRegistration(request);
            return Ok(response);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestModel request)
        {
            var response = await _userService.ForgotPassword(request);
            return Ok(response);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestModel request)
        {
            var response = await _userService.ResetPassword(request);
            return Ok(response);
        }

        [HttpPost("resend-active-code")]
        public async Task<IActionResult> ResendActiveCode(ResendActiveCodeRequestModel request)
        {
            var response = await _userService.ResendActiveCode(request);
            return Ok(response);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequestModel request)
        {
            var response = await _userService.ChangePassword(request);
            return Ok(response);
        }
    }
}
