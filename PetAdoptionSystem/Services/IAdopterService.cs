using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public interface IAdopterService
    {
        Task<IEnumerable<Adopter>> GetAllAdoptersAsync();
        Task<Adopter?> GetAdopterByIdAsync(int id);
        Task<Adopter?> GetAdopterByEmailAsync(string email);
        Task<Adopter> AddAdopterAsync(Adopter adopter);
        Task UpdateAdopterAsync(Adopter adopter);
        Task DeleteAdopterAsync(int id);

        
    }
}
