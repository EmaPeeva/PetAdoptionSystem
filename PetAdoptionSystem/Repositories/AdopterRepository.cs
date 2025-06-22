using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;
using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Repositories
{
    public class AdopterRepository : IAdopterRepository
    {
        private readonly AppDbContext _context;

        public AdopterRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Adopter>> GetAllAsync()
        {
            return await _context.Adopters.ToListAsync();
        }

        // Get adopted pets by adopter
        public async Task<Adopter?> GetByIdAsync(int id)
        {
            return await _context.Adopters
                .Include(a => a.AdoptedPets)  
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Adopter?> GetByEmailAsync(string email)
        {
            return await _context.Adopters.FirstOrDefaultAsync(a => a.Email == email);
        }

        public async Task<Adopter> AddAsync(Adopter adopter)
        {
            await _context.Adopters.AddAsync(adopter);
            await _context.SaveChangesAsync();
            return adopter;
        }


        public async Task UpdateAsync(Adopter adopter)
        {
            _context.Adopters.Update(adopter);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var adopter = await _context.Adopters.FindAsync(id);
            if (adopter != null)
            {
                _context.Adopters.Remove(adopter);
                await _context.SaveChangesAsync();
            }
        }
    }
}
