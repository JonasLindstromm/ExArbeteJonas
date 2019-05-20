using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class AdvRule
    {
        public int Id { get; set; }

        [DisplayName("Skriv in en rubrik för Regeln")]
        [Required(ErrorMessage = "Rubrik är obligatoriskt")]
        [StringLength(30, ErrorMessage = "Rubriken får ha max 30 tecken")]
        public string Title { get; set; }

        [DisplayName("Skriv in en beskrivning av Regeln")]
        [Required(ErrorMessage = "Beskrivning är obligatoriskt")]
        [StringLength(150, ErrorMessage = "Beskrivningen får ha max 150 tecken")]
        public string Description { get; set; }
    }
}
