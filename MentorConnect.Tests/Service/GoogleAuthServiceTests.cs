using System.Security.Claims;
using MentorConnect.Data.Entities;
using MentorConnect.Web.Services;
using MentorConnect.Web.Services.Results;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace MentorConnect.Tests.Service
{
    [TestFixture]
    public class GoogleAuthServiceTests
    {
        private Mock<IConfiguration> _mockConfiguration;
        private Mock<ILogger<GoogleAuthService>> _mockLogger;
        private Mock<SignInManager<ApplicationUser>> _mockSignInManager;
        private Mock<UserManager<ApplicationUser>> _mockUserManager;
        private GoogleAuthService _googleAuthService;

        [SetUp]
        public void SetUp()
        {
            _mockConfiguration = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<GoogleAuthService>>();
    
            var userStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(
                userStoreMock, null, null, null, null, null, null, null, null);
    
            var contextAccessorMock = Mock.Of<IHttpContextAccessor>();
            var userClaimsPrincipalFactoryMock =
                Mock.Of<IUserClaimsPrincipalFactory<ApplicationUser>>();
            _mockSignInManager = new Mock<SignInManager<ApplicationUser>>(
                _mockUserManager.Object, contextAccessorMock, userClaimsPrincipalFactoryMock, null, null, null, null);

            _googleAuthService = new GoogleAuthService(
                _mockConfiguration.Object,
                _mockLogger.Object,
                _mockSignInManager.Object,
                _mockUserManager.Object);
        }

        [Test]
        public async Task HandleGoogleResponseAsync_AuthenticationFailed_ReturnsFailure()
        {
            // Arrange
            AuthenticateResult authenticateResult = AuthenticateResult.Fail("Authentication failed");

            // Act
            GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorMessage, Is.EqualTo("Authentication failed"));
            });
        }

        [Test]
        public async Task HandleGoogleResponseAsync_EmailClaimNotFound_ReturnsFailure()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            AuthenticateResult authenticateResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Google"));

            // Act
            GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.ErrorMessage, Is.EqualTo("Email claim not found"));
            });
        }

        [Test]
        public async Task HandleGoogleResponseAsync_UserNotFound_CreatesNewUser()
        {
            // Arrange
            const string email = "test@example.com";
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, email) }));
            AuthenticateResult authenticateResult = 
                AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Google"));

            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync((ApplicationUser)null);
            _mockUserManager.Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            // Act
            GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.NeedsPassword, Is.True);
            });
            _mockUserManager.Verify(um => um.CreateAsync(It.IsAny<ApplicationUser>()), Times.Once);
        }

        [Test]
        public async Task HandleGoogleResponseAsync_UserWithoutPassword_ReturnsPasswordNeeded()
        {
            // Arrange
            const string email = "test@example.com";
            var user = new ApplicationUser { Email = email };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, email) }));
            AuthenticateResult authenticateResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Google"));

            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);

            // Act
            GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.False);
                Assert.That(result.NeedsPassword, Is.True);
            });
        }

        [Test]
        public async Task HandleGoogleResponseAsync_UserWithPassword_ReturnsSuccess()
        {
            // Arrange
            const string email = "test@example.com";
            var user = new ApplicationUser { Email = email, PasswordHash = "hashedPassword" };
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new Claim[] { new Claim(ClaimTypes.Email, email) }));
            AuthenticateResult authenticateResult = AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, "Google"));

            _mockUserManager.Setup(um => um.FindByEmailAsync(email)).ReturnsAsync(user);
            _mockConfiguration.Setup(c => c["Jwt:IssuerSigningKey"]).Returns("testKey");
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("testIssuer");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("testAudience");

            // Act
            GoogleAuthResult result = await _googleAuthService.HandleGoogleResponseAsync(authenticateResult);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Token, Is.Not.Null);
            });
        }
    }
}