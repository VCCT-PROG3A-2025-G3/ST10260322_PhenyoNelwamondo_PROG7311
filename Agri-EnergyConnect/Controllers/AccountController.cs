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
            // This checks if all the validation for the form fields pass, then whats underneath the if statement can happen
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email, //This makes the username of the user his/her email
                    Email = model.Email,
                    Name = model.Name,
                    Surname = model.Surname
                };

                //This creates the user
                var result = await _userManager.CreateAsync(user, model.Password);

                //This is what happens if the user was sucessfuly created
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, model.Role); //Adds the user to the selected role they chose
                    await _signInManager.SignInAsync(user, isPersistent: false); //This ensures user is signed in after registering

                    return RedirectToAction("Index", model.Role);
                }
                foreach (var error in result.Errors) //This is what happens in case of errors:
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If registration fails then drop down list for roles is rebuilt so that it wont be empty.
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
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                //This is what happens if logins succeeds:
                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    //This takes the user back to their own specific dashboard
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