using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class CreateEqViewModel
    {
        public CreateEqViewModel()
        {
            EqTypeNames = new List<SelectListItem>();
        }

        //Används för model binding till formuläret
        public Equipment CurrentEqm { get; set; }

        public int CurrentAdvId { get; set; }
        public Advertisement CurrentAdv { get; set; }
        public List<Equipment> ExistingEqm { get; set; }

        //Används för att skicka data till vyn
        public List<SelectListItem> EqTypeNames { get; set; }       
    }
}
