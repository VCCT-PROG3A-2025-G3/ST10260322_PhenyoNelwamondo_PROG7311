using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Agri_EnergyConnect.Models;

namespace Agri_EnergyConnect.Controllers
{
    public class AccountController : Controller
    {
        
            private readonly UserManager<IdentityUser> _userManager;
            private readonly SignInManager<IdentityUser> _signInManager;

            public AccountController(
                UserManager<IdentityUser> userManager,
                SignInManager<IdentityUser> signInManager)
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
                    var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                    var result = await _userManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);

                        await _signInManager.SignInAsync(user, isPersistent: false);

                        return RedirectToAction("Index", model.Role + "Dashboard");
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                return View(model);
            }
        
    }
}
