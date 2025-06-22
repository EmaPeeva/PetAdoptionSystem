using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PetAdoptionSystem.Data;

namespace PetAdoptionSystem.Repositories
{
    public class AdoptionRequestRepository : IAdoptionRequestRepository
    {
        private readonly AppDbContext _context;

        public AdoptionRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AdoptionRequest>> GetAllAsync()
        {
            return await _context.AdoptionRequests
                .Include(ar => ar.Pet)
                .Include(ar => ar.Adopter)
                .ToListAsync();
        }

        public async Task<IEnumerable<AdoptionRequest>> GetByAdopterIdAsync(int adopterId)
        {
            return await _context.AdoptionRequests
                .Include(ar => ar.Pet)
                .Where(ar => ar.AdopterId == adopterId)
                .ToListAsync();
        }

        public async Task<AdoptionRequest> GetByIdAsync(int id)
        {
            return await _context.AdoptionRequests
                .Include(ar => ar.Pet)
                .Include(ar => ar.Adopter)
                .FirstOrDefaultAsync(ar => ar.Id == id);
        }

        public async Task AddAsync(AdoptionRequest request)
        {
            _context.AdoptionRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdoptionRequest request)
        {
            _context.AdoptionRequests.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var request = await _context.AdoptionRequests.FindAsync(id);
            if (request != null)
            {
                _context.AdoptionRequests.Remove(request);
                await _context.SaveChangesAsync();
            }
        }
    }
}
