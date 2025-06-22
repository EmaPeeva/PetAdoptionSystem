using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace PetAdoptionSystem.Tests.Repositories
{
    public class AdoptionRequestRepositoryTests
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb_AdoptionRequest")
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task AddAsync_AddsAdoptionRequest()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter = new Adopter { Email = "test@a.com", FullName = "Test Adopter" };
            var pet = new Pet
            {
                Name = "Test Pet",
                Species = "Dog",
                PhotoUrl = "http://example.com/photo.jpg"
            };

            context.Adopters.Add(adopter);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            var initialCount = await context.AdoptionRequests.CountAsync();

            var request = new AdoptionRequest
            {
                AdopterId = adopter.Id,
                PetId = pet.Id,
                Status = "Pending"
            };

            await repo.AddAsync(request);

            var finalCount = await context.AdoptionRequests.CountAsync();

            Assert.Equal(initialCount + 1, finalCount);
        }


        [Fact]
        public async Task GetAllAsync_ReturnsAllAdoptionRequests()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter = new Adopter { Email = "a@a.com", FullName = "Adopter1" };
            var pet = new Pet
            {
                Name = "Pet1",
                Species = "Cat",
                PhotoUrl = "http://example.com/cat.jpg"
            };

            context.Adopters.Add(adopter);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            context.AdoptionRequests.Add(new AdoptionRequest { AdopterId = adopter.Id, PetId = pet.Id, Status = "Pending" });
            context.AdoptionRequests.Add(new AdoptionRequest { AdopterId = adopter.Id, PetId = pet.Id, Status = "Approved" });
            await context.SaveChangesAsync();

            var result = await repo.GetAllAsync();

            Assert.Equal(2, result.Count());
            Assert.All(result, ar => Assert.NotNull(ar.Pet));
            Assert.All(result, ar => Assert.NotNull(ar.Adopter));
        }

        [Fact]
        public async Task GetByAdopterIdAsync_ReturnsCorrectRequests()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter1 = new Adopter { Email = "a@a.com", FullName = "Adopter1" };
            var adopter2 = new Adopter { Email = "b@b.com", FullName = "Adopter2" };
            var pet = new Pet
            {
                Name = "Pet1",
                Species = "Rabbit",
                PhotoUrl = "http://example.com/rabbit.jpg"
            };

            context.Adopters.AddRange(adopter1, adopter2);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            context.AdoptionRequests.Add(new AdoptionRequest { AdopterId = adopter1.Id, PetId = pet.Id, Status = "Pending" });
            context.AdoptionRequests.Add(new AdoptionRequest { AdopterId = adopter2.Id, PetId = pet.Id, Status = "Approved" });
            await context.SaveChangesAsync();

            var result = await repo.GetByAdopterIdAsync(adopter1.Id);

            Assert.Single(result);
            Assert.All(result, ar => Assert.Equal(adopter1.Id, ar.AdopterId));
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectRequest()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter = new Adopter { Email = "a@a.com", FullName = "Adopter1" };
            var pet = new Pet
            {
                Name = "Pet1",
                Species = "Bird",
                PhotoUrl = "http://example.com/bird.jpg"
            };

            context.Adopters.Add(adopter);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            var request = new AdoptionRequest { AdopterId = adopter.Id, PetId = pet.Id, Status = "Pending" };
            context.AdoptionRequests.Add(request);
            await context.SaveChangesAsync();

            var result = await repo.GetByIdAsync(request.Id);

            Assert.NotNull(result);
            Assert.Equal(request.Id, result.Id);
            Assert.NotNull(result.Adopter);
            Assert.NotNull(result.Pet);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAdoptionRequest()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter = new Adopter { Email = "a@a.com", FullName = "Adopter1" };
            var pet = new Pet
            {
                Name = "Pet1",
                Species = "Fish",
                PhotoUrl = "http://example.com/fish.jpg"
            };

            context.Adopters.Add(adopter);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            var request = new AdoptionRequest { AdopterId = adopter.Id, PetId = pet.Id, Status = "Pending" };
            context.AdoptionRequests.Add(request);
            await context.SaveChangesAsync();

            request.Status = "Approved";
            await repo.UpdateAsync(request);

            var updated = await context.AdoptionRequests.FindAsync(request.Id);
            Assert.Equal("Approved", updated.Status);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAdoptionRequest()
        {
            using var context = GetInMemoryDbContext();
            var repo = new AdoptionRequestRepository(context);

            var adopter = new Adopter { Email = "a@a.com", FullName = "Adopter1" };
            var pet = new Pet
            {
                Name = "Pet1",
                Species = "Lizard",
                PhotoUrl = "http://example.com/lizard.jpg"
            };

            context.Adopters.Add(adopter);
            context.Pets.Add(pet);
            await context.SaveChangesAsync();

            var request = new AdoptionRequest { AdopterId = adopter.Id, PetId = pet.Id, Status = "Pending" };
            context.AdoptionRequests.Add(request);
            await context.SaveChangesAsync();

            await repo.DeleteAsync(request.Id);

            var deleted = await context.AdoptionRequests.FindAsync(request.Id);
            Assert.Null(deleted);
        }
    }
}
