using PetAdoptionSystem.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public interface IPetService
    {
        Task<IEnumerable<Pet>> GetAllPetsAsync();
        Task<Pet?> GetPetByIdAsync(int id);
        Task AddPetAsync(Pet pet);
        Task UpdatePetAsync(Pet pet);
        Task DeletePetAsync(int id);

        // Additional business logic example:
        Task<bool> AdoptPetAsync(int petId, int adopterId);
        Task<bool> TransferAdoptionAsync(int petId, int currentAdopterId, int newAdopterId);
        Task<IEnumerable<Pet>> GetPetsByAdopterIdAsync(int adopterId);
        Task<IEnumerable<Pet>> GetAvailablePetsAsync();
        Task<IEnumerable<Pet>> GetAdoptedPetsAsync();

    }
}
