using System;
using System.Threading.Tasks;

namespace PetAdoptionSystem.Services
{
    public class MockNotificationService : INotificationService
    {
        public async Task<bool> SendAdoptionConfirmationEmailAsync(string toEmail, string petName)
        {
            // Simulate sending email (in real project, you'd send actual email)
            await Task.Delay(500); // simulate delay
            Console.WriteLine($"📧 Email sent to {toEmail}: You adopted {petName}!");
            return true;
        }
    }
}
