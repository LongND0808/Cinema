using Cinema.core.IService;
using Cinema.core.RequestModel;
using Cinema.core.ResponseModel;
using Cinema.Core.InterfaceRepository;
using Cinema.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.core.Services
{
    public class UserService : IUserService
    {
        private readonly IRepository<User> _userRepository;

        public UserService(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public Task<LoginResponseModel> Login(LoginRequestModel request)
        {
            throw new NotImplementedException();
        }
    }
}
