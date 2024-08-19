using Cinema.Core.IService;
using Cinema.Core.RequestModel;
using Cinema.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cinema.web.Controllers
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
        public IActionResult Login(LoginRequestModel request)
        {
            var response = _userService.Login(request);
            return Ok(response);
        }

        [HttpPost("check-role-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult checkRoleAdmin()
        {
            return Ok("Admin");
        }
    }
}
