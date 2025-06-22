using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PetAdoptionSystem.Models;
using PetAdoptionSystem.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdoptersController : ControllerBase
    {
        private readonly IAdopterService _adopterService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdoptersController(IAdopterService adopterService, UserManager<ApplicationUser> userManager)
        {
            _adopterService = adopterService;
            _userManager = userManager;
        }


        // GET: api/adopters
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Adopter>>> GetAllAdopters()
        {
            var adopters = await _adopterService.GetAllAdoptersAsync();
            return Ok(adopters);
        }

        // Allow regular users to get their own profile
        // GET: api/adopters/5
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Adopter>> GetAdopterById(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            // Allow if admin OR user is requesting their own profile
            var user = await _userManager.FindByIdAsync(currentUserId); // You might need to inject UserManager<ApplicationUser> into your controller

            if (!isAdmin)
            {
                // Find ApplicationUser by currentUserId and check if adopter.Id == requested id
                if (user == null || user.AdopterId != id)
                {
                    return Forbid();
                }
            }

            var adopter = await _adopterService.GetAdopterByIdAsync(id);
            if (adopter == null)
                return NotFound();

            return Ok(adopter);
        }


        // POST: api/adopters
        [HttpPost]
        public async Task<ActionResult> AddAdopter([FromBody] Adopter adopter)
        {
            try
            {
                await _adopterService.AddAdopterAsync(adopter);
                return CreatedAtAction(nameof(GetAdopterById), new { id = adopter.Id }, adopter);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateAdopter(int id, [FromBody] Adopter adopter)
        {
            if (id != adopter.Id)
                return BadRequest("Adopter ID mismatch.");

            var email = User.FindFirstValue(ClaimTypes.Name);
            var user = await _userManager.FindByEmailAsync(email);
            var isAdmin = User.IsInRole("Admin");

            if (!isAdmin && (user == null || user.AdopterId != id))
                return Forbid();

            var existingAdopter = await _adopterService.GetAdopterByIdAsync(id);
            if (existingAdopter == null)
                return NotFound();

            // Update Adopter info
            existingAdopter.FullName = adopter.FullName;
            existingAdopter.Email = adopter.Email;

            try
            {
                await _adopterService.UpdateAdopterAsync(existingAdopter);

                // Update ApplicationUser email and username if email changed
                if (user != null && user.AdopterId == id && user.Email != adopter.Email)
                {
                    user.Email = adopter.Email;
                    user.UserName = adopter.Email;
                    var result = await _userManager.UpdateAsync(user);

                    if (!result.Succeeded)
                    {
                        // Optional: log errors and return server error
                        return StatusCode(500, "Failed to update user email.");
                    }
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                // log exception here
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }



        [Authorize]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdopter(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var user = await _userManager.FindByIdAsync(currentUserId);
            if (user == null)
                return Unauthorized();

            if (!isAdmin && user.AdopterId != id)
                return Forbid();

            var existingAdopter = await _adopterService.GetAdopterByIdAsync(id);
            if (existingAdopter == null)
                return NotFound();

            await _adopterService.DeleteAdopterAsync(id);
            return NoContent();
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<Adopter>> GetCurrentAdopterProfile()
        {
            // Get user email from claims
            var userEmail = User.FindFirstValue(ClaimTypes.Name);
            if (userEmail == null)
            {
                userEmail = User.FindFirstValue("sub");
            }

            if (userEmail == null)
                return Unauthorized();

            // Find ApplicationUser by email
            var appUser = await _userManager.FindByEmailAsync(userEmail);
            if (appUser == null)
                return Unauthorized();

            if (appUser.AdopterId == null)
                return NotFound("Adopter profile not found.");

            var adopter = await _adopterService.GetAdopterByIdAsync(appUser.AdopterId.Value);
            if (adopter == null)
                return NotFound();

            return Ok(adopter);
        }



    }
}
