using Microsoft.AspNetCore.Mvc;
using Moq;
using PetAdoptionSystem.Controllers;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PetAdoptionSystem.Tests.Controllers
{
    public class PetsControllerTests
    {
        private readonly Mock<IPetService> _mockPetService;
        private readonly Mock<IAdoptionRequestService> _mockAdoptionRequestService;
        private readonly PetsController _controller;

        public PetsControllerTests()
        {
            _mockPetService = new Mock<IPetService>();
            _mockAdoptionRequestService = new Mock<IAdoptionRequestService>();

            _controller = new PetsController(_mockPetService.Object, _mockAdoptionRequestService.Object);
        }

        [Fact]
        public async Task GetAllPets_ReturnsOkResultWithPets()
        {
            // Arrange
            var pets = new List<Pet>
            {
                new Pet { Id = 1, Name = "Max" },
                new Pet { Id = 2, Name = "Bella" }
            };

            var adoptionRequests = new List<AdoptionRequest>
            {
                new AdoptionRequest { PetId = 1, Status = "Approved", AdopterId = 10, RequestedAt = System.DateTime.UtcNow }
            };

            _mockPetService.Setup(s => s.GetAllPetsAsync()).ReturnsAsync(pets);
            _mockAdoptionRequestService.Setup(s => s.GetAllAsync()).ReturnsAsync(adoptionRequests);

            // Act
            var result = await _controller.GetAllPets();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnPets = Assert.IsAssignableFrom<List<Pet>>(okResult.Value);
            Assert.Equal(2, returnPets.Count);

            // Additional check that pet with Id=1 is marked adopted
            var adoptedPet = returnPets.First(p => p.Id == 1);
            Assert.True(adoptedPet.IsAdopted);
            Assert.Equal(10, adoptedPet.AdopterId);
        }
    }
}
