using Agri_EnergyConnect.Data;
using Agri_EnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Agri_EnergyConnect.Controllers
{
    [Authorize(Roles = "Farmer")]
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<FarmerController> _logger;

        public FarmerController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            ILogger<FarmerController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogError("No user found for index view");
                return Challenge();
            }

            _logger.LogInformation("Loading products for {UserId}", user.Id);

            var products = await _context.Products
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.ProductionDate)
                .ToListAsync();

            return View(products);
        }

        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("Name,Category,ProductionDate")] Product productInput)
        {

            _logger.LogInformation("ModelState.IsValid: {IsValid}", ModelState.IsValid);
            _logger.LogInformation("ModelState Errors: {@Errors}",
                ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    ));

            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    if (user == null) return Unauthorized();

                    var product = new Product
                    {
                        Name = productInput.Name,
                        Category = productInput.Category,
                        ProductionDate = productInput.ProductionDate,
                        UserId = user.Id // Set programmatically
                    };

                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error saving product");
                    ModelState.AddModelError("", "Error saving product");
                }
            }
            return View(productInput);
        }
    }
}