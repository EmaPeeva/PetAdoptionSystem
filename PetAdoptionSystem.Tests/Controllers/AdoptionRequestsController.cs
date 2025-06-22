using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PetAdoptionSystem.Controllers;
using PetAdoptionSystem.DTOs;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using Xunit;

namespace PetAdoptionSystem.Tests.Controllers
{
    public class AdoptionRequestsControllerTests
    {
        private readonly Mock<IAdoptionRequestService> _mockService;
        private readonly Mock<IAdopterService> _mockAdopterService;
        private readonly Mock<IPetService> _mockPetService;
        private readonly AdoptionRequestsController _controller;

        public AdoptionRequestsControllerTests()
        {
            _mockService = new Mock<IAdoptionRequestService>();
            _mockAdopterService = new Mock<IAdopterService>();
            _mockPetService = new Mock<IPetService>();

            _controller = new AdoptionRequestsController(
                _mockService.Object,
                _mockAdopterService.Object,
                _mockPetService.Object);

            // Setup a fake authenticated user with claims (email and roles)
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testuser@example.com"),
                new Claim(ClaimTypes.Role, "User"),
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetMine_ReturnsUnauthorized_WhenNoUser()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // no identity

            // Act
            var result = await _controller.GetMine();

            // Assert
            Assert.IsType<UnauthorizedResult>(result.Result);
        }

        [Fact]
        public async Task GetMine_ReturnsRequests_WhenAdopterExists()
        {
            // Arrange
            var adopter = new Adopter { Id = 1, Email = "testuser@example.com" };
            var requests = new List<AdoptionRequest>
            {
                new AdoptionRequest { Id = 1, AdopterId = 1, PetId = 10, Status = "Pending" }
            };

            _mockAdopterService.Setup(s => s.GetAdopterByEmailAsync(adopter.Email)).ReturnsAsync(adopter);
            _mockService.Setup(s => s.GetByAdopterIdAsync(adopter.Id)).ReturnsAsync(requests);

            // Act
            var result = await _controller.GetMine();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedRequests = Assert.IsAssignableFrom<IEnumerable<AdoptionRequest>>(okResult.Value);
            Assert.Single(returnedRequests);
        }

        [Fact]
        public async Task Create_ReturnsUnauthorized_WhenNoUser()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // no identity

            // Act
            var result = await _controller.Create(new AdoptionRequestCreateDTO { PetId = 1 });

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task Create_ReturnsConflict_WhenDuplicateRequestExists()
        {
            // Arrange
            var adopter = new Adopter { Id = 1, Email = "testuser@example.com" };
            var dto = new AdoptionRequestCreateDTO { PetId = 10 };

            _mockAdopterService.Setup(s => s.GetAdopterByEmailAsync(adopter.Email)).ReturnsAsync(adopter);
            _mockService.Setup(s => s.GetByAdopterIdAsync(adopter.Id))
                .ReturnsAsync(new List<AdoptionRequest>
                {
                    new AdoptionRequest { PetId = 10, AdopterId = adopter.Id }
                });

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            Assert.Equal("You have already sent a request for this pet.", conflictResult.Value);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenRequestIsNew()
        {
            // Arrange
            var adopter = new Adopter { Id = 1, Email = "testuser@example.com" };
            var dto = new AdoptionRequestCreateDTO { PetId = 10 };
            var request = new AdoptionRequest { Id = 5, PetId = dto.PetId, AdopterId = adopter.Id, Status = "Pending" };

            _mockAdopterService.Setup(s => s.GetAdopterByEmailAsync(adopter.Email)).ReturnsAsync(adopter);
            _mockService.Setup(s => s.GetByAdopterIdAsync(adopter.Id)).ReturnsAsync(new List<AdoptionRequest>());
            _mockService.Setup(s => s.AddAsync(It.IsAny<AdoptionRequest>())).Returns(Task.CompletedTask);
            _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(request);

            // Act
            var result = await _controller.Create(dto);

            // Assert
            var createdAtResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnValue = Assert.IsType<AdoptionRequest>(createdAtResult.Value);
            Assert.Equal(request.Id, returnValue.Id);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsNotFound_WhenRequestMissing()
        {
            // Arrange
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((AdoptionRequest?)null);

            // Act
            var result = await _controller.UpdateStatus(1, new AdoptionRequestUpdateStatusDTO { Status = "Approved" });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task UpdateStatus_ReturnsBadRequest_WhenPetAdoptionFails()
        {
            // Arrange
            var request = new AdoptionRequest { Id = 1, PetId = 10, AdopterId = 2, Status = "Pending" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(request);
            _mockService.Setup(s => s.UpdateAsync(It.IsAny<AdoptionRequest>())).Returns(Task.CompletedTask);
            _mockPetService.Setup(s => s.AdoptPetAsync(request.PetId, request.AdopterId)).ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateStatus(1, new AdoptionRequestUpdateStatusDTO { Status = "Approved" });

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to adopt pet. It might already be adopted or invalid.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateStatus_ApprovesRequestAndRejectsOthers()
        {
            var request = new AdoptionRequest { Id = 1, PetId = 10, AdopterId = 2, Status = "Pending" };
            var otherPendingRequest = new AdoptionRequest { Id = 2, PetId = 10, AdopterId = 3, Status = "Pending" };
            var allRequests = new List<AdoptionRequest> { request, otherPendingRequest };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(request);
            _mockService.Setup(s => s.UpdateAsync(request)).Returns(Task.CompletedTask);
            _mockPetService.Setup(s => s.AdoptPetAsync(request.PetId, request.AdopterId)).ReturnsAsync(true);
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(allRequests);
            _mockService.Setup(s => s.UpdateAsync(otherPendingRequest)).Returns(Task.CompletedTask);

            var result = await _controller.UpdateStatus(1, new AdoptionRequestUpdateStatusDTO { Status = "Approved" });

            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = okResult.Value;
            var messageProperty = value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            var message = messageProperty.GetValue(value) as string;
            Assert.Contains("Email sent to adopter", message);
            Assert.Equal("Rejected", otherPendingRequest.Status);
            _mockService.Verify(s => s.UpdateAsync(otherPendingRequest), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsUnauthorized_WhenNoUser()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // no identity

            // Mock an existing adoption request so NotFound is not triggered
            _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(new AdoptionRequest { Id = 1, AdopterId = 1 });

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }


        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleted()
        {
            // Arrange
            var adopter = new Adopter { Id = 1, Email = "testuser@example.com" };
            var request = new AdoptionRequest { Id = 1, AdopterId = adopter.Id };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(request);
            _mockAdopterService.Setup(s => s.GetAdopterByEmailAsync(adopter.Email)).ReturnsAsync(adopter);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        }
    }
}
