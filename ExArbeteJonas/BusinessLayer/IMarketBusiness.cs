﻿using ExArbeteJonas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.BusinessLayer
{
    public interface IMarketBusiness
    {
        string CreateAdv(Advertisement adv);
        string CreateAdType(AdType adType);        
        string CreateEquipmentType(EquipmentType eqType);
        void DeleteAdv(Advertisement adv);
        List<Advertisement> GetCurrentAds();
        Advertisement GetAdv(int id);
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
    }
}