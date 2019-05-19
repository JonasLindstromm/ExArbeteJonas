using ExArbeteJonas.IdentityData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class Advertisement
    {        
        public int Id { get; set; }

        public string MemberId { get; set; }

        [DisplayName("Ange Annonstyp")]
        [Required(ErrorMessage = "Annonstyp är obligatoriskt")]
        public int AdvTypeId { get; set; }

        [DisplayName("Skriv in en rubrik för Annonsen")]
        [Required(ErrorMessage = "Rubrik är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Rubriken får ha max 50 tecken")]
        public string Title { get; set; }

        [DisplayName("Skriv in en Annonsbeskrivning")]
        [Required(ErrorMessage = "Beskrivning är obligatoriskt")]
        [StringLength(500, ErrorMessage = "Beskrivningen får ha max 500 tecken")]
        public string Description { get; set; }

        [DisplayName("Skriv in prisförslag i Kronor")]
        [Required(ErrorMessage = "Prisförslag är obligatoriskt")]
        [Range(1, int.MaxValue, ErrorMessage = "Priset måste vara högre än 0 Kr")]
        public int Price { get; set; }

        [DisplayName("Skriv in Plats")]
        [Required(ErrorMessage = "Plats är obligatoriskt")]
        [StringLength(25, ErrorMessage = "Platsen får ha max 25 tecken")]
        public string Place { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        public string ImageFileName { get; set; }

        public virtual ApplicationUser Member { get; set; }
        public virtual AdType AdvType { get; set; }
        public ICollection<Equipment> Equipments { get; set; }
    }
}
