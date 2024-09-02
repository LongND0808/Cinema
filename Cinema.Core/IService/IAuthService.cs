using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IService
{
    public interface IAuthService
    {
        bool IsUserInRole(string roleName);

        bool IsAuthor(Guid userId);
    }
}
