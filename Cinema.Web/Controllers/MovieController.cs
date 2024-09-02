using Cinema.Core.IService;
using Cinema.Core.RequestModel.Movie;
using Cinema.Core.ResponseModel;
using Cinema.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Cinema.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;

        public MovieController(IMovieService movieService)
        { 
            _movieService = movieService;
        }

        [HttpPost("get-all-movie")]
        public async Task<IActionResult> GetAllMovie([FromBody] GetMovieRequestModel? request = null)
        {
            var response = await _movieService.GetAllMovie(request);
            return Ok(response);
        }

        [HttpGet("get-one-movie")]
        public async Task<IActionResult> GetMovieById(Guid requestId)
        {
            var response = await _movieService.GetMovieById(requestId);
            return Ok(response);
        }

        [HttpPost("create-movie")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateMovie([FromBody] CreateMovieRequestModel request)
        {
            var response = await _movieService.CreateMovie(request);
            return Ok(response);
        }

        [HttpPost("update-movie")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateMovie([FromBody] UpdateMovieRequestModel request)
        {
            var response = await _movieService.UpdateMovie(request);
            return Ok(response);
        }

        [HttpDelete("delete-movie")]
        public async Task<IActionResult> DeleteMovie(Guid requestId)
        {
            var response = await _movieService.DeleteMovie(requestId);
            return Ok(response);
        }
    }
}
