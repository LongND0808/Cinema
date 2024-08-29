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
using Cinema.Core.DTOs;

namespace Cinema.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<Core.Identity.ITokenService> _tokenServiceMock;
        private readonly Mock<RoleManager<Role>> _roleManagerMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IRepository<RankCustomer>> _rankCustomerRepositoryMock;
        private readonly Mock<IRepository<UserStatus>> _userStatusRepositoryMock;
        private readonly Mock<UserConverter> _userConverterMock;
        private readonly Mock<IRepository<ConfirmEmail>> _confirmEmailRepositoryMock;

        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userManagerMock = CreateMockUserManager();
            _signInManagerMock = CreateMockSignInManager();
            _tokenServiceMock = new Mock<Core.Identity.ITokenService>();

            _emailServiceMock = new Mock<IEmailService>();
            _emailServiceMock.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            _rankCustomerRepositoryMock = new Mock<IRepository<RankCustomer>>();
            _userStatusRepositoryMock = new Mock<IRepository<UserStatus>>();
            _userConverterMock = new Mock<UserConverter>();
            _confirmEmailRepositoryMock = new Mock<IRepository<ConfirmEmail>>();

            _userService = new UserService(
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _tokenServiceMock.Object,
                _emailServiceMock.Object,
                _rankCustomerRepositoryMock.Object,
                _userStatusRepositoryMock.Object,
                _userConverterMock.Object,
                _confirmEmailRepositoryMock.Object
            );
        }

        private Mock<UserManager<User>> CreateMockUserManager()
        {
            var store = new Mock<IUserStore<User>>();
            return new Mock<UserManager<User>>(store.Object, null, null, null, null, null, null, null, null);
        }

        private Mock<SignInManager<User>> CreateMockSignInManager()
        {
            return new Mock<SignInManager<User>>(
                _userManagerMock.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<User>>().Object,
                null,
                null,
                null,
                null
            );
        }

        [Fact]
        public async Task Login_UserNotFound_ReturnsNotFound()
        {
            var request = new LoginRequestModel { UserName = "nonexistent", Password = "password" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            var result = await _userService.Login(request);

            Assert.Equal(StatusCodes.Status404NotFound, result.Status);
            Assert.Equal("User does not exist", result.Message);
        }

        [Fact]
        public async Task Login_InvalidPassword_ReturnsUnauthorized()
        {
            var user = new User { UserName = "user", PasswordHash = "password" };
            var request = new LoginRequestModel { UserName = "user", Password = "wrongpassword" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            var result = await _userService.Login(request);

            Assert.Equal(StatusCodes.Status401Unauthorized, result.Status);
            Assert.Equal("Invalid credentials", result.Message);
        }

        [Fact]
        public async Task Login_Success_ReturnsOk()
        {
            var user = new User { UserName = "user", PasswordHash = "password" };
            var request = new LoginRequestModel { UserName = "user", Password = "password" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(sm => sm.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);
            _tokenServiceMock.Setup(ts => ts.CreateAccessTokenAsync(It.IsAny<User>())).ReturnsAsync("access_token");
            _tokenServiceMock.Setup(ts => ts.CreateRefreshTokenAsync(It.IsAny<User>())).ReturnsAsync("refresh_token");

            var result = await _userService.Login(request);

            Assert.Equal(StatusCodes.Status200OK, result.Status);
            Assert.Equal("Login success", result.Message);
            Assert.Equal("access_token", result.Data.AccessToken);
            Assert.Equal("refresh_token", result.Data.RefreshToken);
        }

        [Fact]
        public async Task Register_UsernameExists_ReturnsBadRequest()
        {
            var request = new RegisterRequestModel { UserName = "existinguser", Email = "email@example.com" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var result = await _userService.Register(request);

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Username already exists", result.Message);
        }

        [Fact]
        public async Task Register_EmailExists_ReturnsBadRequest()
        {
            var request = new RegisterRequestModel { UserName = "newuser", Email = "existingemail@example.com" };
            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            var result = await _userService.Register(request);

            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Email already exists", result.Message);
        }

        [Fact]
        public async Task Register_Success_ReturnsCreated()
        {
            // Arrange
            var request = new RegisterRequestModel
            {
                UserName = "newuser",
                Email = "newemail@example.com",
                Password = "password",
                FullName = "New User",
                PhoneNumber = "1234567890",
                Gender = Enumerate.Gender.Male,
                DateOfBirth = DateTime.UtcNow.AddYears(-30)
            };

            _userManagerMock.Setup(um => um.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(um => um.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            _rankCustomerRepositoryMock.Setup(rcr => rcr.GetOneAsyncUntracked(It.IsAny<Expression<Func<RankCustomer, bool>>>(),
                null,
                It.IsAny<Expression<Func<RankCustomer, Guid>>>())).ReturnsAsync(Guid.NewGuid());

            _userStatusRepositoryMock.Setup(us => us.GetOneAsyncUntracked(It.IsAny<Expression<Func<UserStatus, bool>>>(),
                null,
                It.IsAny<Expression<Func<UserStatus, Guid>>>())).ReturnsAsync(Guid.NewGuid());

            _userConverterMock.Setup(uc => uc.ConvertToDTO(It.IsAny<User>())).Returns(new UserDTO());

            _emailServiceMock.Setup(es => es.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _userService.Register(request);

            // Assert
            Assert.Equal(StatusCodes.Status201Created, result.Status);
            Assert.Equal("Registration successful. Please check your email to confirm your account.", result.Message);
        }


        [Fact]
        public async Task AuthenticateRegistration_UserDoesNotExist_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticateRegistrationRequestModel { Email = "nonexistent@example.com", ConfirmCode = "123456" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act
            var result = await _userService.AuthenticateRegistration(request);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Error, user does not exist!", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AuthenticateRegistration_ActivationCodeExpired_ReturnsBadRequest()
        {
            // Arrange
            var user = new User { Email = "existinguser@example.com", UserStatusId = Guid.NewGuid() };
            var request = new AuthenticateRegistrationRequestModel { Email = user.Email, ConfirmCode = "123456" };

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userStatusRepositoryMock.Setup(us => us.GetOneAsyncUntracked(It.IsAny<Expression<Func<UserStatus, bool>>>(), null,
                It.IsAny<Expression<Func<UserStatus, Guid>>>())).ReturnsAsync(user.UserStatusId);
            _confirmEmailRepositoryMock.Setup(ce => ce.GetOneAsyncUntracked<ConfirmEmail>(It.IsAny<Expression<Func<ConfirmEmail, bool>>>(), It.IsAny<Expression<Func<IQueryable<ConfirmEmail>, IOrderedQueryable<ConfirmEmail>>>?>(), null)).ReturnsAsync(new ConfirmEmail
                {
                    ConfirmCode = "123456",
                    ExpiredDateTime = DateTime.Now.AddMinutes(-1)  // Đã hết hạn
                });

            // Act
            var result = await _userService.AuthenticateRegistration(request);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, result.Status);
            Assert.Equal("Activation code has expired, please request a new code!", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public async Task AuthenticateRegistration_SuccessfulActivation_ReturnsOk()
        {
            // Arrange
            var user = new User { Email = "existinguser@example.com", UserStatusId = Guid.NewGuid() };
            var request = new AuthenticateRegistrationRequestModel { Email = user.Email, ConfirmCode = "123456" };

            var userStatusPending = user.UserStatusId;
            var userStatusActive = Guid.NewGuid();

            _userManagerMock.Setup(um => um.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userStatusRepositoryMock.SetupSequence(us => us.GetOneAsyncUntracked(
                    It.IsAny<Expression<Func<UserStatus, bool>>>(),
                    null,
                    It.IsAny<Expression<Func<UserStatus, Guid>>>()
                ))
                .ReturnsAsync(userStatusPending) 
                .ReturnsAsync(userStatusActive); 

            _confirmEmailRepositoryMock.Setup(ce => ce.GetOneAsyncUntracked<ConfirmEmail>(It.IsAny<Expression<Func<ConfirmEmail, bool>>>(), It.IsAny<Expression<Func<IQueryable<ConfirmEmail>, IOrderedQueryable<ConfirmEmail>>>?>(), null)).ReturnsAsync(new ConfirmEmail
                {
                    ConfirmCode = "123456",
                    ExpiredDateTime = DateTime.Now.AddMinutes(10)  
                });
            _userManagerMock.Setup(um => um.UpdateAsync(It.IsAny<User>())).ReturnsAsync(IdentityResult.Success);
            _userConverterMock.Setup(uc => uc.ConvertToDTO(It.IsAny<User>())).Returns(new UserDTO());

            // Act
            var result = await _userService.AuthenticateRegistration(request);

            // Assert
            Assert.Equal(StatusCodes.Status200OK, result.Status);
            Assert.Equal("Activate account successfully, welcome to our website!", result.Message);
            Assert.NotNull(result.Data);
        }
    }
}
