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
        string CreateAdType(AdType adType);
        string CreateAdvRule(AdvRule advRule);
        string CreateEquipmentType(EquipmentType eqType);
        void DeleteAdv(Advertisement adv);
        void DeleteAdvRule(AdvRule advRule);
        List<Advertisement> GetCurrentAds();
        List<AdvRule> GetRules();
        List<Equipment> GetCurrentEquipments();
        Advertisement GetAdv(int id);
        AdvRule GetAdvRule(int id);
        List<AdType> GetAdvTypes();
        List<EquipmentType> GetEquipmentTypes();
        string CreateAdv(Advertisement adv);
        string CreateEqm(Equipment eqm);
        List<Equipment> GetEquipment(int advId);
        List<Advertisement> GetUserAds(string UserId);
        bool IsExistingAdv(Advertisement adv);
        bool IsExistingEqm(Equipment eqm);
        string UpdateAdv(Advertisement adv);        
    }
}
