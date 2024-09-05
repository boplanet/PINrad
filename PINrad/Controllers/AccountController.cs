using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using PINrad.Models;
using Microsoft.AspNetCore.Authorization;

namespace PINrad.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<RegLogUser> _signInManager;
        private readonly UserManager<RegLogUser> _userManager;

        public AccountController(SignInManager<RegLogUser> signInManager, UserManager<RegLogUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        // GET: Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        // POST: Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Ako prijava ne uspije, prikazujemo odgovarajuću poruku
                    if (result.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Račun je zaključan.");
                    }
                    else if (result.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "Prijava nije dopuštena.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Neispravan pokušaj prijave.");
                    }
                }
            }
            return View(model);
        }
        // GET: Register
        [HttpGet]
       // [AllowAnonymous]
       [Authorize(Roles = "Admin")] // samo za admin
        public IActionResult Register()
        {
            return View();
        }
        // POST: Register
        [HttpPost]
        //[AllowAnonymous]
        [Authorize(Roles = "Admin")] // samo za admin
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new RegLogUser { UserName = model.Email, Email = model.Email, PunoIme = model.PunoIme };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }
     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        /*  [HttpPost]
          [ValidateAntiForgeryToken]
          public async Task<IActionResult> Logout()
          {
              await _signInManager.SignOutAsync();
              return RedirectToAction("LoggedOut");
              // return RedirectToAction("Login", "Account");
          }

          [HttpGet]
          [AllowAnonymous]
          public IActionResult LoggedOut()
          {
              return View();
          } */

    }
}
