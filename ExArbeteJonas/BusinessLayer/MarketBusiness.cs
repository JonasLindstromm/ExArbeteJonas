using ExArbeteJonas.DataLayer;
using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ExArbeteJonas.BusinessLayer
{
    public class MarketBusiness : IMarketBusiness
    {
        private IMarketData _marketData;
        private IHostingEnvironment _environment;
        private readonly IConfiguration _config;

        //Dependency Injection via konstruktorn
        public MarketBusiness(IMarketData marketData, IHostingEnvironment environment, IConfiguration config)
        {
            _marketData = marketData;
            _environment = environment;
            _config = config;
        }

        // Skapa en Annonstyp
        public string CreateAdType(AdType adType)
        {
            var currentNames = _marketData.GetAdTypeNames();

            if (currentNames.Contains(adType.Name))
            {
                return ("Denna Annonstyp finns redan");
            }

            return _marketData.CreateAdType(adType);
        }

        // Skapa en ny annons
        public string CreateAdv(Advertisement adv)
        {
            // Det ska inte vara tillåtet att lägga in samma annons flera gånger 
            if (_marketData.IsExistingAdv(adv))
            {
                return "Denna annons finns redan inlagd";
            }

            return _marketData.CreateAdv(adv);
        }

        // Skapa en ny Regel för annonsering
        public string CreateAdvRule(AdvRule advRule)
        {          
            return _marketData.CreateAdvRule(advRule);
        }

        // Skapa ny Utrustning till en annons
        public string CreateEqm(Equipment eqm)
        {
            // Det ska inte vara tillåtet att lägga in samma utrustning flera gånger 
            if (_marketData.IsExistingEqm(eqm))
            {
                return "Denna utrustning finns redan inlagd";
            }

            return _marketData.CreateEqm(eqm);
        }

        // Skapa en ny Utrustningstyp
        public string CreateEquipmentType(EquipmentType eqType)
        {
            var currentNames = _marketData.GetEquipmentTypeNames();

            if (currentNames.Contains(eqType.Name))
            {
                return ("Denna Utrustningstyp finns redan");
            }

            return _marketData.CreateEquipmentType(eqType);
        }

        // Ta bort en annons
        public void DeleteAdv(Advertisement adv)
        {
            _marketData.DeleteAdv(adv);
        }

        // Ta bort en regel för Annonsering
        public void DeleteAdvRule(AdvRule advRule)
        {
            _marketData.DeleteAdvRule(advRule);
        }

        // Ta bort alla annonser, som lagts in av en viss Medlem
        public void DeleteMemberAds(string memberId)
        {
            var memberAds = _marketData.GetCurrentAds().Where(a => a.MemberId == memberId);

            foreach (var adv in memberAds)
            {
                _marketData.DeleteAdv(adv);
            }
        }

        // Ta bort alla annonser, som är äldre än 1 månad
        public void DeleteOldAds()
        {
            var currentAds = _marketData.GetCurrentAds();

            var oldAds = currentAds.Where(a => (a.StartDate < DateTime.Now.AddMonths(-1)));
            foreach (var adv in oldAds)
            {
                _marketData.DeleteAdv(adv);
            }
        }

        // Läs data för en annons
        public Advertisement GetAdv(int id)
        {
            return _marketData.GetAdv(id);
        }

        // Läs en regel för annonsering
        public AdvRule GetAdvRule(int id)
        {
            return _marketData.GetAdvRule(id);
        }

        // Läs namnen på existerande Annonstyper
        public List<string> GetAdvTypeNames()
        {
            return _marketData.GetAdTypeNames();
        }

        // Läs existerande Annonstyper
        public List<AdType> GetAdvTypes()
        {
            return _marketData.GetAdvTypes();
        }

        // Läs alla aktuella annonser
        public List<Advertisement> GetCurrentAds()
        {
            return _marketData.GetCurrentAds();
        }

        public List<AdvRule> GetRules()
        {
            return _marketData.GetRules();            
        }

        // Läs all utrustning som är kopplad till en viss annons 
        public List<Equipment> GetEquipment(int AdvId)
        {
            return _marketData.GetEquipment(AdvId);
        }

        // Läs namnen på existerande Utrustningstyper
        public List<string> GetEquipmentTypeNames()
        {
            return _marketData.GetEquipmentTypeNames();
        }

        // Läs existerande Utrustningstyper
        public List<EquipmentType> GetEquipmentTypes()
        {
            return _marketData.GetEquipmentTypes();
        }

        // Ta fram statistik över antalet dagar för annonser av olika typer
        public IDictionary<string, List<int>> GetAgeAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames)
        {
            var statistics = new Dictionary<string, List<int>>();

            var currentAds = _marketData.GetCurrentAds();
            var currentEquipments = _marketData.GetCurrentEquipments();

            var eqTypeStatistics = new List<int>();

            // Räkna antalet dagar för annonser där utrustningen är ospecificerad
            foreach (var adTypeName in adTypeNames)
            {
                var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                    .Where(a => a.Equipments.Count == 0);
                eqTypeStatistics.Add(CalculateAverageNrDays(ads));
            }
            statistics.Add("Odefinerade", eqTypeStatistics);

            // Räkna antalet dagar för annonser där utrustningen är specificerad
            foreach (var eqTypeName in eqTypeNames)
            {
                eqTypeStatistics = new List<int>();

                var equipments = currentEquipments.Where(e => e.EqType.Name == eqTypeName);

                foreach (var adTypeName in adTypeNames)
                {
                    var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                        .Where(a => a.Equipments.Count == 1)
                        .Where(a => a.Equipments.First().EqType.Name == eqTypeName);
                    eqTypeStatistics.Add(CalculateAverageNrDays(ads));
                }
                statistics.Add(eqTypeName, eqTypeStatistics);
            }

            eqTypeStatistics = new List<int>();
            // Räkna antalet dagar för annonser där utrustningen är ett paket
            foreach (var adTypeName in adTypeNames)
            {
                var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                   .Where(a => a.Equipments.Count == 2);
                eqTypeStatistics.Add(CalculateAverageNrDays(ads));
            }
            statistics.Add("Paket", eqTypeStatistics);


            return statistics;
        }

        // Ta fram statistik över angivet pris för annonser av olika typer
        public IDictionary<string, List<int>> GetPriceAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames)
        {
            var statistics = new Dictionary<string, List<int>>();

            var currentAds = _marketData.GetCurrentAds();
            var currentEquipments = _marketData.GetCurrentEquipments();

            var eqTypeStatistics = new List<int>();

            // Räkna ut genomsnittspriset för annonser där utrustningen är ospecificerad
            foreach (var adTypeName in adTypeNames)
            {
                var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                    .Where(a => a.Equipments.Count == 0);
                eqTypeStatistics.Add(CalculateAveragePrice(ads));
            }
            statistics.Add("Odefinerade", eqTypeStatistics);

            // Räkna ut genomsnittspriset för annonser där utrustningen är specificerad
            foreach (var eqTypeName in eqTypeNames)
            {
                eqTypeStatistics = new List<int>();

                var equipments = currentEquipments.Where(e => e.EqType.Name == eqTypeName);

                foreach (var adTypeName in adTypeNames)
                {
                    var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                        .Where(a => a.Equipments.Count == 1)
                        .Where(a => a.Equipments.First().EqType.Name == eqTypeName);
                    eqTypeStatistics.Add(CalculateAveragePrice(ads));
                }
                statistics.Add(eqTypeName, eqTypeStatistics);
            }

            eqTypeStatistics = new List<int>();
            // Räkna ut genomsnittspriset för annonser där utrustningen är ett paket
            foreach (var adTypeName in adTypeNames)
            {
                var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                   .Where(a => a.Equipments.Count == 2);
                eqTypeStatistics.Add(CalculateAveragePrice(ads));
            }
            statistics.Add("Paket", eqTypeStatistics);


            return statistics;
        }

        // Ta fram statistik över antalet annonser av olika typer
        public IDictionary<string, List<int>> GetNrAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames)
        {
            var statistics = new Dictionary<string, List<int>>();

            var currentAds = _marketData.GetCurrentAds();
            var currentEquipments = _marketData.GetCurrentEquipments();

            var eqTypeStatistics = new List<int>();

            // Räkna antalet annonser där utrustningen är ospecificerad
            foreach (var adTypeName in adTypeNames)
            {
                int count = currentAds.Where(a => a.AdvType.Name == adTypeName)
                    .Where(a => a.Equipments.Count == 0).Count();
                eqTypeStatistics.Add(count);
            }
            statistics.Add("Odefinerade", eqTypeStatistics);

            // Räkna antalet annonser där en utrustning är specificerad
            foreach (var eqTypeName in eqTypeNames)
            {
                eqTypeStatistics = new List<int>();

                foreach (var adTypeName in adTypeNames)
                {                    
                    var ads = currentAds.Where(a => a.AdvType.Name == adTypeName)
                       .Where(a => a.Equipments.Count == 1)
                       .Where(a => a.Equipments.First().EqType.Name == eqTypeName);
                  
                    eqTypeStatistics.Add(ads.Count());
                }

                statistics.Add(eqTypeName, eqTypeStatistics);
            }

            eqTypeStatistics = new List<int>();

            // Räkna antalet annonser där utrustningen är ett paket
            foreach (var adTypeName in adTypeNames)
            {
                int count = currentAds.Where(a => a.AdvType.Name == adTypeName)
                    .Where(a => a.Equipments.Count == 2).Count();
                eqTypeStatistics.Add(count);
            }
            statistics.Add("Paket", eqTypeStatistics);

            return statistics;
        }

        // Läs alla annonser som en viss användare har lagt in
        public List<Advertisement> GetUserAds(string UserId)
        {
            return _marketData.GetUserAds(UserId);
        }

        // Sök bland aktuella Annonser
        public List<Advertisement> SearchCurrentAds(int advTypeId, int eqTypeId, string searchTitle, string searchDescription, string searchPlace)
        {
            var currentAds = _marketData.GetCurrentAds();
            List<Advertisement> searchedAds = new List<Advertisement>();

            // Filtrera på Utrustningstyp
            if (eqTypeId != 0)
            {
                foreach (var ad in currentAds)
                {
                    if (_marketData.GetEquipment(ad.Id).Where(e => e.EqTypeId == eqTypeId).Count() > 0)
                        searchedAds.Add(ad);
                }
            }
            else
            {
                searchedAds = currentAds;
            }

            // Filtrera på Annonstyp
            if (advTypeId != 0)
            {
                searchedAds = searchedAds.Where(a => a.AdvTypeId == advTypeId).ToList();
            }

            // Filtrera på sökord i Titeln
            if (!String.IsNullOrEmpty(searchTitle))
            {
                searchedAds = searchedAds.Where(p => p.Title.Contains(searchTitle)).ToList();
            }

            // Filtrera på sökord i Beskrivningen
            if (!String.IsNullOrEmpty(searchDescription))
            {
                searchedAds = searchedAds.Where(p => p.Description.Contains(searchDescription)).ToList();
            }

            // Filtrera på Plats
            if (!String.IsNullOrEmpty(searchPlace))
            {
                searchedAds = searchedAds.Where(p => p.Place.Equals(searchPlace)).ToList();
            }

            return searchedAds;
        }

        // Skicka Email
        public void SendEmail(string mailSubject, string mailText, string receiver)
        {
            SmtpClient client;
            string host = _config.GetValue<String>("Email:Smtp:Host");
            int port = _config.GetValue<int>("Email:Smtp:Port");

            if (_environment.IsProduction())
            {
                client = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    Credentials = new NetworkCredential(
                     _config.GetValue<String>("Email:Smtp:ProdUsername"),
                     _config.GetValue<String>("Email:Smtp:ProdPassword")
                 ),
                    EnableSsl = true
                };
            }
            else
            {
                client = new SmtpClient()
                {
                    Host = host,
                    Port = port,
                    Credentials = new NetworkCredential(
                        _config.GetValue<String>("Email:Smtp:Username"),
                        _config.GetValue<String>("Email:Smtp:Password")
                    ),
                    EnableSsl = true
                };
            }

            string sender = _config.GetValue<string>("SiteName") + "@nackademin.se";

            // from, to, subject, text
            client.Send(sender, receiver, mailSubject, mailText);
        }

        public string UpdateAdv(Advertisement adv)
        {
            return _marketData.UpdateAdv(adv);
        }

        // Beräkna genomsnittligt antal dagar för annonser
        private int CalculateAverageNrDays(IEnumerable<Advertisement> ads)
        {
            if (ads.Count() == 0)
            {
                return 0;
            }
            int counter = 0;
            int sum = 0;
            foreach (var ad in ads)
            {
                counter++;
                sum += (DateTime.Now - ad.StartDate).Days;
            }

            return sum / counter;
        }

        // Beräkna genomsnittligt pris för annonser
        private int CalculateAveragePrice(IEnumerable<Advertisement> ads)
        {
            if (ads.Count() == 0)
            {
                return 0;
            }
            int counter = 0;
            int sum = 0;
            foreach (var ad in ads)
            {
                counter++;
                sum += ad.Price;
            }

            return sum / counter;
        }
    }
}
