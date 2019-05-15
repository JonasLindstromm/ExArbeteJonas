using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExArbeteJonas.BusinessLayer;
using Microsoft.AspNetCore.Identity;
using ExArbeteJonas.IdentityData;
using ExArbeteJonas.Models;
using ExArbeteJonas.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ExArbeteJonas.Controllers
{
    public class HomeController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> _signInManager;
        private readonly IMarketBusiness _businessLayer;

        //Dependency Injection via konstruktorn
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMarketBusiness businessLayer)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _businessLayer = businessLayer;
        }

        // Lägg till ny AnnonsTyp
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAdType()
        {
            return View();
        }

        // Lägg till ny AnnonsTyp
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAdType([Bind("Id, Name")] AdType adType)
        {
            if (ModelState.IsValid)
            {
                // Begär att BusinessLagret lägger till den nya AnnonsTypen
                string result = _businessLayer.CreateAdType(adType);
                if (result == "OK")
                {
                    return RedirectToAction("IndexTypes");
                }
                else
                {
                    AddError(result);
                    return View(adType);
                }
            }

            return View(adType);
        }

        // Lägg in ny Annons
        [Authorize(Roles = "Member")]
        public IActionResult CreateAdv()
        {
            CreateAdvViewModel viewModel = new CreateAdvViewModel();

            var advTypes = _businessLayer.GetAdvTypes();

            foreach (var advType in advTypes)
            {
                viewModel.AdvTypeNames.Add(new SelectListItem { Text = advType.Name, Value = advType.Id.ToString() });
            }

            return View(viewModel);
        }

        // Lägg in ny annons
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> CreateAdv(CreateAdvViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Begär att BusinessLagret skapar en ny annons
                Advertisement adv = new Advertisement();

                adv.AdvTypeId = viewModel.CurrentAdv.AdvTypeId;
                adv.Title = viewModel.CurrentAdv.Title;
                adv.Description = viewModel.CurrentAdv.Description;
                adv.Price = viewModel.CurrentAdv.Price;
                adv.Place = viewModel.CurrentAdv.Place;
                adv.StartDate = DateTime.Now;

                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name.ToLower());
                adv.MemberId = user.Id;

                string result = _businessLayer.CreateAdv(adv);
                if (result == "OK")
                {
                    // Ge Medlemmen möjlighet att lägga till utrustning till en annons            
                    return RedirectToAction("CreateEquipment", new { advId = adv.Id });
                }
                else
                {
                    AddError(result);
                }
            }

            var advTypes = _businessLayer.GetAdvTypes();

            foreach (var advType in advTypes)
            {
                viewModel.AdvTypeNames.Add(new SelectListItem { Text = advType.Name, Value = advType.Id.ToString() });
            }

            return View(viewModel);
        }

        // Lägg in ny utrustning till en annons      
        public IActionResult CreateEquipment(int advId)
        {
            CreateEqViewModel viewModel = new CreateEqViewModel();
            viewModel.CurrentAdvId = advId;
            viewModel.CurrentAdv = _businessLayer.GetAdv(advId);
            viewModel.ExistingEqm = _businessLayer.GetEquipment(advId);
            var eqTypes = _businessLayer.GetEquipmentTypes();

            foreach (var eqType in eqTypes)
            {
                viewModel.EqTypeNames.Add(new SelectListItem { Text = eqType.Name, Value = eqType.Id.ToString() });
            }

            return View(viewModel);
        }

        // Lägg in ny utrustning till en annons
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public IActionResult CreateEquipment(CreateEqViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Begär att BusinessLagret lägger in utrustning till annonsen
                Equipment eqm = new Equipment();
                eqm.ActualAdId = viewModel.CurrentAdvId;
                eqm.EqTypeId = viewModel.CurrentEqm.EqTypeId;
                eqm.Make = viewModel.CurrentEqm.Make;
                eqm.Model = viewModel.CurrentEqm.Model;
                eqm.Size = viewModel.CurrentEqm.Size;
                eqm.Length = viewModel.CurrentEqm.Length;
                string result = _businessLayer.CreateEqm(eqm);
                if (result == "OK")
                {
                    // Om annonsen innehåller mindre än 2 Utrustningar
                    if (_businessLayer.GetEquipment(eqm.ActualAdId).Count() < 2)
                    {
                        // Ge Medlemmen möjlighet att lägga till ytterligare utrustning till annonsen            
                        return RedirectToAction("CreateEquipment", new { advId = eqm.ActualAdId });
                    }
                    else
                    {
                        return RedirectToAction("IndexOwnAds");
                    }
                }
                else
                {
                    AddError(result);
                }
            }
          
            viewModel.CurrentAdv = _businessLayer.GetAdv(viewModel.CurrentAdvId);
            viewModel.ExistingEqm = _businessLayer.GetEquipment(viewModel.CurrentAdvId);
            var eqTypes = _businessLayer.GetEquipmentTypes();

            foreach (var eqType in eqTypes)
            {
                viewModel.EqTypeNames.Add(new SelectListItem { Text = eqType.Name, Value = eqType.Id.ToString() });
            }
           
            return View(viewModel);
        }

        // Lägg till ny UtrustningsTyp
        [Authorize(Roles = "Admin")]
        public IActionResult CreateEquipmentType()
        {
            return View();
        }

        // Lägg till ny UtrustningsTyp
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateEquipmentType([Bind("Id, Name")] EquipmentType eqType)
        {
            if (ModelState.IsValid)
            {
                // Begär att BusinessLagret lägger till den nya AnnonsTypen
                string result = _businessLayer.CreateEquipmentType(eqType);
                if (result == "OK")
                {
                    return RedirectToAction("IndexTypes");
                }
                else
                {
                    AddError(result);
                    return View(eqType);
                }
            }

            return View(eqType);
        }

        // Ta bort annons
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAdv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Begär att BusinessLagret hämtar en viss annons
            Advertisement adv = _businessLayer.GetAdv((int)id);

            if (adv == null)
            {
                return NotFound();
            }


            // Begär att BusinessLagret tar bort annonsen
            _businessLayer.DeleteAdv(adv);

            return RedirectToAction("IndexAds");
        }

        // Ta bort egen annons
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> DeleteOwnAdv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Begär att BusinessLagret hämtar en specifik annons
            Advertisement adv = _businessLayer.GetAdv((int)id);

            if (adv == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Medlemmar ska bara kunna ta bort sina egna annonser
            if (adv.MemberId == user.Id)
            {
                // Begär att BusinessLagret tar bort annonsen
                _businessLayer.DeleteAdv(adv);
            }

            return RedirectToAction("IndexOwnAds");
        }

        // Visa alla detaljer i en annons
        public IActionResult DetailsAdv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Begär att BusinessLagret hämtar en viss annons
            Advertisement ad = _businessLayer.GetAdv((int)(id));

            if (ad == null)
            {
                return NotFound();
            }

            AdvDetailsViewModel viewModel = new AdvDetailsViewModel();
            viewModel.AdvId = (int)id;
            viewModel.AdvType = ad.AdvType;
            viewModel.Member = ad.Member;
            viewModel.Title = ad.Title;
            viewModel.Description = ad.Description;
            viewModel.Price = ad.Price;
            viewModel.Place = ad.Place;
            viewModel.StartDate = ad.StartDate;
            viewModel.Equipments = _businessLayer.GetEquipment((int)(id));

            return View(viewModel);
        }

        // Visa Startsidan       
        public IActionResult Index()
        {
            return View();
        }

        // Visa alla annonser      
        public IActionResult IndexAds()
        {
            // För gamla annonser ska tas bort 
            // Begär att Businesslagret gör detta
            _businessLayer.DeleteOldAds();

            AdsViewModel viewModel = new AdsViewModel();

            // Begär att BusinessLagret ger alla aktuella annonser
            viewModel.CurrentAds = _businessLayer.GetCurrentAds();

            if (viewModel.CurrentAds.Count > 0)
            {
                viewModel.PageHeading = "Alla Annonser";
            }
            else
            {
                viewModel.PageHeading = "Inga Aktuella Annonser";
            }

            return View(viewModel);
        }

        // Visa alla egna annonser   
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexOwnAds()
        {
            AdsViewModel viewModel = new AdsViewModel();

            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Begär att BusinessLagret ger mina aktuella annonser
            viewModel.CurrentAds = _businessLayer.GetUserAds(user.Id);

            return View(viewModel);
        }

        // Visa sidan för administration av AnnonsTyper och UtrustningsTyper 
        [Authorize(Roles = "Admin")]
        public IActionResult IndexTypes()
        {
            TypesViewModel allTypes = new TypesViewModel();
            allTypes.AdTypeNames = _businessLayer.GetAdvTypeNames();
            allTypes.EquipmentTypeNames = _businessLayer.GetEquipmentTypeNames();

            return View(allTypes);
        }

        // Sök annonser       
        public IActionResult SearchAds()
        {
            SearchAdsViewModel viewModel = new SearchAdsViewModel();

            var advTypes = _businessLayer.GetAdvTypes();
            viewModel.AdvTypeNames.Add(new SelectListItem { Text = "Alla Annonstyper", Value = "0" });
            foreach (var advType in advTypes)
            {
                viewModel.AdvTypeNames.Add(new SelectListItem { Text = advType.Name, Value = advType.Id.ToString() });
            }

            var eqTypes = _businessLayer.GetEquipmentTypes();
            viewModel.EqTypeNames.Add(new SelectListItem { Text = "Alla Utrustningstyper", Value = "0" });
            foreach (var eqType in eqTypes)
            {
                viewModel.EqTypeNames.Add(new SelectListItem { Text = eqType.Name, Value = eqType.Id.ToString() });
            }

            return View(viewModel);
        }

        // Sök annonser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SearchAds(SearchAdsViewModel vM)
        {
            if (ModelState.IsValid)
            {                
                // För gamla annonser ska tas bort 
                // Begär att Businesslagret gör detta
                _businessLayer.DeleteOldAds();

                AdsViewModel adsViewModel = new AdsViewModel();

                // Begär att BusinessLagret söker annonser enligt angivna sökvillkor               
                adsViewModel.CurrentAds = _businessLayer
                    .SearchCurrentAds(vM.AdvTypeId, vM.EqTypeId, vM.SearchTitle, vM.SearchDescription, vM.SearchPlace);

                if (adsViewModel.CurrentAds.Count > 0)
                {
                    adsViewModel.PageHeading = "Sökta Annonser";
                }
                else
                {
                    adsViewModel.PageHeading = "Inga Funna Annonser";
                }

                return View("IndexAds", adsViewModel);
            }

            var advTypes = _businessLayer.GetAdvTypes();

            vM.AdvTypeNames.Add(new SelectListItem { Text = "Alla Annonstyper", Value = "0" });
            foreach (var advType in advTypes)
            {
                vM.AdvTypeNames.Add(new SelectListItem
                {
                    Text = advType.Name,
                    Value = advType.Id.ToString()
                });
            }

            var eqTypes = _businessLayer.GetEquipmentTypes();
            vM.EqTypeNames.Add(new SelectListItem { Text = "Alla Utrustningstyper", Value = "0" });
            foreach (var eqType in eqTypes)
            {
                vM.EqTypeNames.Add(new SelectListItem { Text = eqType.Name, Value = eqType.Id.ToString() });
            }

            return View(vM);
        }

        // Visa felmeddelande
        private void AddError(string errorDescription)
        {
            ModelState.AddModelError(string.Empty, errorDescription);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
