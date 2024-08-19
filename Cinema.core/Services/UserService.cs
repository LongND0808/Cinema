using Cinema.Core.IService;
using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityModel;
using Cinema.Core.InterfaceRepository;

namespace Cinema.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;
        private readonly Identity.ITokenService _tokenService;
        private readonly IRepository<Role> _roleRepository;
        private readonly IRepository<UserRole> _userRoleRepository;

        public UserService(IRepository<User> userRepository, Identity.ITokenService tokenService, IRepository<Role> roleRepository, IRepository<UserRole> userRoleRepository)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<LoginResponseModel> Login(LoginRequestModel request)
        {
            var user = await _userRepository.GetUserByUserNameAsync(request.UserName);
            if (user == null || user.PasswordHash != request.Password)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            var roleIds = await _userRoleRepository.GetRoleIdsByUserID(user.Id);

            if (roleIds == null)
            {
                throw new InvalidOperationException("Role IDs could not be retrieved for the given user.");
            }

            var roleNames = new List<string>();

            foreach(var id in roleIds)
            {
                var role = await _roleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    throw new InvalidOperationException("Role could not be retrieved for the given user.");
                }

                roleNames.Add(role.Name);
            }

            var claims = new List<Claim>();

            foreach(var name in roleNames)
            {
                claims.Add(new Claim(JwtClaimTypes.Role, name));
            }    

            var accessToken = await _tokenService.CreateAccessTokenAsync(user, claims);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return new LoginResponseModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
    }
}
