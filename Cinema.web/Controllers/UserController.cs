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
    }
}
