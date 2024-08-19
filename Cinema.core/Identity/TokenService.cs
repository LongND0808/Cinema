using IdentityServer4.Services;
using IdentityServer4.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using Cinema.Domain.Entities;
using Cinema.Core.InterfaceRepository;

public class TokenService : Cinema.Core.Identity.ITokenService
{
    private readonly ITokenCreationService _tokenCreationService;
    private readonly ITokenService _tokenService;
    private readonly IRepository<Cinema.Domain.Entities.RefreshToken> _refreshTokenRepository;

    public TokenService(ITokenCreationService tokenCreationService)
    {
        _tokenCreationService = tokenCreationService;
    }

    public async Task<string> CreateAccessTokenAsync(User user, ICollection<Claim> claims)
    {
        var token = new Token
        {
            Claims = claims,
            Lifetime = 3600, 
            AccessTokenType = AccessTokenType.Jwt,
            ClientId = "client", 
            Description = "Access Token for User", 
            Audiences = { "api1" } 
        };

        var createdToken = await _tokenCreationService.CreateTokenAsync(token);

        return createdToken;
    }


    public async Task<string> CreateRefreshTokenAsync(User user)
    {
        var refreshToken = Guid.NewGuid().ToString();

        var refreshTokenEntity = new Cinema.Domain.Entities.RefreshToken
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpiredTime = DateTime.UtcNow.AddDays(30), 
        };

        await _refreshTokenRepository.AddAsync(refreshTokenEntity);

        return refreshToken;
    }

}
