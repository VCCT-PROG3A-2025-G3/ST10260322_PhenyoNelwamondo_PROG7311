using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Agri_EnergyConnect.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public DateTime ProductionDate { get; set; }

        [ForeignKey("User")] //This will link the product to the user that is currently logged in. 
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }
    }
}
