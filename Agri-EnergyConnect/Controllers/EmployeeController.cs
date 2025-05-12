using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Data;

namespace Agri_EnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public EmployeeController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult AddFarmer()
        {
            var model = new RegisterViewModel
            {
                Role = "Farmer" // Default role for this form
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddFarmer(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.Role = "Farmer";

                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already registered.");
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("Index", "Employee");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SelectFarmer()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            return View(new FarmerProductsViewModel { Farmers = farmers.ToList() });
        }

        [HttpGet]
        public async Task<IActionResult> ViewFarmerProducts(string farmerId, string category = null,
            DateTime? startDate = null, DateTime? endDate = null)
        {
            if (string.IsNullOrEmpty(farmerId))
            {
                return RedirectToAction("SelectFarmer");
            }

            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
            {
                return NotFound();
            }

            // Base query for products
            var query = _context.Products
                .Where(p => p.UserId == farmerId)
                .Include(p => p.User)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= endDate.Value);
            }

            // Get available categories for dropdown
            var categories = await _context.Products
                .Where(p => p.UserId == farmerId)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            // Get all farmers for dropdown
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");

            var model = new FarmerProductsViewModel
            {
                SelectedFarmerId = farmerId,
                Farmers = farmers.ToList(),
                ProductFilter = new ProductFilterViewModel
                {
                    Category = category,
                    StartDate = startDate,
                    EndDate = endDate,
                    Products = await query.ToListAsync(),
                    Categories = categories
                }
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ViewProducts(string category = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            // This shows all products from all farmers (original functionality)
            var query = _context.Products
                .Include(p => p.User)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category == category);
            }

            if (startDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(p => p.ProductionDate <= endDate.Value);
            }

            var categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            var viewModel = new ProductFilterViewModel
            {
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                Products = await query.ToListAsync(),
                Categories = categories
            };

            return View(viewModel);
        }
    }
}