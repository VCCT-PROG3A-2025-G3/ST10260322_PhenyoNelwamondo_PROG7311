namespace Agri_EnergyConnect.Models
{
    public class ProductFilterViewModel
    {
        public string? Category { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
        public List<string> Categories { get; set; } = new List<string>();
    }
}