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
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using Rotativa.AspNetCore;
using System.Runtime.InteropServices.ComTypes;

namespace ExArbeteJonas.Controllers
{
    public class HomeController : Controller
    {
        private readonly Microsoft.AspNetCore.Identity.UserManager<ApplicationUser> _userManager;
        private readonly Microsoft.AspNetCore.Identity.SignInManager<ApplicationUser> _signInManager;
        private readonly IMarketBusiness _businessLayer;
        private IHostingEnvironment _hostingEnv;

        //Dependency Injection via konstruktorn
        public HomeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMarketBusiness businessLayer,
            IHostingEnvironment hostingEnv)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _businessLayer = businessLayer;
            _hostingEnv = hostingEnv;
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
                    return RedirectToAction("IndexAdmin");
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
                Advertisement adv = new Advertisement();
                adv.AdvTypeId = viewModel.CurrentAdv.AdvTypeId;
                adv.Title = viewModel.CurrentAdv.Title;
                adv.Description = viewModel.CurrentAdv.Description;
                adv.Price = viewModel.CurrentAdv.Price;
                adv.Place = viewModel.CurrentAdv.Place;
                adv.StartDate = DateTime.Now;

                IFormFile imageFile = viewModel.MyImage;

                // Om medlemmen har laddat upp en bildfil
                if (imageFile != null)
                {
                    if (imageFile.Length > 0)
                    {
                        adv.ImageFileName = _businessLayer.SaveImage(imageFile);
                    }
                }

                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name.ToLower());
                adv.MemberId = user.Id;

                // Begär att BusinessLagret skapar en ny annons
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

        // Lägg till en ny Regel
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAdvRule()
        {
            return View();
        }

        // Lägg till en ny Regel
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateAdvRule([Bind("Id, Title, Description")] AdvRule advRule)
        {
            if (ModelState.IsValid)
            {
                // Begär att BusinessLagret lägger till den nya Regeln
                string result = _businessLayer.CreateAdvRule(advRule);
                if (result == "OK")
                {
                    return RedirectToAction("IndexAdmin");
                }
                else
                {
                    AddError(result);
                    return View(advRule);
                }
            }

            return View(advRule);
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

        // Lägg till en ny UtrustningsTyp
        [Authorize(Roles = "Admin")]
        public IActionResult CreateEquipmentType()
        {
            return View();
        }

        // Lägg till en ny UtrustningsTyp
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
                    return RedirectToAction("IndexAdmin");
                }
                else
                {
                    AddError(result);
                    return View(eqType);
                }
            }

            return View(eqType);
        }

        // Visa olika typer av statistik för Annonser
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStatistics()
        {
            CreateStatisticsViewModel viewModel = new CreateStatisticsViewModel();
            viewModel.StatisticsTypes.Add(new SelectListItem { Text = "Statistik över antal aktuella annonser", Value = "1" });
            viewModel.StatisticsTypes.Add(new SelectListItem { Text = "Statistik över genomsnittligt antal dagar för aktuella annonser", Value = "2" });
            viewModel.StatisticsTypes.Add(new SelectListItem { Text = "Statistik över genomsnittligt pris för aktuella annonser", Value = "3" });
            viewModel.StatisticsTypes.Add(new SelectListItem { Text = "Statistik över antal borttagna annonser", Value = "4" });
            viewModel.StatisticsTypes.Add(new SelectListItem { Text = "Statistik över genomsnittligt antal dagar för borttagna annonser", Value = "5" });
            return View(viewModel);
        }

        // Visa olika typer av statistik för Annonser
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStatistics(CreateStatisticsViewModel viewModel)
        {
            StatisticsAdsViewModel statvM = new StatisticsAdsViewModel();
            var adTypeNames = _businessLayer.GetAdvTypeNames();
            statvM.AdTypeNames = adTypeNames;
            var eqTypeNames = _businessLayer.GetEquipmentTypeNames();
            statvM.EqTypeNames = eqTypeNames;
            switch (viewModel.TypeId)
            {
                case 1:
                    statvM.Heading = "Statistik över antal aktuella Annonser:";
                    statvM.Statistics = _businessLayer.GetNrAdsStatistics(eqTypeNames, adTypeNames);
                    return PartialView("_StatisticsNrAdsPartial", statvM);
                case 2:
                    statvM.Heading = "Statistik över genomsnittligt antal dagar för aktuella annonser:";
                    statvM.Statistics = _businessLayer.GetAgeAdsStatistics(eqTypeNames, adTypeNames);
                    return PartialView("_StatisticsAgeAdsPartial", statvM);
                case 3:
                    statvM.Heading = "Statistik över genomsnittligt pris för aktuella annonser:";
                    statvM.Statistics = _businessLayer.GetPriceAdsStatistics(eqTypeNames, adTypeNames);
                    return PartialView("_StatisticsPriceAdsPartial", statvM);
                case 4:
                    statvM.Heading = "Statistik över antal borttagna Annonser:";
                    statvM.Statistics = _businessLayer.GetNrDeletedAdsStatistics(eqTypeNames, adTypeNames);
                    return PartialView("_StatisticsNrAdsPartial", statvM);
                default:  // viewModel.TypeId == 5
                    statvM.Heading = "Statistik över genomsnittligt antal dagar för borttagna annonser:";
                    statvM.Statistics = _businessLayer.GetAgeDeletedAdsStatistics(eqTypeNames, adTypeNames);
                    return PartialView("_StatisticsAgeAdsPartial", statvM);
            }
           
        }

        // Ta bort en egen annons
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

        // Ta bort en egen annons
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


        // Ta bort en regel för annonsering
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteAdvRule(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Begär att BusinessLagret hämtar en viss regel
            AdvRule advRule = _businessLayer.GetAdvRule((int)id);

            if (advRule == null)
            {
                return NotFound();
            }

            // Begär att BusinessLagret tar bort regeln
            _businessLayer.DeleteAdvRule(advRule);

            return RedirectToAction("IndexTypes");
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
            viewModel.ImageFileName = ad.ImageFileName;
            viewModel.Equipments = _businessLayer.GetEquipment((int)(id));

            return View(viewModel);
        }

        // Visa en hel regel, i en Partial View
        public IActionResult DetailsAdvRule(int id)
        {
            AdvRule advRule = _businessLayer.GetAdvRule(id);

            return PartialView("_DetailsAdvRulePartial", advRule);
        }

        // Uppdatera annons
        [Authorize(Roles = "Member")]
        public IActionResult EditAdv(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adv = _businessLayer.GetAdv((int)id);
            if (adv == null)
            {
                return NotFound();
            }

            CreateAdvViewModel viewModel = new CreateAdvViewModel();
            viewModel.CurrentAdv = adv;

            var advTypes = _businessLayer.GetAdvTypes();

            foreach (var advType in advTypes)
            {
                viewModel.AdvTypeNames.Add(new SelectListItem { Text = advType.Name, Value = advType.Id.ToString() });
            }

            return View(viewModel);
        }

        // Uppdatera annons
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> EditAdv(CreateAdvViewModel viewModel)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name.ToLower());
            if (viewModel.CurrentAdv.MemberId != user.Id)
            {
                ModelState.AddModelError(string.Empty, "Annonsen kan bara uppdateras av Medlemmen som la in annonsen");
            }

            if (ModelState.IsValid)
            {
                IFormFile imageFile = viewModel.MyImage;
                // Om medlemmen har laddat upp en ny bildfil
                if (imageFile != null)
                {
                    if (imageFile.Length > 0)
                    {
                        viewModel.CurrentAdv.ImageFileName = _businessLayer.SaveImage(imageFile);
                    }
                }

                // Begär att BusinessLagret uppdaterar annonsen
                string result = _businessLayer.UpdateAdv(viewModel.CurrentAdv);
                if (result == "OK")
                {
                    return RedirectToAction("IndexOwnAds");
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

        // Visa sidan med Regler för Prylmarknaden      
        public IActionResult IndexRules()
        {
            RulesViewModel viewModel = new RulesViewModel();
            viewModel.CurrentRules = _businessLayer.GetRules();

            return View(viewModel);
        }

        // Visa medlemmens egna annonser   
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> IndexOwnAds()
        {
            AdsViewModel viewModel = new AdsViewModel();

            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);

            // Begär att BusinessLagret ger mina aktuella annonser
            viewModel.CurrentAds = _businessLayer.GetUserAds(user.Id);

            return View(viewModel);
        }

        // Visa sidan för administration av AnnonsTyper, UtrustningsTyper och Regler
        [Authorize(Roles = "Admin")]
        public IActionResult IndexAdmin()
        {
            AdminViewModel viewModel = new AdminViewModel();
            viewModel.AdTypeNames = _businessLayer.GetAdvTypeNames();
            viewModel.EquipmentTypeNames = _businessLayer.GetEquipmentTypeNames();
            viewModel.CurrentRules = _businessLayer.GetRules();

            return View(viewModel);
        }

        // Visa en annons som Pdf         
        public IActionResult PdfAdv(int? id)
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
            viewModel.ImageFileName = ad.ImageFileName;
            viewModel.Equipments = _businessLayer.GetEquipment((int)(id));

            /*  Fungerar inte att sätta CustomSwitches och FileName
            var pdfResult = new ViewAsPdf("PdfAdv", viewModel);
            {
                CustomSwitches = "--page-offset 0 --footer-center [page] --footer-font-size 12"
                FileName = "";
            };
            return pdfResult;
            */

            return new ViewAsPdf("PdfAdv", viewModel);
        }

        // Skicka Meddelande med Epost till Annonsör
        public IActionResult SendMsg(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            SendEmailViewModel viewModel = new SendEmailViewModel();
            viewModel.AdvId = (int)id;

            Advertisement adv = _businessLayer.GetAdv((int)id);

            viewModel.AdvTitle = adv.Title;
            viewModel.AdvTypeName = adv.AdvType.Name;
            viewModel.UserName = adv.Member.UserName;

            return PartialView("_SendMsgPartial", viewModel);
        }

        // Skicka Epost till annonsör
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SendMsg(SendEmailViewModel viewModel)
        {
            Advertisement adv = _businessLayer.GetAdv(viewModel.AdvId);

            if (ModelState.IsValid)
            {
                _businessLayer.SendEmail(viewModel.Subject, viewModel.Message, adv.Member.Email);

                return RedirectToAction("DetailsAdv", new { id = adv.Id });
            }

            viewModel.AdvTitle = adv.Title;
            viewModel.AdvTypeName = adv.AdvType.Name;
            viewModel.UserName = adv.Member.UserName;
            return View(viewModel);
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
