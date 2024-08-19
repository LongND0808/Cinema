using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IService
{
    public interface IUserService
    {
        Task<LoginResponseModel> Login(LoginRequestModel request);
    }
}
