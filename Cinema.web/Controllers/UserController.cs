using Microsoft.AspNetCore.Mvc;

namespace Cinema.web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = new List<string> { "User1", "User2", "User3" };
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUser(int id)
        {
            var user = $"User{id}";
            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] string user)
        {
            return Ok($"User '{user}' đã được tạo.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] string user)
        {
            return Ok($"User ID '{id}' đã được cập nhật thành '{user}'.");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            return Ok($"User ID '{id}' đã được xóa.");
        }
    }
}
