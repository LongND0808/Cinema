using Cinema.Core.IService;
using Cinema.Core.RequestModel.ManageUser;
using Cinema.Core.RequestModel.User;
using Cinema.Core.ResponseModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cinema.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManageUserController : ControllerBase
    {
        private readonly IManageUserService _manageUserService;

        public ManageUserController(IManageUserService manageUserService)
        {
            _manageUserService = manageUserService;
        }

        [HttpPost("get-all-users")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetAllUsers([FromBody] GetUserRequestModel? request = null)
        {
            var response = await _manageUserService.GetAllUsers(request);
            return Ok(response);
        }

        [HttpGet("get-one-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserById(Guid requestId)
        {
            var response = await _manageUserService.GetUserById(requestId);
            return Ok(response);
        }

        [HttpPost("create-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestModel request)
        {
            var response = await _manageUserService.CreateUser(request);
            return Ok(response);
        }

        [HttpPost("update-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequestModel request)
        {
            var response = await _manageUserService.UpdateUser(request);
            return Ok(response);
        }

        [HttpDelete("delete-user")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUser([FromBody] Guid requestId)
        {
            var response = await _manageUserService.DeleteUser(requestId);
            return Ok(response);
        }
    }
}
