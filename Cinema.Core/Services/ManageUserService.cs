using BaseInsightDotNet.Commons.Enums;
using Cinema.Common.Constant;
using Cinema.Core.DTOs;
using Cinema.Core.IConverters;
using Cinema.Core.InterfaceRepository;
using Cinema.Core.IService;
using Cinema.Core.RequestModel.ManageUser;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Cinema.Core.Services
{
    public class ManageUserService : IManageUserService
    {
        #region Private fields
        private readonly UserManager<User> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthService _authService;
        private readonly IRepository<UserStatus> _userStatusRepository;
        private readonly RoleManager<Role> _roleManager;
        private readonly IRepository<RankCustomer> _rankCustomerRepository;
        private readonly IUserConverter _userConverter;
        #endregion

        #region Constructor
        public ManageUserService(UserManager<User> userManager, IHttpContextAccessor httpContextAccessor, IAuthService authService, IRepository<UserStatus> userStatusRepository, RoleManager<Role> roleManager, IRepository<RankCustomer> rankCustomerRepository, IUserConverter userConverter)
        {
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _authService = authService;
            _userStatusRepository = userStatusRepository;
            _roleManager = roleManager;
            _rankCustomerRepository = rankCustomerRepository;
            _userConverter = userConverter;
        }
        #endregion

        #region CreateUser method
        public async Task<BaseResponseModel<UserDTO>> CreateUser(CreateUserRequestModel request)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var userStatus = await _userStatusRepository.GetByIdAsync(request.UserStatusId);

                if (userStatus == null)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User status does not exist.",
                        Data = null
                    };
                }

                var roles = _roleManager.Roles;

                foreach (var roleId in request.RoleIds)
                {
                    var role = roles.FirstOrDefault(item => item.Id == roleId);

                    if (role == null)
                    {
                        return new BaseResponseModel<UserDTO>
                        {
                            Status = StatusCodes.Status404NotFound,
                            Message = $"Role with ID {roleId} does not exist.",
                            Data = null
                        };
                    }
                }

                if (!Regex.IsMatch(request.Password, Constant.PasswordRegexPattern))
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Password is not valid",
                        Data = null
                    };
                }

                var rankId = await _rankCustomerRepository.GetOneAsyncUntracked(
                    filter: f => f.Point < request.Point,
                    orderBy: o => o.OrderByDescending(x => x.Point),
                    selector: s => s.Id);

                var user = new User
                {
                    UserName = request.UserName,
                    NormalizedUserName = request.UserName.ToUpper(),
                    Email = request.Email,
                    NormalizedEmail = request.Email.ToUpper(),
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    FullName = request.FullName,
                    RankCustomerId = rankId,
                    UserStatusId = request.UserStatusId,
                    Point = request.Point,
                    LockoutEnabled = true,
                    AccessFailedCount = 0,
                    AvatarUrl = request.AvatarUrl,
                    Gender = request.Gender,
                    DateOfBirth = request.DateOfBirth,
                    isDeleted = false,
                    PhoneNumber = request.PhoneNumber,
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Error create new user.",
                        Data = null
                    };
                }


                var rolesName = new List<string>();

                foreach (var roleId in request.RoleIds)
                {
                    var role = roles.FirstOrDefault(item => item.Id == roleId);
                    if(role != null)
                    {
                        rolesName.Add(role.Name);
                    }
                }

                var addRoles = await _userManager.AddToRolesAsync(user, rolesName);

                if(!addRoles.Succeeded)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Error add roles to user.",
                        Data = null
                    };
                }    

                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Create new user successfully.",
                    Data = _userConverter.ConvertToDTO(user)
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion

        #region DeleteUser method
        public async Task<BaseResponseModel<object>> DeleteUser(Guid requestId)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<object>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(requestId.ToString());
                if (user == null)
                {
                    return new BaseResponseModel<object>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = $"User with ID {requestId} does not exist.",
                        Data = null
                    };
                }

                var deletedUserStatusId = await _userStatusRepository.GetOneAsyncUntracked(
                    filter: f => f.Code == "Deleted",
                    selector: s => s.Id);

                user.UserStatusId = deletedUserStatusId;
                user.isDeleted = true;

                var res = await _userManager.UpdateAsync(user);

                if(!res.Succeeded)
                {
                    return new BaseResponseModel<object>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Error deleting user.",
                        Data = null
                    };
                }    

                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User deleted successfully.",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";

                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion

        #region GetAllUsers method
        public async Task<BaseResponseModel<IQueryable<UserDTO>>> GetAllUsers(GetUserRequestModel? request)
        {
            if (!_authService.IsUserInRole("Admin"))
            {
                return new BaseResponseModel<IQueryable<UserDTO>>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You lack the necessary permissions to perform this task.",
                    Data = null
                };
            }

            var users = _userManager.Users;

            if (request.RoleId.HasValue)
            {
                var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                users = users.Where(record => usersInRole.Select(r => r.Id).Contains(record.Id));
            }

            if(request.RankId.HasValue)
            {
                users = users.Where(record => record.RankCustomerId == request.RankId);
            }    

            if(request.StatusId.HasValue)
            {
                users = users.Where(record => record.UserStatusId == request.StatusId);
            }

            return new BaseResponseModel<IQueryable<UserDTO>>
            {
                Status = StatusCodes.Status200OK,
                Message = "Get users successfully.",
                Data = users.Select(r => _userConverter.ConvertToDTO(r))
            };
        }
        #endregion

        #region GetUserById method
        public async Task<BaseResponseModel<UserDTO>> GetUserById(Guid requestId)
        {
            if (!_authService.IsUserInRole("Admin") && !_authService.IsAuthor(requestId))
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status403Forbidden,
                    Message = "You lack the necessary permissions to perform this task.",
                    Data = null
                };
            }

            var user = await _userManager.FindByIdAsync(requestId.ToString());

            if(user == null)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found.",
                    Data = null
                };
            }

            return new BaseResponseModel<UserDTO>
            {
                Status = StatusCodes.Status200OK,
                Message = "User found",
                Data = _userConverter.ConvertToDTO(user)
            };
        }
        #endregion

        #region UpdateUser method
        public async Task<BaseResponseModel<UserDTO>> UpdateUser(UpdateUserRequestModel request)
        {
            try
            {
                if (!_authService.IsUserInRole("Admin"))
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status403Forbidden,
                        Message = "You lack the necessary permissions to perform this task.",
                        Data = null
                    };
                }

                var user = await _userManager.FindByIdAsync(request.Id.ToString());
                if (user == null)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status404NotFound,
                        Message = $"User with ID {request.Id} does not exist.",
                        Data = null
                    };
                }

                var userStatus = await _userStatusRepository.GetByIdAsync(request.UserStatusId);
                if (userStatus == null)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "User status does not exist.",
                        Data = null
                    };
                }

                var roles = _roleManager.Roles;

                foreach (var roleId in request.RoleIds)
                {
                    var role = roles.FirstOrDefault(item => item.Id == roleId);

                    if (role == null)
                    {
                        return new BaseResponseModel<UserDTO>
                        {
                            Status = StatusCodes.Status404NotFound,
                            Message = $"Role with ID {roleId} does not exist.",
                            Data = null
                        };
                    }
                }

                if (!string.IsNullOrEmpty(request.Password))
                {
                    if (!Regex.IsMatch(request.Password, Constant.PasswordRegexPattern))
                    {
                        return new BaseResponseModel<UserDTO>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "New password is not valid",
                            Data = null
                        };
                    }

                    var result = await _userManager.RemovePasswordAsync(user);
                    if (!result.Succeeded)
                    {
                        return new BaseResponseModel<UserDTO>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Error removing old password.",
                            Data = null
                        };
                    }

                    result = await _userManager.AddPasswordAsync(user, request.Password);
                    if (!result.Succeeded)
                    {
                        return new BaseResponseModel<UserDTO>
                        {
                            Status = StatusCodes.Status400BadRequest,
                            Message = "Error adding new password.",
                            Data = null
                        };
                    }
                }

                var rankId = await _rankCustomerRepository.GetOneAsyncUntracked(
                    filter: f => f.Point < request.Point,
                    orderBy: o => o.OrderByDescending(x => x.Point),
                    selector: s => s.Id);

                user.UserName = request.UserName;
                user.NormalizedUserName = request.UserName.ToUpper();
                user.Email = request.Email;
                user.NormalizedEmail = request.Email.ToUpper();
                user.FullName = request.FullName;
                user.RankCustomerId = rankId;
                user.UserStatusId = request.UserStatusId;
                user.Point = request.Point;
                user.AvatarUrl = request.AvatarUrl;
                user.Gender = request.Gender;
                user.DateOfBirth = request.DateOfBirth;
                user.PhoneNumber = request.PhoneNumber;

                var existingRoleNames = await _userManager.GetRolesAsync(user);
                var requestRoleNames = new List<string>();
                foreach (var roleId in request.RoleIds)
                {
                    var role = roles.FirstOrDefault(item => item.Id == roleId);
                    if (role != null)
                    {
                        requestRoleNames.Add(role.Name);
                    }
                }

                var rolesToAdd = requestRoleNames.Except(existingRoleNames);
                var rolesToRemove = existingRoleNames.Except(requestRoleNames);

                foreach (var roleName in rolesToAdd)
                {
                    await _userManager.AddToRoleAsync(user, roleName);
                }

                foreach(var roleName in rolesToRemove)
                {
                    await _userManager.RemoveFromRoleAsync(user, roleName);
                }    

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Error updating user.",
                        Data = null
                    };
                }

                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "User updated successfully.",
                    Data = _userConverter.ConvertToDTO(user)
                };
            }
            catch (Exception ex)
            {
                var errorMessage = "An unexpected error occurred while processing your request.";
                if (ex.InnerException != null)
                {
                    errorMessage += $" Inner Exception: {ex.InnerException.Message}";
                }

                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status500InternalServerError,
                    Message = errorMessage,
                    Data = null
                };
            }
        }
        #endregion
    }
}
