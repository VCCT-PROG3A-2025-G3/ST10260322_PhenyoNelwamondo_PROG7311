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

        // GET: Employee Dashboard
        public IActionResult Index()
        {
            return View();
        }

        // GET: Add Farmer Form
        [HttpGet]
        public IActionResult AddFarmer()
        {
            var model = new RegisterViewModel
            {
                Role = "Farmer" // Default role for this form
            };
            return View(model);
        }

        // POST: Add New Farmer
        [HttpPost]
        public async Task<IActionResult> AddFarmer(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Force the role to be Farmer
                model.Role = "Farmer";

                // Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError(string.Empty, "Email is already registered.");
                    return View(model);
                }

                // Create new user
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname
                };

                // Save user
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Assign Farmer role
                    await _userManager.AddToRoleAsync(user, model.Role);
                    return RedirectToAction("Index", "Employee");
                }

                // Add errors if any
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        // GET: Select Farmer to View Products
        [HttpGet]
        public async Task<IActionResult> SelectFarmer()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            return View(new FarmerProductsViewModel { Farmers = farmers.ToList() });
        }

        // GET: View Products for Specific Farmer
        [HttpGet]
        public async Task<IActionResult> ViewFarmerProducts(
            string farmerId,
            string category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Validate farmerId
            if (string.IsNullOrEmpty(farmerId))
            {
                return RedirectToAction("SelectFarmer");
            }

            // Get farmer
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
            {
                return NotFound();
            }

            // Base query for farmer's products
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

            // Get available categories for this farmer
            var categories = await _context.Products
                .Where(p => p.UserId == farmerId)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            // Get all farmers for dropdown
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");

            // Prepare view model
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

        // GET: View All Products from All Farmers
        [HttpGet]
        public async Task<IActionResult> ManageProducts(
            string category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            // Base query with farmer information
            var query = _context.Products
                .Include(p => p.User)  // Include farmer information
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

            // Get distinct categories for dropdown
            var categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            // Prepare view model
            var model = new ProductManagementViewModel
            {
                Category = category,
                StartDate = startDate,
                EndDate = endDate,
                Products = await query.ToListAsync(),
                Categories = categories
            };

            return View(model);
        }
    }
}