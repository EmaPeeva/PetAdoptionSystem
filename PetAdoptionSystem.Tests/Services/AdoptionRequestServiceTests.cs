using Moq;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PetAdoptionSystem.Tests.Services
{
    public class AdoptionRequestServiceTests
    {
        private readonly Mock<IAdoptionRequestRepository> _mockRepository;
        private readonly AdoptionRequestService _service;

        public AdoptionRequestServiceTests()
        {
            _mockRepository = new Mock<IAdoptionRequestRepository>();
            _service = new AdoptionRequestService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllRequests()
        {
            // Arrange
            var expectedRequests = new List<AdoptionRequest>
            {
                new AdoptionRequest { Id = 1 },
                new AdoptionRequest { Id = 2 }
            };

            _mockRepository.Setup(r => r.GetAllAsync())
                           .ReturnsAsync(expectedRequests);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.Equal(expectedRequests.Count, ((List<AdoptionRequest>)result).Count);
        }

        [Fact]
        public async Task GetByAdopterIdAsync_ReturnsRequestsForAdopter()
        {
            // Arrange
            int adopterId = 5;
            var expectedRequests = new List<AdoptionRequest>
            {
                new AdoptionRequest { Id = 1, AdopterId = adopterId },
                new AdoptionRequest { Id = 2, AdopterId = adopterId }
            };

            _mockRepository.Setup(r => r.GetByAdopterIdAsync(adopterId))
                           .ReturnsAsync(expectedRequests);

            // Act
            var result = await _service.GetByAdopterIdAsync(adopterId);

            // Assert
            Assert.All(result, r => Assert.Equal(adopterId, r.AdopterId));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsRequest()
        {
            // Arrange
            int requestId = 3;
            var expectedRequest = new AdoptionRequest { Id = requestId };

            _mockRepository.Setup(r => r.GetByIdAsync(requestId))
                           .ReturnsAsync(expectedRequest);

            // Act
            var result = await _service.GetByIdAsync(requestId);

            // Assert
            Assert.Equal(requestId, result.Id);
        }

        [Fact]
        public async Task AddAsync_CallsRepositoryAddAsync()
        {
            // Arrange
            var newRequest = new AdoptionRequest();

            // Act
            await _service.AddAsync(newRequest);

            // Assert
            _mockRepository.Verify(r => r.AddAsync(newRequest), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_CallsRepositoryUpdateAsync()
        {
            // Arrange
            var existingRequest = new AdoptionRequest { Id = 1 };

            // Act
            await _service.UpdateAsync(existingRequest);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(existingRequest), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_CallsRepositoryDeleteAsync()
        {
            // Arrange
            int requestId = 10;

            // Act
            await _service.DeleteAsync(requestId);

            // Assert
            _mockRepository.Verify(r => r.DeleteAsync(requestId), Times.Once);
        }
    }
}
