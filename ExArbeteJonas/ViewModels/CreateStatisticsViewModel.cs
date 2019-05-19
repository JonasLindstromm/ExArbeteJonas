using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class CreateStatisticsViewModel
    {
        public CreateStatisticsViewModel()
        {
            StatisticsTypes = new List<SelectListItem>();
        }
        
        [Required]
        [DisplayName("Ange önskad typ av statistik")]
        public int TypeId { get; set; }
       
       
        //Används för att skicka data till vyn
        public List<SelectListItem> StatisticsTypes { get; set; }
    }
}
