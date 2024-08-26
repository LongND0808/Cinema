using Cinema.Core.IService;
using Cinema.Core.RequestModel;
using Cinema.Core.ResponseModel;
using Cinema.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Configuration;
using Cinema.Core.Services;
using IdentityModel;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using BaseInsightDotNet.Commons.Enums;
using Cinema.Core.InterfaceRepository;
using System.Linq.Expressions;
using Cinema.Core.Converters;
using Cinema.Core.Email;

namespace Cinema.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<Core.Identity.ITokenService> _tokenServiceMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<EmailService> _emailServiceMock;
        private readonly Mock<IRepository<RankCustomer>> _rankCustomerRepositoryMock;
        private readonly Mock<IRepository<UserStatus>> _userStatusRepositoryMock;
        private readonly Mock<IRepository<ConfirmEmail>> _confirmEmailRepositoryMock;
        private readonly Mock<UserConverter> _userConverterMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _tokenServiceMock = new Mock<Core.Identity.ITokenService>();
            _roleManagerMock = new Mock<RoleManager<Role>>(
                Mock.Of<IRoleStore<Role>>(), null, null, null, null);
            _emailServiceMock = new Mock<EmailService>();
            _rankCustomerRepositoryMock = new Mock<IRepository<RankCustomer>>();
            _userStatusRepositoryMock = new Mock<IRepository<UserStatus>>();
            _confirmEmailRepositoryMock = new Mock<IRepository<ConfirmEmail>>();
            _userConverterMock = new Mock<UserConverter>();

            _userService = new UserService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _roleManagerMock.Object,
                _emailServiceMock.Object,
                _rankCustomerRepositoryMock.Object,
                _userStatusRepositoryMock.Object,
                _userConverterMock.Object,
                _confirmEmailRepositoryMock.Object);
        }

        [Fact]
        public async Task Login_UserDoesNotExist_ShouldReturnNotFound()
        {
            var loginRequest = new LoginRequestModel
            {
                UserName = "nonexistentuser",
                Password = "Password123!"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginRequest.UserName))
                .ReturnsAsync((User)null);

            var result = await _userService.Login(loginRequest);

            Assert.Equal(StatusCodes.Status404NotFound, result.Status);
            Assert.Equal("User does not exist", result.Message);
        }

        [Fact]
        public async Task Login_InvalidPassword_ShouldReturnUnauthorized()
        {
            var testUser = new User { UserName = "testuser" };
            var loginRequest = new LoginRequestModel
            {
                UserName = "testuser",
                Password = "WrongPassword!"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginRequest.UserName))
                .ReturnsAsync(testUser);

            _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(testUser, loginRequest.Password, false))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _userService.Login(loginRequest);

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status);
            Assert.Equal("Invalid credentials", result.Message);
        }

        [Fact]
        public async Task Login_ValidCredentials_ShouldReturnTokens()
        {
            var testUser = new User
            {
                UserName = "testuser",
                Email = "testuser@example.com",
                FullName = "Test User"
            };

            var loginRequest = new LoginRequestModel
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var userRoles = new List<string> { "Admin", "User", "Employee" };

            _userManagerMock.Setup(um => um.FindByNameAsync(loginRequest.UserName))
                .ReturnsAsync(testUser);

            _userManagerMock.Setup(um => um.GetRolesAsync(testUser))
                .ReturnsAsync(userRoles);

            _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(testUser, loginRequest.Password, false))
                .ReturnsAsync(SignInResult.Success);

            _tokenServiceMock.Setup(ts => ts.CreateAccessTokenAsync(testUser))
                .ReturnsAsync("mockAccessToken");

            _tokenServiceMock.Setup(ts => ts.CreateRefreshTokenAsync(testUser))
                .ReturnsAsync("mockRefreshToken");

            var result = await _userService.Login(loginRequest);

            Assert.NotNull(result);
            Assert.Equal("mockAccessToken", result.Data.AccessToken);
            Assert.Equal("mockRefreshToken", result.Data.RefreshToken);
        }

        [Fact]
        public async Task Register_UsernameAlreadyExists_ShouldReturnBadRequest()
        {
            var registerRequest = new RegisterRequestModel
            {
                UserName = "existinguser",
                Email = "newemail@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerRequest.UserName))
                .ReturnsAsync(new User());

            var result = await _userService.Register(registerRequest);

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Username already exists", result.Message);
        }

        [Fact]
        public async Task Register_EmailAlreadyExists_ShouldReturnBadRequest()
        {
            var registerRequest = new RegisterRequestModel
            {
                UserName = "newuser",
                Email = "existingemail@example.com",
                Password = "Password123!"
            };

            _userManagerMock.Setup(um => um.FindByEmailAsync(registerRequest.Email))
                .ReturnsAsync(new User());

            var result = await _userService.Register(registerRequest);

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Email already exists", result.Message);
        }

        [Fact]
        public async Task Register_Success_ShouldReturnCreated()
        {
            var registerRequest = new RegisterRequestModel
            {
                UserName = "newuser",
                Email = "newuser@example.com",
                Password = "Password123!",
                FullName = "New User",
                PhoneNumber = "123456789",
                Gender = Enumerate.Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-20)
            };

            var mockRankCustomerId = Guid.NewGuid();
            var mockUserStatusId = Guid.NewGuid();

            var mockRankCustomer = new RankCustomer
            {
                Id = mockRankCustomerId,
                Name = "User"
            };

            var mockUserStatus = new UserStatus
            {
                Id = mockUserStatusId,
                Code = "Pending"
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(registerRequest.UserName))
                .ReturnsAsync((User)null);

            _userManagerMock.Setup(um => um.FindByEmailAsync(registerRequest.Email))
                .ReturnsAsync((User)null);

            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), registerRequest.Password))
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(um => um.AddToRoleAsync(It.IsAny<User>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _rankCustomerRepositoryMock.Setup(r => r.GetOneAsyncUntracked(
                It.IsAny<Expression<Func<RankCustomer, bool>>>(),
                It.IsAny<Expression<Func<RankCustomer, Guid>>>()))
                .ReturnsAsync(mockRankCustomer.Id);

            _userStatusRepositoryMock.Setup(r => r.GetOneAsyncUntracked(
                us => us.Code == "Pending",
                It.IsAny<Expression<Func<UserStatus, Guid>>>()))
                .ReturnsAsync(mockUserStatus.Id);

            var result = await _userService.Register(registerRequest);

            Assert.Equal(StatusCodes.Status201Created, result.Status);
            Assert.Equal("Registration successful. Please check your email to confirm your account.", result.Message);
            Assert.NotNull(result.Data);
        }


    }
}
