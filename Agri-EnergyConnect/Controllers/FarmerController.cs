using Agri_EnergyConnect.Data;
using Agri_EnergyConnect.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Agri_EnergyConnect.Controllers
{
    [Authorize(Roles = "Farmer")] //Ensures only farmers can access the logic in this controller
    public class FarmerController : Controller
    {
        private readonly ApplicationDbContext _context; //This allows connection with the database
        private readonly UserManager<ApplicationUser> _userManager; //User Manager


        //Constructor for this controller
        public FarmerController( ApplicationDbContext context, UserManager<ApplicationUser> userManager) 
        {
            _context = context;
            _userManager = userManager;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User); //Gets the farmer thats currently logged in
            var products = await _context.Products 
                .Where(p => p.UserId == user.Id) //This ensures farmer can only see tehir own products
                .ToListAsync();

            return View(products); //This will allow farmers to see their own products in the dashboard
        }



        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(Product product)
        {
            if (ModelState.IsValid) //This checks if all form fields are correct first then the following can happen:
            {
                var user = await _userManager.GetUserAsync(User);//This gets teh ID of the farmer 
                product.UserId = user.Id; //This line links the product to the user(farmer) who is currently entering the form

                //This is the code that saves the product to the database
                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            return View(product);
        }
    }
}
