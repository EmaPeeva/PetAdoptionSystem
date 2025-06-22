using Moq;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PetAdoptionSystem.Tests.Services
{
    public class PetServiceTests
    {
        private readonly Mock<IPetRepository> _mockPetRepository;
        private readonly Mock<IAdopterRepository> _mockAdopterRepository;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly PetService _petService;

        public PetServiceTests()
        {
            _mockPetRepository = new Mock<IPetRepository>();
            _mockAdopterRepository = new Mock<IAdopterRepository>();
            _mockNotificationService = new Mock<INotificationService>();

            _petService = new PetService(
                _mockPetRepository.Object,
                _mockAdopterRepository.Object,
                _mockNotificationService.Object
            );
        }

        [Fact]
        public async Task GetAllPetsAsync_ReturnsListOfPets()
        {
            // Arrange
            var expectedPets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max", IsAdopted = false },
                new Pet { Id = 2, Name = "Bella", IsAdopted = true }
            };

            _mockPetRepository.Setup(repo => repo.GetAllAsync())
                              .ReturnsAsync(expectedPets);

            // Act
            var result = await _petService.GetAllPetsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }


        [Fact]
        public async Task AddPetAsync_CallsRepositoryAddAsync()
        {
            // Arrange
            var newPet = new Pet { Name = "Rocky", IsAdopted = false };

            // Act
            await _petService.AddPetAsync(newPet);

            // Assert
            _mockPetRepository.Verify(r => r.AddAsync(newPet), Times.Once);
        }

        [Fact]
        public async Task DeletePetAsync_Throws_WhenPetNotFound()
        {
            // Arrange
            int petId = 99;
            _mockPetRepository.Setup(r => r.GetByIdAsync(petId)).ReturnsAsync((Pet?)null);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _petService.DeletePetAsync(petId));
            Assert.Contains("not found", ex.Message);
        }

        [Fact]
        public async Task AdoptPetAsync_SuccessfullyAdoptsPet()
        {
            // Arrange
            int petId = 1;
            int adopterId = 10;
            var pet = new Pet { Id = petId, Name = "Luna", IsAdopted = false };
            var adopter = new Adopter { Id = adopterId, Email = "adopter@example.com" };

            _mockPetRepository.Setup(r => r.GetByIdAsync(petId)).ReturnsAsync(pet);
            _mockAdopterRepository.Setup(r => r.GetByIdAsync(adopterId)).ReturnsAsync(adopter);
            _mockPetRepository.Setup(r => r.UpdateAsync(It.IsAny<Pet>())).Returns(Task.CompletedTask);

            // Act
            var result = await _petService.AdoptPetAsync(petId, adopterId);

            // Assert
            Assert.True(result);
            Assert.True(pet.IsAdopted);
            Assert.Equal(adopterId, pet.AdopterId);
            _mockNotificationService.Verify(n => n.SendAdoptionConfirmationEmailAsync(adopter.Email, pet.Name), Times.Once);
        }



    }
}
