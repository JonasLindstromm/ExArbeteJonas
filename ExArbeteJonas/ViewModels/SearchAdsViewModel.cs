using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class SearchAdsViewModel
    {
        public SearchAdsViewModel()
        {
            AdvTypeNames = new List<SelectListItem>();
            EqTypeNames = new List<SelectListItem>();
        }      

        //Används för att skicka data till vyn
        public List<SelectListItem> AdvTypeNames { get; set; }
        public List<SelectListItem> EqTypeNames { get; set; }

        [DisplayName("Annonstyp")]      
        public int AdvTypeId { get; set; }

        [DisplayName("Utrustningstyp")]
        public int EqTypeId { get; set; }

        [DisplayName("Sökord i Annonsrubriken")]
        public string SearchTitle { get; set; }

        [DisplayName("Sökord i Beskrivningen")]
        public string SearchDescription { get; set; }

        [DisplayName("Plats")]
        public string SearchPlace { get; set; }
    }
}
