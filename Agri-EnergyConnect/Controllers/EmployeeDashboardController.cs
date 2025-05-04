using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Agri_EnergyConnect.Controllers
{
    public class EmployeeDashboardController : Controller
    {
        [Authorize(Roles = "Employee")] //This makes it specific to employees only
        public IActionResult Index()
        {
            return View();
        }

        
    }
}
