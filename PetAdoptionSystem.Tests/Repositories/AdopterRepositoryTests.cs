using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Threading.Tasks;
using Xunit;
using System.Linq;

namespace PetAdoptionSystem.Tests.Repositories
{
    public class AdopterRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb_Adopter")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsAdopterToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new AdopterRepository(context);
            var newAdopter = new Adopter
            {
                Email = "newadopter@example.com",
                FullName = "John Doe"
                // Add other required properties if any
            };

            // Act
            await repository.AddAsync(newAdopter);
            var adopters = await repository.GetAllAsync();

            // Assert
            Assert.Single(adopters);
            Assert.Equal("newadopter@example.com", adopters.First().Email);
        }
    }
}
