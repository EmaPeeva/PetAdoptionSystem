using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public class AdoptionRequestService : IAdoptionRequestService
    {
        private readonly IAdoptionRequestRepository _repository;

        public AdoptionRequestService(IAdoptionRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<AdoptionRequest>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<IEnumerable<AdoptionRequest>> GetByAdopterIdAsync(int adopterId)
        {
            return await _repository.GetByAdopterIdAsync(adopterId);
        }

        public async Task<AdoptionRequest> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(AdoptionRequest request)
        {
            await _repository.AddAsync(request);
        }

        public async Task UpdateAsync(AdoptionRequest request)
        {
            await _repository.UpdateAsync(request);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }
    }
}
