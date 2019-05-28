using ExArbeteJonas.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.DataLayer
{
    public class MarketData : IMarketData
    {
        private MarketContext _context;

        // Dependency Injection av Contextklassen via konstruktorn,'
        // för att kunna göra anrop mot databasen 
        public MarketData(MarketContext context)
        {
            _context = context;
        }

        // Skapa en Annonstyp
        public string CreateAdType(AdType adType)
        {
            _context.Add(adType);
            _context.SaveChanges();
            return "OK";
        }

        // Skapa en Annons
        public string CreateAdv(Advertisement adv)
        {
            _context.Add(adv);
            _context.SaveChanges();
            return "OK";
        }

        // Spara en borttagen annons i Databasen  
        public string CreateRemovedAdv(RemovedAdv remAdv)
        {
            _context.Add(remAdv);
            _context.SaveChanges();
            return "OK";
        }

        // Spara en borttagen utrustning i Databasen  
        public string CreateRemovedEqm(RemovedEqm remEqm)
        {
            _context.Add(remEqm);
            _context.SaveChanges();
            return "OK";
        }

        // Skapa en regel för annonsering
        public string CreateAdvRule(AdvRule advRule)
        {            
            _context.Add(advRule);
            _context.SaveChanges();            
            return "OK";
        }

        // Skapa en ny utrustning 
        public string CreateEqm(Equipment eqm)
        {
            _context.Add(eqm);
            _context.SaveChanges();
            return "OK";
        }

        // Skapa en ny Utrustningstyp
        public string CreateEquipmentType(EquipmentType eqType)
        {
            _context.Add(eqType);
            _context.SaveChanges();
            return "OK";
        }

        // Ta bort en annons
        public void DeleteAdv(Advertisement adv)
        {
            _context.Advertisement.Remove(adv);
            _context.SaveChanges();
        }

        // Ta bort en regel för annonsering
        public void DeleteAdvRule(AdvRule advRule)
        {
            _context.AdvRule.Remove(advRule);
            _context.SaveChanges();
        }

        // Läs en annons
        public Advertisement GetAdv(int id)
        {
            return _context.Advertisement
              .Include(a => a.AdvType)
              .Include(a => a.Member)
              .SingleOrDefault(m => m.Id == id);
        }

        // Läs en regel för annonsering
        public AdvRule GetAdvRule(int id)
        {           
            return _context.AdvRule              
              .SingleOrDefault(m => m.Id == id);   
        }

        // Läs namnen på alla existerande Annonstyper
        public List<string> GetAdTypeNames()
        {
            // Sortera Advtypes i Namnordning
            var advTypes = _context.AdType.OrderBy(p => p.Name);
            return advTypes.Select(e => e.Name).ToList();
        }

        // Läs alla Annonstyper
        public List<AdType> GetAdvTypes()
        {
            // Sortera Advtypes i Namnordning
            var advTypes = _context.AdType.OrderBy(p => p.Name);
            return advTypes.ToList();
        }

        // Läs alla Utrustningstyper
        public List<EquipmentType> GetEquipmentTypes()
        {
            // Sortera Advtypes i Namnordning
            var eqTypes = _context.EquipmentType.OrderBy(p => p.Name);
            return eqTypes.ToList();
        }

        // Läs alla annonser
        public List<Advertisement> GetCurrentAds()
        {
            var allAds = _context.Advertisement.Include(a => a.AdvType)
                .Include(a => a.Equipments).Include(a => a.Member);

            // Sortera annonserna i tidsordning                    
            return allAds.OrderBy(p => p.StartDate).ToList();
        }

        // Läs alla Utrustningar
        public List<Equipment> GetCurrentEquipments()
        {
            return _context.Equipment.Include(e => e.EqType).Include(e => e.ActualAd.AdvType).ToList();
        }

        // Läs alla borttagna annonser
        public List<RemovedAdv> GetRemovedAds()
        {
            var allAds = _context.RemovedAdv.Include(a => a.AdvType)
                .Include(a => a.RemovedEqms);

            // Sortera annonserna i tidsordning                    
            return allAds.OrderBy(p => p.StartDate).ToList();
        }

        // Läs alla borttagna Utrustningar
        public List<RemovedEqm> GetRemovedEquipments()
        {
            return _context.RemovedEqm.Include(e => e.EqType).Include(e => e.RemovedAd.AdvType).ToList();
        }

        // Läs alla regler för Annonsering
        public List<AdvRule> GetRules()
        {
            return _context.AdvRule.ToList();            
        }

        // Läs utrustning för en viss annons
        public List<Equipment> GetEquipment(int advId)
        {        
            var equipment = _context.Equipment.Include(e => e.EqType)
                .Where(e => e.ActualAdId == advId);

            if (equipment.Count() > 0)
            {
                return equipment.OrderBy(e => e.EqType.Name).ToList();
            }
            else
            {
                return new List<Equipment>();
            }
        }

        // Läs namnen på existerande Utrustningstyper
        public List<string> GetEquipmentTypeNames()
        {
            // Sortera EquiomentTypes i Namnordning
            var eqTypes = _context.EquipmentType.OrderBy(e => e.Name);
            return eqTypes.Select(e => e.Name).ToList();
        }

        // Läs alla annonser som en viss användare har lagt in
        public List<Advertisement> GetUserAds(string UserId)
        {
            var allAds = _context.Advertisement.Include(a => a.AdvType)
                 .Include(a => a.Equipments).Where(a => a.MemberId == UserId);

            // Sortera annonserna i tidsordning                    
            return allAds.OrderBy(p => p.StartDate).ToList();
        }

        // Svara på om en viss annons redan finns i databasen
        public bool IsExistingAdv(Advertisement adv)
        {
            var existingAds = _context.Advertisement.Include(a => a.AdvType).
             Where(a => (a.Title == adv.Title && a.AdvTypeId == adv.AdvTypeId));
            if (existingAds.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Svara på om en viss utrustning redan finns i databasen
        public bool IsExistingEqm(Equipment eqm)
        {
            var existingEqms = _context.Equipment.Include(e => e.EqType).
              Where(e => (e.ActualAdId == eqm.ActualAdId && e.EqTypeId == eqm.EqTypeId));

            if (existingEqms.ToList().Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // Uppdatera en annons
        public string UpdateAdv(Advertisement adv)
        {           
            try
            {
                _context.Update(adv);
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return "Uppdatering av annonsen misslyckades";
            }
            return "OK";
        }
    }
}
