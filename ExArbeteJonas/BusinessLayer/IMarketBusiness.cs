using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.BusinessLayer
{
    public interface IMarketBusiness
    {
        string CreateAdv(Advertisement adv);
        string CreateAdvRule(AdvRule advRule);
        string CreateAdType(AdType adType);
        string CreateEquipmentType(EquipmentType eqType);
        void DeleteAdv(Advertisement adv);
        void DeleteAdvRule(AdvRule advRule);
        void DeleteMemberAds(string memberId);
        List<Advertisement> GetCurrentAds();
        List<AdvRule> GetRules();
        Advertisement GetAdv(int id);
        AdvRule GetAdvRule(int id);
        List<string> GetAdvTypeNames();
        List<string> GetEquipmentTypeNames();
        List<AdType> GetAdvTypes();
        List<EquipmentType> GetEquipmentTypes();
        List<Equipment> GetEquipment(int AdvId);
        List<Advertisement> GetUserAds(string UserId);
        void DeleteOldAds();
        string CreateEqm(Equipment eqm);
        List<Advertisement> SearchCurrentAds(int advTypeId, int eqTypeId, string searchTitle, string searchDescription, string searchPlace);
        void SendEmail(string mailSubject, string mailText, string receiver);
        string UpdateAdv(Advertisement adv);
        IDictionary<string, List<int>> GetAgeAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames);
        IDictionary<string, List<int>> GetNrAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames);
        IDictionary<string, List<int>> GetNrDeletedAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames);
        IDictionary<string, List<int>> GetPriceAdsStatistics(List<string> eqTypeNames, List<string> adTypeNames);
        string SaveImage(IFormFile imageFile);
    }
}
