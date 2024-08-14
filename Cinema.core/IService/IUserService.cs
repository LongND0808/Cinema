using Cinema.core.RequestModel;
using Cinema.core.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.core.IService
{
    public interface IUserService
    {
        Task<LoginResponseModel> Login(LoginRequestModel request);
    }
}
