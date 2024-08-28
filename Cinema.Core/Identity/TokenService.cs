﻿using Cinema.Domain.Entities;
using Cinema.Core.InterfaceRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

public class TokenService : Cinema.Core.Identity.ITokenService
{
    private readonly IRepository<RefreshToken> _refreshTokenRepository;
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public TokenService(IRepository<RefreshToken> refreshTokenRepository, IConfiguration configuration, UserManager<User> userManager)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _configuration = configuration;
        _userManager = userManager;
    }

    public async Task<string> CreateAccessTokenAsync(User user)
    {
        var secretKey = _configuration["JWT:Secret"];
        var issuer = _configuration["JWT:ValidIssuer"];
        var audience = _configuration["JWT:ValidAudience"];
        var tokenValidityInHours = int.TryParse(_configuration["JWT:TokenValidityInHours"], out int hours) ? hours : 1;

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

        var userRoles = await _userManager.GetRolesAsync(user);

        var claims = new List<Claim>
            {
                new(JwtClaimTypes.Id, user.Id.ToString()),
                new(JwtClaimTypes.Name, user.UserName),
                new(JwtClaimTypes.Email, user.Email),
                new(JwtClaimTypes.GivenName, user.FullName),
            };

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.Now.AddHours(tokenValidityInHours),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(securityToken);

        return tokenString;
    }

    public async Task<string> CreateRefreshTokenAsync(User user)
    {
        var existingTokens = await _refreshTokenRepository.GetAllAsync(x => x.UserId == user.Id);

        var lastToken = existingTokens.LastOrDefault();

        if (lastToken != null && lastToken.ExpiredTime > DateTime.Now)
        {
            return lastToken.Token;
        }

        var refreshToken = Guid.NewGuid().ToString();
        var refreshTokenValidity = int.TryParse(_configuration["JWT:RefreshTokenValidity"], out int validity) ? validity : 7;

        var refreshTokenEntity = new RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            CreateTime = DateTime.Now,
            ExpiredTime = DateTime.Now.AddDays(refreshTokenValidity),
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return refreshToken;
    }

}
