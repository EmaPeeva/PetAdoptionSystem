using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public class AdopterService : IAdopterService
    {
        private readonly IAdopterRepository _adopterRepository;

        public AdopterService(IAdopterRepository adopterRepository)
        {
            _adopterRepository = adopterRepository;
        }

        public async Task<IEnumerable<Adopter>> GetAllAdoptersAsync()
        {
            return await _adopterRepository.GetAllAsync();
        }

        public async Task<Adopter?> GetAdopterByIdAsync(int id)
        {
            return await _adopterRepository.GetByIdAsync(id);
        }

        public async Task<Adopter?> GetAdopterByEmailAsync(string email)
        {
            return await _adopterRepository.GetByEmailAsync(email);
        }

        public async Task<Adopter> AddAdopterAsync(Adopter adopter)
        {
            if (string.IsNullOrWhiteSpace(adopter.Email))
                throw new ArgumentException("Email is required.");

            var existing = await _adopterRepository.GetByEmailAsync(adopter.Email);
            if (existing != null)
                throw new InvalidOperationException("An adopter with this email already exists.");

            adopter = await _adopterRepository.AddAsync(adopter);
            return adopter;
        }


        public async Task UpdateAdopterAsync(Adopter adopter)
        {
            await _adopterRepository.UpdateAsync(adopter);
        }

        public async Task DeleteAdopterAsync(int id)
        {
            await _adopterRepository.DeleteAsync(id);
        }
    }
}
