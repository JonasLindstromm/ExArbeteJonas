using ExArbeteJonas.DataLayer;
using ExArbeteJonas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.BusinessLayer
{
    public class MarketBusiness : IMarketBusiness
    {
        private IMarketData _marketData;

        //Dependency Injection via konstruktorn
        public MarketBusiness(IMarketData marketData)
        {
            _marketData = marketData;
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

        // Ta bort gamla annonser, som är äldre än 1 månad
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
    }
}
