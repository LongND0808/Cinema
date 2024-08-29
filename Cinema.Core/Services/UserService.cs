using Cinema.Core.IService;
using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Cinema.Core.InterfaceRepository;
using IdentityModel;
using Microsoft.AspNetCore.Http;
using Cinema.Core.DTOs;
using Cinema.Core.Email;
using System.Linq.Expressions;
using Cinema.Core.Converters;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;
using System.Data.SqlTypes;
using System.Text;
using Cinema.Common.Constant;
using System.Text.RegularExpressions;
using Cinema.Core.IConverters;

namespace Cinema.Core.Services
{
    public class UserService : IUserService
    {
        #region private fields
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Identity.ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRepository<RankCustomer> _rankCustomerRepository;
        private readonly IRepository<UserStatus> _userStatusRepository;
        private readonly IUserConverter _userConverter;
        private readonly IRepository<ConfirmEmail> _confirmEmailRepository;

        private readonly Random _random = new();
        #endregion

        #region constructer
        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            Identity.ITokenService tokenService,
            IEmailService emailService,
            IRepository<RankCustomer> rankCustomerRepository,
            IRepository<UserStatus> userStatusRepository,
            IUserConverter userConverter,
            IRepository<ConfirmEmail> confirmEmailRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailService = emailService;
            _rankCustomerRepository = rankCustomerRepository;
            _userStatusRepository = userStatusRepository;
            _userConverter = userConverter;
            _confirmEmailRepository = confirmEmailRepository;
        }
        #endregion

        #region Private method
        private string GenerateCode()
        {
            var random = new Random();

            int code = random.Next(0, 1000000);

            return code.ToString("D6");
        }

        private async Task<Guid> GetUnrankedRankCustomerIdAsync()
        {
            try
            {
                return await _rankCustomerRepository.GetOneAsyncUntracked(
                    filter: f => f.Name == "Unranked",
                    selector: s => s.Id);
            }
            catch (Exception e)
            {
                throw new Exception("Unranked RankCustomer not found");
            }
        }


        private async Task<Guid> GetPendingUserStatusIdAsync()
        {
            try
            {
                return await _userStatusRepository.GetOneAsyncUntracked(
                    filter: f => f.Code == "Pending",
                    selector: s => s.Id);
            }
            catch (Exception e)
            {
                throw new Exception("Pending UserStatus not found");
            }
        }

        private async Task<Guid> GetActiveUserStatusIdAsync()
        {
            try
            {
                return await _userStatusRepository.GetOneAsyncUntracked(
                    filter: f => f.Code == "Active",
                    selector: s => s.Id);
            }
            catch (Exception e)
            {
                throw new Exception("Active UserStatus not found");
            }
        }

        private async Task<ConfirmEmail?> GetLastConfirmCode(Guid userId)
        {
            try
            {
                return await _confirmEmailRepository.GetOneAsyncUntracked<ConfirmEmail>(
                filter: f => f.UserId == userId,
                orderBy: o => o.OrderByDescending(b => b.RequiredDateTime));
            }
            catch (Exception e)
            {
                throw new Exception($"Error while retrieving the last confirm code for user with ID {userId}: {e.Message}", e);
            }
        }

        public string GenerateRandomPassword(int length = 8)
        {
            if (length < 8) length = 8;

            StringBuilder password = new StringBuilder();

            password.Append(Constant.UppercaseChars[_random.Next(Constant.UppercaseChars.Length)]);

            password.Append(Constant.SpecialChars[_random.Next(Constant.SpecialChars.Length)]);

            password.Append(Constant.DigitChars[_random.Next(Constant.DigitChars.Length)]);

            password.Append(Constant.LowercaseChars[_random.Next(Constant.LowercaseChars.Length)]);

            for (int i = 4; i < length; i++)
            {
                password.Append(Constant.AllChars[_random.Next(Constant.AllChars.Length)]);
            }

            return new string(password.ToString().ToCharArray().OrderBy(s => _random.Next()).ToArray());
        }
        #endregion

