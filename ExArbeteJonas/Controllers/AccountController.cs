using ExArbeteJonas.BusinessLayer;
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
        private readonly IMarketBusiness _businessLayer;

        //Dependency Injection via konstruktorn
        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IMarketBusiness businessLayer
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _businessLayer = businessLayer;
        }

        // Ta bort Medlem eller Administratör       
        public async Task<IActionResult> Delete(string id)
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

            UserViewModel viewModel = new UserViewModel();
            viewModel.UserName = user.UserName;
            viewModel.Id = user.Id;

            return View(viewModel);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            // Om inloggad användare är Medlem
            if (User.IsInRole("Member"))
            {
                // Ta bort alla annonser som lagts in av Medlemmen
                _businessLayer.DeleteMemberAds(id);

                // Logga ut Medlemmen
                await _signInManager.SignOutAsync();

                // Ta bort Medlemmen från Databasen
                await _userManager.DeleteAsync(user);

                // Visa startsidan
                return RedirectToAction("Index", "Home");
            }

            // Om inloggad användare är Administratör
            if (User.IsInRole("Admin"))
            {
                // Kontrollera så att jag inte tar bort inloggad Administratör 
                if (user.UserName != User.Identity.Name)
                {
                    // Ta bort alla annonser som lagts in av Medlemmen
                    _businessLayer.DeleteMemberAds(id);

                    // Ta bort Medlemmen från Databasen
                    await _userManager.DeleteAsync(user);
                }

                return RedirectToAction("Index");
            }

            // Visa sidan som visar alla annonser
            return RedirectToAction("IndexAds", "Home");
        }

        //Ändra uppgifter för Medlem eller Administratör         
        public async Task<IActionResult> Edit(string id)
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

            EditUserViewModel viewModel = new EditUserViewModel();
            viewModel.UserName = user.UserName;
            viewModel.Id = user.Id;
            viewModel.Name = user.Name;
            viewModel.Email = user.Email;
            viewModel.Phone = user.PhoneNumber;

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, EditUserViewModel viewModel)
        {
            if (id != viewModel.Id)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = viewModel.Name;
            user.Email = viewModel.Email;
            user.PhoneNumber = viewModel.Phone;

            IdentityResult result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return RedirectToAction("IndexAds", "Home");
            }

            AddErrors(result);

            return View(viewModel);
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
                { Id = user.Id, Name = user.Name, UserName = user.UserName, Email = user.Email, RoleName = "Admin" };
                {
                    allUsers.Add(adminUser);
                }
            }

            // Läs alla Member användare från databasen
            Users = await _userManager.GetUsersInRoleAsync("Member");
            foreach (var user in Users)
            {
                UserViewModel memberUser = new UserViewModel
                { Id = user.Id, Name = user.Name, UserName = user.UserName, Email = user.Email, RoleName = "Member" };
                {
                    allUsers.Add(memberUser);
                }
            }

            // Sortera användarna efter Användarnamn
            allUsers.OrderBy(u => u.UserName);

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
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginUser.UserName, loginUser.Password, false, false);

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

        // Registrera ny Administratör 
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAdmin(RegisterViewModel regUser)
        {
            if (ModelState.IsValid)
            {
                // Kopierar värdena som matades in i Registervyn till ett ApplicationUser objekt              
                ApplicationUser user = new ApplicationUser();

                user.Name = regUser.Name;
                user.Email = regUser.Email;
                user.PhoneNumber = regUser.Phone;
                user.UserName = regUser.UserName;

                //Skapa användaren i databasen
                IdentityResult result = await _userManager.CreateAsync(user, regUser.Password);

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

                user.Name = regUser.Name;
                user.Email = regUser.Email;
                user.PhoneNumber = regUser.Phone;
                user.UserName = regUser.UserName;

                // Skapa användaren i databasen
                IdentityResult result = await _userManager.CreateAsync(user, regUser.Password);

                // Om det går bra tilldelas rollen Member till användaren                
                if (result.Succeeded)
                {
                    result = await _userManager.AddToRoleAsync(user, "Member");
                }

                // Om det går bra loggas användaren in
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToAction("IndexAds", "Home");
                }
                AddErrors(result);
            }

            return View(regUser);
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

