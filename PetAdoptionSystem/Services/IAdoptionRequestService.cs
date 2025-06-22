using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public interface IAdoptionRequestService
    {
        Task<IEnumerable<AdoptionRequest>> GetAllAsync();
        Task<IEnumerable<AdoptionRequest>> GetByAdopterIdAsync(int adopterId);
        Task<AdoptionRequest> GetByIdAsync(int id);
        Task AddAsync(AdoptionRequest request);
        Task UpdateAsync(AdoptionRequest request);
        Task DeleteAsync(int id);
    }
}
