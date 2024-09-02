using Cinema.Core.IService;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Services
{
    public class AuthService : IAuthService
    {
        #region Private fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public AuthService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region IsUserInRole method
        public bool IsUserInRole(string roleName)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return false;

            var userClaims = jwtToken.Claims;
            var roles = userClaims.Where(record => record.Type == "role").Select(x => x.Value);

            return roles.Contains(roleName);
        }
        #endregion

        #region IsAuthor method
        public bool IsAuthor(Guid userId)
        {
            var token = _httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadToken(token) as JwtSecurityToken;

            if (jwtToken == null)
                return false;

            var userClaims = jwtToken.Claims;
            var currentUserId = userClaims.FirstOrDefault(item => item.Type == "id");

            if(currentUserId == null)
            {
                return false;
            }    

            return userId.ToString() == currentUserId.Value;
        }
        #endregion
    }
}
