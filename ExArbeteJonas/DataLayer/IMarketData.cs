using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ExArbeteJonas.Models;

namespace ExArbeteJonas.DataLayer
{
    public interface IMarketData
    {
        List<string> GetAdTypeNames();
        List<string> GetEquipmentTypeNames();
        string CreateAdv(Advertisement adv);
        string CreateEqm(Equipment eqm);
        string CreateAdType(AdType adType);
        string CreateAdvRule(AdvRule advRule);
        string CreateEquipmentType(EquipmentType eqType);        
        string CreateRemovedAdv(RemovedAdv remAdv);
        string CreateRemovedEqm(RemovedEqm remEqm);       
        void DeleteAdv(Advertisement adv);
        void DeleteAdvRule(AdvRule advRule);
        List<Advertisement> GetCurrentAds();       
        List<Equipment> GetCurrentEquipments();
        List<RemovedAdv> GetRemovedAds();
        List<RemovedEqm> GetRemovedEquipments();
        Advertisement GetAdv(int id);
        AdvRule GetAdvRule(int id);
        List<AdType> GetAdvTypes();
        List<EquipmentType> GetEquipmentTypes();        
        List<Equipment> GetEquipment(int advId);
        List<AdvRule> GetRules();
        List<Advertisement> GetUserAds(string UserId);
        bool IsExistingAdv(Advertisement adv);
        bool IsExistingEqm(Equipment eqm);
        string UpdateAdv(Advertisement adv);       
    }
}
