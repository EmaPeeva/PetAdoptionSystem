using PetAdoptionSystem.Models;
using PetAdoptionSystem.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public class PetService : IPetService
    {
        private readonly IPetRepository _petRepository;
        private readonly IAdopterRepository _adopterRepository;
        private readonly INotificationService _notificationService;

        public PetService(IPetRepository petRepository, IAdopterRepository adopterRepository, INotificationService notificationService)
        {
            _petRepository = petRepository;
            _adopterRepository = adopterRepository;
            _notificationService = notificationService;
        }

        public async Task<IEnumerable<Pet>> GetAllPetsAsync()
        {
            return await _petRepository.GetAllAsync();
        }

        public async Task<Pet?> GetPetByIdAsync(int id)
        {
            return await _petRepository.GetByIdAsync(id);
        }

        public async Task AddPetAsync(Pet pet)
        {
            await _petRepository.AddAsync(pet);
        }

        public async Task UpdatePetAsync(Pet pet)
        {
            await _petRepository.UpdateAsync(pet);
        }

        public async Task DeletePetAsync(int id)
        {
            var pet = await _petRepository.GetByIdAsync(id);
            if (pet == null)
                throw new KeyNotFoundException($"Pet with id {id} not found.");

            if (pet.IsAdopted)
                throw new InvalidOperationException("Cannot delete a pet that has already been adopted.");

            await _petRepository.DeleteAsync(id);
        }


        // Business logic: Adopt a pet
        public async Task<bool> AdoptPetAsync(int petId, int adopterId)
        {
            var pet = await _petRepository.GetByIdAsync(petId);
            var adopter = await _adopterRepository.GetByIdAsync(adopterId);

            if (pet == null || adopter == null || pet.IsAdopted)
                return false;

            pet.IsAdopted = true;
            pet.AdoptionDate = DateTime.UtcNow;
            pet.AdopterId = adopterId;

            await _petRepository.UpdateAsync(pet);

            // Send notification after successful adoption
            try
            {
                await _notificationService.SendAdoptionConfirmationEmailAsync(adopter.Email, pet.Name);
            }
            catch (Exception ex)
            {
                // Don’t stop the adoption process if email fails
                Console.WriteLine($"⚠️ Failed to send email: {ex.Message}");
            }

            return true;
        }


        public async Task<bool> TransferAdoptionAsync(int petId, int currentAdopterId, int newAdopterId)
        {
            // Step 1: Fetch pet and adopters
            var pet = await _petRepository.GetByIdAsync(petId);
            var currentAdopter = await _adopterRepository.GetByIdAsync(currentAdopterId);
            var newAdopter = await _adopterRepository.GetByIdAsync(newAdopterId);

            // Step 2: Validate data
            if (pet == null)
                throw new KeyNotFoundException($"Pet with id {petId} not found.");

            if (currentAdopter == null)
                throw new KeyNotFoundException($"Current adopter with id {currentAdopterId} not found.");

            if (newAdopter == null)
                throw new KeyNotFoundException($"New adopter with id {newAdopterId} not found.");

            if (!pet.IsAdopted || pet.AdopterId != currentAdopterId)
                throw new InvalidOperationException("Pet is not adopted by the current adopter.");

            // Step 3: Orchestrate the transfer
            pet.AdopterId = newAdopterId;
            pet.AdoptionDate = DateTime.UtcNow;

            // You could add additional business logic here, like sending notifications

            await _petRepository.UpdateAsync(pet);

            // Optional: log the transfer or call other services (email, audit, etc.)

            return true;
        }
        public async Task<IEnumerable<Pet>> GetPetsByAdopterIdAsync(int adopterId)
        {
            return await _petRepository.GetPetsByAdopterIdAsync(adopterId);
        }
        public async Task<IEnumerable<Pet>> GetAvailablePetsAsync()
        {
            var allPets = await _petRepository.GetAllAsync();
            return allPets.Where(p => !p.IsAdopted);
        }

        public async Task<IEnumerable<Pet>> GetAdoptedPetsAsync()
        {
            var allPets = await _petRepository.GetAllAsync();
            return allPets.Where(p => p.IsAdopted);
        }


    }
}
