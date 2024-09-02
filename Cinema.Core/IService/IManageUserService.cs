using Cinema.Core.DTOs;
using Cinema.Core.RequestModel.ManageUser;
using Cinema.Core.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.Core.IService
{
    public interface IManageUserService
    {
        Task<BaseResponseModel<UserDTO>> CreateUser(CreateUserRequestModel request);
        Task<BaseResponseModel<object>> DeleteUser(Guid requestId);
        Task<BaseResponseModel<IQueryable<UserDTO>>> GetAllUsers(GetUserRequestModel? request);
        Task<BaseResponseModel<UserDTO>> GetUserById(Guid requestId);
        Task<BaseResponseModel<UserDTO>> UpdateUser(UpdateUserRequestModel request);
    }
}
