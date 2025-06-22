using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using PetAdoptionSystem.DTOs;

namespace PetAdoptionSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptionRequestsController : ControllerBase
    {
        private readonly IAdoptionRequestService _service;
        private readonly IAdopterService _adopterService;
        private readonly IPetService _petService;

        public AdoptionRequestsController(
            IAdoptionRequestService service,
            IAdopterService adopterService,
            IPetService petService)   // add this parameter
        {
            _service = service;
            _adopterService = adopterService;
            _petService = petService;  // assign it
        }


        // GET: api/adoptionrequests
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdoptionRequest>>> GetAll()
        {
            var requests = await _service.GetAllAsync();
            return Ok(requests);
        }

        // GET: api/adoptionrequests/mine
        [Authorize]
        [HttpGet("mine")]
        public async Task<ActionResult<IEnumerable<AdoptionRequest>>> GetMine()
        {
            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var appUser = await _adopterService.GetAdopterByEmailAsync(email);

            // 🔧 If adopter is missing, create them from identity
            if (appUser == null)
            {
                appUser = await _adopterService.AddAdopterAsync(new Adopter
                {
                    FullName = email.Split('@')[0], // use part of email as name
                    Email = email,
                    Role = "User"
                });
            }

            var requests = await _service.GetByAdopterIdAsync(appUser.Id);
            return Ok(requests);
        }


        // POST: api/adoptionrequests
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] AdoptionRequestCreateDTO dto)
        {
            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var adopter = await _adopterService.GetAdopterByEmailAsync(email);
            if (adopter == null)
                return NotFound("Adopter profile not found.");

            // Prevent duplicate requests
            var existingRequests = await _service.GetByAdopterIdAsync(adopter.Id);
            var duplicate = existingRequests.FirstOrDefault(r => r.PetId == dto.PetId);
            if (duplicate != null)
                return Conflict("You have already sent a request for this pet.");

            var request = new AdoptionRequest
            {
                PetId = dto.PetId,
                AdopterId = adopter.Id,
                Status = "Pending",
                RequestedAt = DateTime.UtcNow
            };

            await _service.AddAsync(request);

            var createdRequest = await _service.GetByIdAsync(request.Id);

            return CreatedAtAction(nameof(GetById), new { id = createdRequest.Id }, createdRequest);
        }




        // GET: api/adoptionrequests/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AdoptionRequest>> GetById(int id)
        {
            var request = await _service.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var adopter = await _adopterService.GetAdopterByEmailAsync(email);

            var isAdmin = User.IsInRole("Admin");

            // Allow if admin or if the request belongs to the current adopter
            var appUser = await _adopterService.GetAdopterByEmailAsync(User.Identity.Name);
            if (!isAdmin && (appUser == null || request.AdopterId != appUser.Id))
                return Forbid();

            return Ok(request);
        }


  
        // PUT: api/adoptionrequests/5
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateStatus(int id, [FromBody] AdoptionRequestUpdateStatusDTO dto)
        {
            var existingRequest = await _service.GetByIdAsync(id);
            if (existingRequest == null)
                return NotFound();

            // Update the adoption request status
            existingRequest.Status = dto.Status;
            await _service.UpdateAsync(existingRequest);

            if (dto.Status == "Approved")
            {
                // Use PetService's AdoptPetAsync to update pet adoption and send notification
                bool success = await _petService.AdoptPetAsync(existingRequest.PetId, existingRequest.AdopterId);

                if (!success)
                {
                    return BadRequest("Failed to adopt pet. It might already be adopted or invalid.");
                }

                // Reject other pending requests for the same pet (keep this)
                var allRequests = await _service.GetAllAsync();
                var otherPendingRequests = allRequests
                    .Where(r => r.PetId == existingRequest.PetId && r.Id != id && r.Status == "Pending")
                    .ToList();

                foreach (var req in otherPendingRequests)
                {
                    req.Status = "Rejected";
                    await _service.UpdateAsync(req);
                }
                return Ok(new { message = $"📧 Email sent to adopter confirming adoption." }); 
            }


            return Ok(new { message = "Status updated successfully." });

        }



        // DELETE: api/adoptionrequests/5
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var request = await _service.GetByIdAsync(id);
            if (request == null)
                return NotFound();

            var email = User.Identity?.Name;
            if (email == null)
                return Unauthorized();

            var isAdmin = User.IsInRole("Admin");
            var appUser = await _adopterService.GetAdopterByEmailAsync(email);

            if (!isAdmin && (appUser == null || request.AdopterId != appUser.Id))
                return Forbid();

            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
