using Microsoft.AspNetCore.Mvc;
using Moq;
using PetAdoptionSystem.Controllers;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace PetAdoptionSystem.Tests.Controllers
{
    public class AdoptersControllerTests
    {
        private readonly Mock<IAdopterService> _mockAdopterService;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly AdoptersController _controller;

        public AdoptersControllerTests()
        {
            _mockAdopterService = new Mock<IAdopterService>();

            var userStore = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStore.Object, null, null, null, null, null, null, null, null);

            _controller = new AdoptersController(_mockAdopterService.Object, _mockUserManager.Object);

            // Setup fake user with Admin role for [Authorize(Roles = "Admin")]
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "admin-id"),
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        [Fact]
        public async Task GetAllAdopters_ReturnsOkWithAdopters_WhenAdmin()
        {
            // Arrange
            var adopters = new List<Adopter>
            {
                new Adopter { Id = 1, Email = "a@example.com" },
                new Adopter { Id = 2, Email = "b@example.com" }
            };
            _mockAdopterService.Setup(s => s.GetAllAdoptersAsync()).ReturnsAsync(adopters);

            // Act
            var result = await _controller.GetAllAdopters();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnAdopters = Assert.IsAssignableFrom<IEnumerable<Adopter>>(okResult.Value);
            Assert.Equal(2, ((List<Adopter>)returnAdopters).Count);
        }
    }
}
