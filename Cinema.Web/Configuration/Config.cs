using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;

namespace Cinema.Web.Configuration
{
    public class Config
    {
        public IEnumerable<IdentityResource> IdentityResources { get; }
        public IEnumerable<ApiScope> ApiScopes { get; }
        public IEnumerable<ApiResource> ApiResources { get; }
        public IEnumerable<Client> Clients { get; }

        public Config(IConfiguration configuration)
        {
            IdentityResources = configuration.GetSection("IdentityServer:IdentityResources")
                .Get<IEnumerable<IdentityResource>>();

            ApiScopes = configuration.GetSection("IdentityServer:ApiScopes")
                .Get<IEnumerable<ApiScope>>();

            ApiResources = configuration.GetSection("IdentityServer:ApiResources")
                .Get<IEnumerable<ApiResource>>();

            Clients = configuration.GetSection("IdentityServer:Clients")
                .Get<IEnumerable<Client>>();
        }
    }
}
