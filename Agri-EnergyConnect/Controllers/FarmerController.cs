using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class FarmerController : Controller
    {
        [Authorize(Roles = "Farmer")] //This line of code makes it that this is specific to farmers only
        public IActionResult Index()
        {
            return View();
        }
    }
}
