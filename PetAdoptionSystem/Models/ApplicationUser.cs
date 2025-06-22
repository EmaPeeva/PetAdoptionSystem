using Microsoft.AspNetCore.Identity;

namespace PetAdoptionSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Link to the Adopter entity
        public int? AdopterId { get; set; }
    }
}
