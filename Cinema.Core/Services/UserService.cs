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

namespace Cinema.Core.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;
        private readonly Identity.ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly IRepository<RankCustomer> _rankCustomerRepository;
        private readonly IRepository<UserStatus> _userStatusRepository;
        private readonly UserConverter _userConverter;
        private readonly IRepository<ConfirmEmail> _confirmEmailRepository;

        public UserService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            Identity.ITokenService tokenService,
            RoleManager<Role> roleManager,
            IEmailService emailService,
            IRepository<RankCustomer> rankCustomerRepository,
            IRepository<UserStatus> userStatusRepository,
            UserConverter userConverter,
            IRepository<ConfirmEmail> confirmEmailRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _roleManager = roleManager;
            _emailService = emailService;
            _rankCustomerRepository = rankCustomerRepository;
            _userStatusRepository = userStatusRepository;
            _userConverter = userConverter;
            _confirmEmailRepository = confirmEmailRepository;
        }

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
                    Message = string.Join("; ", result.Errors.Select(e => e.Description)),
                    Data = null
                };
            }

            await _userManager.AddToRoleAsync(user, "User");

            var activeCode = GenerateActiveCode();

            var confirmEmail = new ConfirmEmail
            {
                UserId = user.Id,
                RequiredDateTime = DateTime.UtcNow,
                ExpiredDateTime = DateTime.UtcNow.AddMinutes(5),
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

        private string GenerateActiveCode()
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

    }
}