        #region Login method
        public async Task<BaseResponseModel<LoginResponseModel>> Login(LoginRequestModel request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null)
            {
                return new BaseResponseModel<LoginResponseModel>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User does not exist",
                    Data = new LoginResponseModel
                    {
                        AccessToken = "",
                        RefreshToken = ""
                    }
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                return new BaseResponseModel<LoginResponseModel>
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Message = "Invalid credentials",
                    Data = new LoginResponseModel
                    {
                        AccessToken = "",
                        RefreshToken = ""
                    }
                };
            }


            var accessToken = await _tokenService.CreateAccessTokenAsync(user);
            var refreshToken = await _tokenService.CreateRefreshTokenAsync(user);

            return new BaseResponseModel<LoginResponseModel>
            {
                Status = StatusCodes.Status200OK,
                Message = "Login success",
                Data = new LoginResponseModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                }
            };
        }

        #endregion

        #region Register moethod
        public async Task<BaseResponseModel<UserDTO>> Register(RegisterRequestModel request)
        {
            var existingUser = await _userManager.FindByNameAsync(request.UserName);
            if (existingUser != null)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Username already exists",
                    Data = null
                };
            }

            var existingEmail = await _userManager.FindByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Email already exists",
                    Data = null
                };
            }

            if(!Regex.IsMatch(request.Password, Constant.PasswordRegexPattern))
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Password is not valid",
                    Data = null
                };
            }

            var user = new User
            {
                UserName = request.UserName,
                Email = request.Email,
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                DateOfBirth = request.DateOfBirth,
                AvatarUrl = "",
                isDeleted = false,
                Point = 0,
                RankCustomerId = GetUnrankedRankCustomerIdAsync().Result,
                UserStatusId = GetPendingUserStatusIdAsync().Result
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Error creating User",
                    Data = null
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var activeCode = GenerateCode();

            var confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                RequiredDateTime = DateTime.Now,
                ExpiredDateTime = DateTime.Now.AddMinutes(5),
                ConfirmCode = activeCode,
                IsConfirm = false
            };

            await _confirmEmailRepository.AddAsync(confirmEmail);

            var emailSubject = "Activate your account";
            var emailBody = $"Please activate your account using this code: " + activeCode;

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            var userDTO = _userConverter.ConvertToDTO(user);

            return new BaseResponseModel<UserDTO>
            {
                Status = StatusCodes.Status201Created,
                Message = "Registration successful. Please check your email to confirm your account.",
                Data = userDTO
            };
        }

        #endregion

        #region AuthenticateRegistration method
        public async Task<BaseResponseModel<UserDTO>> AuthenticateRegistration(AuthenticateRegistrationRequestModel request)
        {

            var userStatusPendingId = await GetPendingUserStatusIdAsync();

            var userStatusActiveId = await GetActiveUserStatusIdAsync();

            var existingUser = await _userManager.FindByEmailAsync(request.Email);

            if (existingUser == null)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Error, user does not exist!",
                    Data = null
                };
            }

            if (existingUser.UserStatusId != userStatusPendingId)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Error, user is not in pending status, cannot activate this account!",
                    Data = null
                };
            }

            var lastConfirmCode = await GetLastConfirmCode(existingUser.Id);

            if (lastConfirmCode == null)
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Activation code does not existed, please request a new code!",
                    Data = null
                };
            }

            if (request.ConfirmCode == lastConfirmCode.ConfirmCode)
            {
                if (DateTime.Now < lastConfirmCode.ExpiredDateTime)
                {
                    existingUser.UserStatusId = userStatusActiveId;
                    await _userManager.UpdateAsync(existingUser);

                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status200OK,
                        Message = "Activate account successfully, welcome to our website!",
                        Data = _userConverter.ConvertToDTO(existingUser)
                    };
                }
                else
                {
                    return new BaseResponseModel<UserDTO>
                    {
                        Status = StatusCodes.Status400BadRequest,
                        Message = "Activation code has expired, please request a new code!",
                        Data = null
                    };
                }
            }
            else
            {
                return new BaseResponseModel<UserDTO>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Activation code does not match, please try again!",
                    Data = null
                };
            }

        }

        #endregion

        #region Forgot Password method
        public async Task<BaseResponseModel<object>> ForgotPassword(ForgotPasswordRequestModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Can not find user, please try again!",
                    Data = null
                };
            }

            var userStatusActiveId = await GetActiveUserStatusIdAsync();

            if (user.UserStatusId != userStatusActiveId)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "User can not reset password, please try again!",
                    Data = null
                };
            }

            var resetCode = GenerateCode();

            var confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                RequiredDateTime = DateTime.Now,
                ExpiredDateTime = DateTime.Now.AddMinutes(5),
                ConfirmCode = resetCode,
                IsConfirm = false
            };

            await _confirmEmailRepository.AddAsync(confirmEmail);

            var emailSubject = "Reset your account";
            var emailBody = $"Please using this code to reset your account password: " + resetCode;

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return new BaseResponseModel<object>
            {
                Status = StatusCodes.Status200OK,
                Message = "Please check your email to get reset password code!",
                Data = null
            };
        }
        #endregion

        #region Reset Password method
        public async Task<BaseResponseModel<object>> ResetPassword(ResetPasswordRequestModel request)
        {
            var user  = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) 
            { 
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Can not find user, please try again!",
                    Data = null
                }; 
            }

            var userStatusActiveId = await GetActiveUserStatusIdAsync();

            if(user.UserStatusId != userStatusActiveId)
            {
                return new BaseResponseModel<object> 
                { 
                    Status = StatusCodes.Status400BadRequest, 
                    Message = "User can not reset password, please try again",
                    Data= null
                };
            }

            var lastConfirmCode = await GetLastConfirmCode(user.Id);
            if (lastConfirmCode == null)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Can not find last confirm code, please try again!",
                    Data = null
                };
            }

            if(lastConfirmCode.ConfirmCode != request.ResetCode)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Reset code do not match, please try again!",
                    Data = null
                };
            }

            if (lastConfirmCode.ExpiredDateTime < DateTime.Now)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Reset code has expired, please request a new one!",
                    Data = null
                };
            }

            var newPassword = GenerateRandomPassword();
            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, newPassword);

            var emailSubject = "Reset password successfully";
            var emailBody = $"You have successfully reset your password. Here is your new password: {newPassword}";

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return new BaseResponseModel<object>
            {
                Status = StatusCodes.Status200OK,
                Message = "Successfully reset password, check your email for new password!",
                Data = null
            };
        }
        #endregion

        #region Resend Active Code method
        public async Task<BaseResponseModel<object>> ResendActiveCode(ResendActiveCodeRequestModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null) 
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found, please try again!",
                    Data = null
                };
            }

            var userStatusPendingId = await GetPendingUserStatusIdAsync();

            if (user.UserStatusId != userStatusPendingId)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "User is not in a pending status, activation is not allowed.",
                    Data = null
                };
            }

            var activeCode = GenerateCode();

            var confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                RequiredDateTime = DateTime.Now,
                ExpiredDateTime = DateTime.Now.AddMinutes(5),
                ConfirmCode = activeCode,
                IsConfirm = false
            };

            await _confirmEmailRepository.AddAsync(confirmEmail);

            var emailSubject = "Activate your account";
            var emailBody = $"Please activate your account using this code: " + activeCode;

            await _emailService.SendEmailAsync(user.Email, emailSubject, emailBody);

            return new BaseResponseModel<object>
            {
                Status = StatusCodes.Status201Created,
                Message = "Resend active code successful. Please check your email to confirm your account.",
                Data = null
            };
        }
        #endregion

        #region Change Password method
        public async Task<BaseResponseModel<object>> ChangePassword(ChangePasswordRequestModel request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status404NotFound,
                    Message = "User not found, please try again.",
                    Data = null
                };
            }

            if (!Regex.IsMatch(request.NewPassword, Constant.PasswordRegexPattern))
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "New Password is not valid, please try again.",
                    Data = null
                };
            }

            var result = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);

            if (result.Succeeded)
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status200OK,
                    Message = "Password changed successfully.",
                    Data = null
                };
            }

            if (result.Errors.Any(e => e.Code == "PasswordMismatch"))
            {
                return new BaseResponseModel<object>
                {
                    Status = StatusCodes.Status400BadRequest,
                    Message = "Old password is incorrect, please try again.",
                    Data = null
                };
            }

            return new BaseResponseModel<object>
            {
                Status = StatusCodes.Status400BadRequest,
                Message = "Password change failed, please try again.",
                Data = null
            };
        }
        #endregion
    }
}
