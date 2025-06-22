using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Repositories
{
    public interface IAdopterRepository
    {
        Task<IEnumerable<Adopter>> GetAllAsync();
        Task<Adopter?> GetByIdAsync(int id);
        Task<Adopter?> GetByEmailAsync(string email);
        Task<Adopter> AddAsync(Adopter adopter);
        Task UpdateAsync(Adopter adopter);
        Task DeleteAsync(int id);
    }
}
