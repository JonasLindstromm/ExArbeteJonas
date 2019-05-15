using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class Equipment
    {
        public int Id { get; set; }

        public int ActualAdId { get; set; }

        [DisplayName("Ange Utrustningstyp")]
        [Required(ErrorMessage = "Utrustningstyp är obligatoriskt")]
        public int EqTypeId { get; set; }

        [DisplayName("Skriv in Märke")]
        [Required(ErrorMessage = "Märke är obligatoriskt")]
        [StringLength(25, ErrorMessage = "Märket får ha max 25 tecken")]
        public string Make { get; set; }

        [DisplayName("Skriv in Modell, ej obligatoriskt")]
        [StringLength(25, ErrorMessage = "Modellen får ha max 25 tecken")]
        public string Model { get; set; }

        [DisplayName("Skriv in Storlek, ej obligatoriskt")]
        [StringLength(10, ErrorMessage = "Storleken får ha max 10 tecken")]
        public string Size { get; set; }

        [DisplayName("Skriv in Längd i cm, ej obligatoriskt")]
        [StringLength(10, ErrorMessage = "Längden får ha max 10 tecken")]
        public string Length { get; set; }

        public virtual Advertisement ActualAd { get; set; }
        public virtual EquipmentType EqType { get; set; }
    }
}
