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
        string CreateEquipmentType(EquipmentType eqType);
        void DeleteAdv(Advertisement adv);
        List<Advertisement> GetCurrentAds();
        Advertisement GetAdv(int id);
        List<AdType> GetAdvTypes();
        List<EquipmentType> GetEquipmentTypes();
        string CreateAdv(Advertisement adv);
        string CreateEqm(Equipment eqm);
        List<Equipment> GetEquipment(int advId);
        List<Advertisement> GetUserAds(string UserId);
        bool IsExistingAdv(Advertisement adv);
        bool IsExistingEqm(Equipment eqm);      
    }
}
