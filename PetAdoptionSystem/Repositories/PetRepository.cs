using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;
using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Repositories
{
    public class PetRepository : IPetRepository
    {
        private readonly AppDbContext _context;

        public PetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Pet>> GetAllAsync()
        {
            return await _context.Pets.ToListAsync();
        }

        public async Task<Pet?> GetByIdAsync(int id)
        {
            return await _context.Pets.FindAsync(id);
        }

        public async Task AddAsync(Pet pet)
        {
            await _context.Pets.AddAsync(pet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Pet pet)
        {
            // Get the tracked entity from the context
            var existingPet = await _context.Pets.FindAsync(pet.Id);

            if (existingPet == null)
                throw new KeyNotFoundException($"Pet with id {pet.Id} not found.");

            // Update fields manually
            existingPet.Name = pet.Name;
            existingPet.Species = pet.Species;
            existingPet.Age = pet.Age;
            existingPet.IsAdopted = pet.IsAdopted;
            existingPet.AdopterId = pet.AdopterId;
            existingPet.AdoptionDate = pet.AdoptionDate;
            existingPet.PhotoUrl = pet.PhotoUrl;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pet = await _context.Pets.FindAsync(id);
            if (pet != null)
            {
                _context.Pets.Remove(pet);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Pet>> GetPetsByAdopterIdAsync(int adopterId)
        {
            return await _context.Pets
                                 .Where(p => p.AdopterId == adopterId)
                                 .ToListAsync();
        }

    }
}
