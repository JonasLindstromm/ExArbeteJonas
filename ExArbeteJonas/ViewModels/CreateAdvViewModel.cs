using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class CreateAdvViewModel
    {
        public CreateAdvViewModel()
        {
            AdvTypeNames = new List<SelectListItem>();
        }

        //Används för model binding till formuläret
        public Advertisement CurrentAdv { get; set; }

        [DisplayName("Ladda upp en bild om du vill")]
        public IFormFile MyImage { get; set; }

        //Används för att skicka data till vyn
        public List<SelectListItem> AdvTypeNames { get; set; }
    }
}
