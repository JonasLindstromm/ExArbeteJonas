using ExArbeteJonas.IdentityData;
using ExArbeteJonas.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        //Dependency Injection via konstruktorn
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // Visa alla användare
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            List<UserViewModel> allUsers = new List<UserViewModel>();

            // Läs alla Admin användare från databasen
            var Users = await _userManager.GetUsersInRoleAsync("Admin");
            foreach (var user in Users)
            {
                UserViewModel adminUser = new UserViewModel
                { Id = user.Id, Namn = user.Name, AnvandarNamn = user.UserName, Email = user.Email, RollNamn = "Admin" };
                {
                    allUsers.Add(adminUser);
                }
            }

            // Läs alla Member användare från databasen
            Users = await _userManager.GetUsersInRoleAsync("Member");
            foreach (var user in Users)
            {
                UserViewModel memberUser = new UserViewModel
                { Id = user.Id, Namn = user.Name, AnvandarNamn = user.UserName, Email = user.Email, RollNamn = "Member" };
                {
                    allUsers.Add(memberUser);
                }
            }

            // Sortera användarna efter Användarnamn
            allUsers.OrderBy(u => u.AnvandarNamn);

            return View(allUsers);
        }

        // Logga in en användare
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        // Logga in en användare
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginUser)
        {
            if (ModelState.IsValid)
            {
                //Kontrollera om det är en giltig användare
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginUser.AnvandarNamn, loginUser.Losenord, false, false);

                //Om inloggningen är riktig så ska en sida med Annonser visas
                if (result.Succeeded)
                {
                    return RedirectToAction("IndexAds", "Home");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Ogiltigt inloggningsförsök");
                }
            }

            return View(loginUser);
        }

        //Logga ut en användare       
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            // Visa startsidan
            return RedirectToAction("Index", "Home");
        }

        // Registrera ny Admin 
        [Authorize(Roles = "Admin")]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        // Registrera ny Admin 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel regUser)
        {
            if (ModelState.IsValid)
            {
                // Kopierar värdena som matades in i Registervyn till ett ApplicationUser objekt              
                ApplicationUser user = new ApplicationUser();

                user.Name = regUser.Namn;
                user.Email = regUser.Email;
                user.PhoneNumber = regUser.Telefon;
                user.UserName = regUser.AnvandarNamn;

                //Skapa användaren i databasen
                IdentityResult result = await _userManager.CreateAsync(user, regUser.Losenord);

                // Om det går bra tilldelas rollen Admin till användaren                
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, "Admin");

                    return RedirectToAction("Index");
                }

                AddErrors(result);
            }

            return View(regUser);
        }

        // Registrera sig som ny Medlem
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        // Registrera sig som ny Medlem
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel regUser)
        {
            if (ModelState.IsValid)
            {
                // Kopierar värdena som matades in i Registervyn till ett ApplicationUser objekt              
                ApplicationUser user = new ApplicationUser();

                user.Name = regUser.Namn;
                user.Email = regUser.Email;
                user.PhoneNumber = regUser.Telefon;
                user.UserName = regUser.AnvandarNamn;

                // Skapa användaren i databasen
                IdentityResult result = await _userManager.CreateAsync(user, regUser.Losenord);

                // Om det går bra tilldelas rollen Member till användaren                
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, "Member");
                }

                // Om det går bra loggas användaren in
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                AddErrors(result);
            }

            return View(regUser);
        }

        //Avregistrera en Administratör eller Medlem      
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Unregister(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Kontrollera så att jag inte tar bort inloggad Administratör från Databasen
            if (user.UserName != User.Identity.Name)
            {
                // Ta bort Medlemmen/Administratören från Databasen
                await _userManager.DeleteAsync(user);
            }

            return RedirectToAction("Index");
        }

        // Visa alla fel som har med Identity att göra
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}

