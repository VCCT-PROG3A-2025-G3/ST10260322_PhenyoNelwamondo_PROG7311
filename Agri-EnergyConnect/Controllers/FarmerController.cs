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
        private readonly ApplicationDbContext _context; //Links the database so it can be used in this controller
        private readonly UserManager<ApplicationUser> _userManager; //So i can have user operations
        

        //Constructor for this class
        public FarmerController(ApplicationDbContext context,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;   
        }


        //This is the method which ensures the users products are listed in the farmer dashboard
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); 
            }

            var products = await _context.Products
                .Where(p => p.UserId == user.Id)
                .OrderByDescending(p => p.ProductionDate)
                .ToListAsync();

            return View(products);
        }


        //Brings the view where the user can input their products
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }



        //This submits the form to add a product into the database.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProduct([Bind("Name,Category,ProductionDate")] Product productInput)
        {
            if (ModelState.IsValid) //Checks if the form is filled in correctly then the following wil happen
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
                        UserId = user.Id
                    };

                    _context.Products.Add(product); //This adds the product to the database.
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "An error occurred while saving the product.");
                }
            }
            return View(productInput);
        }
    }
}