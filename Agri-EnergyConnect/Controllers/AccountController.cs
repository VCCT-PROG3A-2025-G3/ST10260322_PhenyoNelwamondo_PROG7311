using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Farmer", Text = "Farmer" },
                new SelectListItem { Value = "Employee", Text = "Employee" }
            };
            return View();
        }

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
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("Index", model.Role);
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Repopulate roles if we need to return the view
            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "Farmer", Text = "Farmer" },
                new SelectListItem { Value = "Employee", Text = "Employee" }
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    var roles = await _userManager.GetRolesAsync(user);

                    // Redirect based on role
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
            return RedirectToAction("Index", "Home");
        }
    }
}