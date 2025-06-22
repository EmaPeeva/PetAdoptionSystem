using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace PetAdoptionSystem.Tests.Repositories
{
    public class PetRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsPetToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new PetRepository(context);
            var newPet = new Pet
            {
                Name = "Simba",
                Species = "Cat",
                Age = 2,
                PhotoUrl = "https://example.com/photo.jpg" // Required property
            };

            // Act
            await repository.AddAsync(newPet);
            var pets = await repository.GetAllAsync();

            // Assert
            Assert.Single(pets);
            Assert.Equal("Simba", pets.First().Name);
        }
    }
}
