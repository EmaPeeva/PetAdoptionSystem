using Moq;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using PetAdoptionSystem.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PetAdoptionSystem.Tests.Services
{
    public class AdopterServiceTests
    {
        private readonly Mock<IAdopterRepository> _mockAdopterRepository;
        private readonly AdopterService _adopterService;

        public AdopterServiceTests()
        {
            _mockAdopterRepository = new Mock<IAdopterRepository>();
            _adopterService = new AdopterService(_mockAdopterRepository.Object);
        }

        [Fact]
        public async Task AddAdopterAsync_AddsNewAdopter_WhenEmailIsUnique()
        {
            // Arrange
            var newAdopter = new Adopter { Email = "test@example.com" };
            _mockAdopterRepository.Setup(r => r.GetByEmailAsync(newAdopter.Email))
                                  .ReturnsAsync((Adopter?)null);
            _mockAdopterRepository.Setup(r => r.AddAsync(newAdopter))
                                  .ReturnsAsync(newAdopter);

            // Act
            var result = await _adopterService.AddAdopterAsync(newAdopter);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(newAdopter.Email, result.Email);
            _mockAdopterRepository.Verify(r => r.AddAsync(newAdopter), Times.Once);
        }

        [Fact]
        public async Task AddAdopterAsync_ThrowsArgumentException_WhenEmailIsEmpty()
        {
            // Arrange
            var adopter = new Adopter { Email = "" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _adopterService.AddAdopterAsync(adopter));
        }

        [Fact]
        public async Task AddAdopterAsync_ThrowsInvalidOperationException_WhenEmailAlreadyExists()
        {
            // Arrange
            var adopter = new Adopter { Email = "exists@example.com" };
            _mockAdopterRepository.Setup(r => r.GetByEmailAsync(adopter.Email))
                                  .ReturnsAsync(new Adopter { Email = adopter.Email });

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => _adopterService.AddAdopterAsync(adopter));
        }
    }
}
