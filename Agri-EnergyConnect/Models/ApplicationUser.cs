using Microsoft.AspNetCore.Identity;

namespace Agri_EnergyConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Custom properties
        public string Name { get; set; }
        public string Surname { get; set; }

       
    }
}