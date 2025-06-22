using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PetAdoptionSystem.Models
{
    public class AdoptionRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PetId { get; set; }

        [ForeignKey("PetId")]
        public Pet? Pet { get; set; }  // Remove [Required], make nullable

        [Required]
        public int AdopterId { get; set; }

        [ForeignKey("AdopterId")]
        public Adopter? Adopter { get; set; }  // Remove [Required], make nullable

        [Required]
        public string Status { get; set; } = "Pending";

        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
    }


}
