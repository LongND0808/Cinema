using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.Identity
{
    public interface ITokenService
    {
        Task<string> CreateAccessTokenAsync(User user, ICollection<Claim> claims);
        Task<string> CreateRefreshTokenAsync(User user);
    }
}
