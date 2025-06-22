namespace PetAdoptionSystem.Models
{
    public class Adopter
    {
        public int Id { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;

        // New: User role for authorization (e.g., Admin, User)
        public string Role { get; set; } = "User";

        // Navigation property: list of pets adopted by this adopter
        public ICollection<Pet>? AdoptedPets { get; set; }
    }
}
