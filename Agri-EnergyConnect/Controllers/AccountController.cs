using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Controllers
{
    //This is the controller which will handle all the authetication and registering of users on the website
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager; //This handles user opertaions
        private readonly SignInManager<ApplicationUser> _signInManager; //This handles signing in and signing out

        //Constructor to use the dependency injection 
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }



        //Gets the action for displaying the registration form
        [HttpGet]
        public IActionResult Register()
        {
            //This creates a drop down list of these roles so users can choose from them (Simplifies it for people using the website)
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Farmer", Text = "Farmer" },
                new SelectListItem { Value = "Employee", Text = "Employee" }
            };
            return View();
        }


        //This submits what the user has put on the registration form.
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
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

                    // Change this to not persist the sign-in
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", model.Role);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.Roles = new List<SelectListItem>
    {
        new SelectListItem { Value = "Farmer", Text = "Farmer" },
        new SelectListItem { Value = "Employee", Text = "Employee" }
    };

            return View(model);
        }


        //This gets the form for login 
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        //This submits action for login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                // Change this line to remove the persistent login option
                var result = await _signInManager.PasswordSignInAsync(
                    model.Email,
                    model.Password,
                    isPersistent: false,  // Set to false for session-only cookies
                    lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    if (roles.Contains("Farmer"))
                    {
                        return RedirectToAction("Index", "Farmer");
                    }
                    else if (roles.Contains("Employee"))
                    {
                        return RedirectToAction("Index", "Employee");
                    }

                    return LocalRedirect(returnUrl);
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home"); // Takes user to original homepage when they logout
        }
    }
}