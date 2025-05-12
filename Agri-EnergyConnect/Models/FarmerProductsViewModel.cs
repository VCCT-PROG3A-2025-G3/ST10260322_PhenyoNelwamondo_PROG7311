namespace Agri_EnergyConnect.Models
{
    public class FarmerProductsViewModel
    {
        public string SelectedFarmerId { get; set; }
        public List<ApplicationUser> Farmers { get; set; }
        public ProductFilterViewModel ProductFilter { get; set; }
    }
}