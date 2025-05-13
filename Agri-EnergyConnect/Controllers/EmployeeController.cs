using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agri_EnergyConnect.Models;
using Agri_EnergyConnect.Data;

namespace Agri_EnergyConnect.Controllers
{
    [Authorize(Roles = "Employee")] //This ensures only users with the role of employee can access the logic in this controller:
    public class EmployeeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; //Can now do user operations
        private readonly ApplicationDbContext _context; //Can now link the database with the actions in this controller

        //This is the constructor for this controller:
        public EmployeeController(UserManager<ApplicationUser> userManager,ApplicationDbContext context)
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
                Role = "Farmer" //Because we need to add a famer role this will by default be farmere
            };
            return View(model);
        }

        
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

                // Create new user (This happens if it can confirm email is not the same)
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

                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }



        //This is gets teh view for a employee veing able to view a specific farmers products
        [HttpGet]
        public async Task<IActionResult> SelectFarmer()
        {
            var farmers = await _userManager.GetUsersInRoleAsync("Farmer");
            return View(new FarmerProductsViewModel { Farmers = farmers.ToList() });
        }

        //This gets the view for that specific farmers products.
        [HttpGet]
        public async Task<IActionResult> ViewFarmerProducts(
            string farmerId,
            string category = null, //The null ensures that it is optional
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            
            if (string.IsNullOrEmpty(farmerId))
            {
                return RedirectToAction("SelectFarmer");
            }

            //Gets the farmer that was selected
            var farmer = await _userManager.FindByIdAsync(farmerId);
            if (farmer == null)
            {
                return NotFound();
            }

            //Shows the products for the farmer
            var query = _context.Products
                .Where(p => p.UserId == farmerId)
                .Include(p => p.User)
                .AsQueryable();

            // This is the code which applies all the filters category and date
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
                .Where(p => p.UserId == farmerId)
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            
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

        //This gets the manage products view (Essentially shows all the products that are in teh database)
        [HttpGet]
        public async Task<IActionResult> ManageProducts(
            string category = null,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            
            var query = _context.Products
                .Include(p => p.User)  // Include farmer information so it can show who owns the product
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

            //Get categories that have already been used
            var categories = await _context.Products
                .Select(p => p.Category)
                .Distinct()
                .ToListAsync();

            
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