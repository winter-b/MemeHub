using Moq;
using System;
using WebApi.Interfaces;
using WebApi.Models;
using WebApi.Services;
using WebApi.Repositories;
using Xunit;

namespace WebApi.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAuthorizationRepository> _authorizationRepoMock;
        private readonly Mock<IHackingService> _hackingServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly string _salt = "testSalt";
        private readonly string _key = "RDFGNEM3OTNENzM1QjkzN0ExN0E0NDcxODIzQkY=";
        private readonly string _veriKey = "testVeriKey";

        public AuthServiceTests()
        {
            _authorizationRepoMock = new Mock<IAuthorizationRepository>();
            _hackingServiceMock = new Mock<IHackingService>();
            _emailServiceMock = new Mock<IEmailService>();
        }

        [Fact]
        public void Auth_ValidCredentials_ReturnsToken()
        {
            // Arrange
            var authService = new AuthorizationService(_authorizationRepoMock.Object, _emailServiceMock.Object, _hackingServiceMock.Object, _salt, _key, _veriKey);
            var request = new LoginRequest { UserName = "testUser", Password = "testPassword" };
            var expectedPassword = AuthorizationService.GenerateSaltedHash(System.Text.Encoding.ASCII.GetBytes(request.Password), System.Text.Encoding.ASCII.GetBytes(_salt));
            var userInfo = new UserInfo { UserName = request.UserName, Password = System.Text.Encoding.ASCII.GetString(expectedPassword), VerificationCode = ""};
            _authorizationRepoMock.Setup(x => x.GetUserInfoByName(request.UserName)).Returns(userInfo);

            // Act
            var result = authService.Auth(request);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Auth_InvalidCredentials_ReturnsEmptyString()
        {
            // Arrange
            var authService = new AuthorizationService(_authorizationRepoMock.Object, _emailServiceMock.Object, _hackingServiceMock.Object, _salt, _key, _veriKey);
            var request = new LoginRequest { UserName = "testUser", Password = "testPassword" };
            var userInfo = new UserInfo { UserName = request.UserName, Password = "incorrectPassword", VerificationCode = "" };
            var securePassword = AuthorizationService.GenerateSaltedHash(System.Text.Encoding.ASCII.GetBytes(request.Password), System.Text.Encoding.ASCII.GetBytes(_salt));
            _authorizationRepoMock.Setup(x => x.GetUserInfoByName(request.UserName)).Returns(userInfo);

            // Act
            var result = authService.Auth(request);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void Register_NewUser_ReturnsSuccessMessage()
        {
            // Arrange
            var authService = new AuthorizationService(_authorizationRepoMock.Object, _emailServiceMock.Object, _hackingServiceMock.Object, _salt, _key, _veriKey);
            var request = new RegisterRequest { Username = "testUser", Password = "testPassword", Email = "test@test.com" };
            _authorizationRepoMock.Setup(x => x.UsernameExists(request.Username)).Returns(false);

            // Act
            var result = authService.Register(request);

            // Assert
            Assert.Equal("Check mail ;)", result);
        }


        [Fact]
        public void Register_ExistingUser_ReturnsErrorMessage()
        {
            // Arrange
            var authService = new AuthorizationService(_authorizationRepoMock.Object, _emailServiceMock.Object, _hackingServiceMock.Object, _salt, _key, _veriKey);
            var request = new RegisterRequest { Username = "testUser", Password = "testPassword", Email = "test@test.com" };
            _authorizationRepoMock.Setup(x => x.UsernameExists(request.Username)).Returns(true);
            // Act
            var result = authService.Register(request);

            // Assert
            Assert.Equal("Username already exists!", result);
        }

    }
}
