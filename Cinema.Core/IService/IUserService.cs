using Cinema.Core.DTOs;
using Cinema.Core.RequestModel.User;
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
        Task<BaseResponseModel<UserDTO>> AuthenticateRegistration(AuthenticateRegistrationRequestModel request);
        Task<BaseResponseModel<object>> ChangePassword(ChangePasswordRequestModel request);
        Task<BaseResponseModel<object>> ForgotPassword(ForgotPasswordRequestModel request);
        Task<BaseResponseModel<LoginResponseModel>> Login(LoginRequestModel request);
        Task<BaseResponseModel<UserDTO>> Register(RegisterRequestModel request);
        Task<BaseResponseModel<object>> ResendActiveCode(ResendActiveCodeRequestModel request);
        Task<BaseResponseModel<object>> ResetPassword(ResetPasswordRequestModel request);
    }
}
