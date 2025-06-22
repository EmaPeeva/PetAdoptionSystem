using System.ComponentModel.DataAnnotations;

namespace PetAdoptionSystem.Models
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public int Age { get; set; }
        public bool IsAdopted { get; set; }

        // New: Date when adopted (nullable if not adopted yet)
        public DateTime? AdoptionDate { get; set; }

        // New: Foreign key to Adopter, nullable if not adopted
        public int? AdopterId { get; set; }
        public Adopter? Adopter { get; set; }

      
        [Url(ErrorMessage = "Photo URL must be a valid URL.")]
        public string PhotoUrl { get; set; } = null!;
    }
}
