using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Controllers
{
    
    [ApiController]
    [Route("api/[controller]")]
    public class PetsController : ControllerBase
    {
        private readonly IPetService _petService;
        private readonly IAdoptionRequestService _adoptionRequestService;

        public PetsController(IPetService petService, IAdoptionRequestService adoptionRequestService)
        {
            _petService = petService;
            _adoptionRequestService = adoptionRequestService;
        }

        // GET: api/pets
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pet>>> GetAllPets()
        {
            var pets = (await _petService.GetAllPetsAsync()).ToList();
            var adoptionRequests = await _adoptionRequestService.GetAllAsync();

            // For each pet, find if it has an approved adoption request
            foreach (var pet in pets)
            {
                var approvedRequest = adoptionRequests
                    .FirstOrDefault(r => r.PetId == pet.Id && r.Status == "Approved");

                if (approvedRequest != null)
                {
                    pet.IsAdopted = true;
                    pet.AdopterId = approvedRequest.AdopterId;
                    pet.AdoptionDate = approvedRequest.RequestedAt;
                }
                else
                {
                    pet.IsAdopted = false;
                    pet.AdopterId = null;
                    pet.AdoptionDate = null;
                }
            }

            return Ok(pets);
        }

        // GET: api/pets/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pet>> GetPetById(int id)
        {
            var pet = await _petService.GetPetByIdAsync(id);
            if (pet == null)
                return NotFound();

            return Ok(pet);
        }

        // POST: api/pets
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult> AddPet([FromBody] Pet pet)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _petService.AddPetAsync(pet);
            return CreatedAtAction(nameof(GetPetById), new { id = pet.Id }, pet);
        }

        // PUT: api/pets/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdatePet(int id, [FromBody] Pet pet)
        {
            if (id != pet.Id)
                return BadRequest("Pet ID mismatch.");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingPet = await _petService.GetPetByIdAsync(id);
            if (existingPet == null)
                return NotFound();

            await _petService.UpdatePetAsync(pet);
            return Ok(pet);
        }

        // DELETE: api/pets/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeletePet(int id)
        {
            try
            {
                await _petService.DeletePetAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST: api/pets/5/adopt/3
        [Authorize]
        [HttpPost("{petId}/adopt/{adopterId}")]
        public async Task<ActionResult> AdoptPet(int petId, int adopterId)
        {
            var success = await _petService.AdoptPetAsync(petId, adopterId);
            if (!success)
                return BadRequest("Pet is already adopted or invalid adopter.");

            return Ok("Pet adopted successfully.");
        }

        // POST: api/pets/5/transfer/2/3
        [Authorize]
        [HttpPost("{petId}/transfer/{currentAdopterId}/{newAdopterId}")]
        public async Task<ActionResult> TransferAdoption(int petId, int currentAdopterId, int newAdopterId)
        {
            try
            {
                var success = await _petService.TransferAdoptionAsync(petId, currentAdopterId, newAdopterId);
                if (!success)
                    return BadRequest("Transfer failed due to invalid data.");

                return Ok("Adoption transferred successfully.");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("adopter/{adopterId}")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByAdopter(int adopterId)
        {
            var pets = await _petService.GetPetsByAdopterIdAsync(adopterId);
            return Ok(pets);
        }

        [HttpGet("available")]
        public async Task<IActionResult> GetAvailablePets()
        {
            var pets = await _petService.GetAvailablePetsAsync();
            return Ok(pets);
        }

        [HttpGet("adopted")]
        public async Task<IActionResult> GetAdoptedPets()
        {
            var pets = await _petService.GetAdoptedPetsAsync();
            return Ok(pets);
        }


    }
}
