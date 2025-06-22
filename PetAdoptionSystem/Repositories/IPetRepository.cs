using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Repositories
{
    public interface IPetRepository
    {
        Task<IEnumerable<Pet>> GetAllAsync();
        Task<Pet?> GetByIdAsync(int id);
        Task AddAsync(Pet pet);
        Task UpdateAsync(Pet pet);
        Task DeleteAsync(int id);
        Task<IEnumerable<Pet>> GetPetsByAdopterIdAsync(int adopterId);

    }
}
