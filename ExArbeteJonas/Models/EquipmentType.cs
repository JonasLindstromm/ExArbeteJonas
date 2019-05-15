using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class EquipmentType
    {
        public int Id { get; set; }

        [DisplayName("Skriv in namnet på Utrustningstypen")]
        [Required(ErrorMessage = "Namn på Utrustningstypen är obligatoriskt")]
        [StringLength(20, ErrorMessage = "Utrustningstypen får ha max 20 tecken")]
        public string Name { get; set; }

        public virtual ICollection<Equipment> Equipments { get; set; }
    }    
}
