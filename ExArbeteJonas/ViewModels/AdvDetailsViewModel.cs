using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ExArbeteJonas.IdentityData;
using ExArbeteJonas.Models;

namespace ExArbeteJonas.ViewModels
{
    public class AdvDetailsViewModel
    {
        public int AdvId { get; set; }
        public AdType AdvType { get; set; }
        public ApplicationUser Member { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public string Place { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        public List<Equipment> Equipments { get; set; }
    }
}
