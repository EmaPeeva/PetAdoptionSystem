namespace PetAdoptionSystem.Services
{
    public interface INotificationService
    {
        Task<bool> SendAdoptionConfirmationEmailAsync(string toEmail, string petName);
    }
}
