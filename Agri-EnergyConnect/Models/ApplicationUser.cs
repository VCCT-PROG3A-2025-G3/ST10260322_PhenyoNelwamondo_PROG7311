using Microsoft.AspNetCore.Identity;

namespace Agri_EnergyConnect.Models
{
    public class ApplicationUser : IdentityUser
    {
        
        public string Name { get; set; }
        public string Surname { get; set; }

        public virtual ICollection<Product> Products { get; set; }


    }
}