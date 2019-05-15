using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class AdType
    {
        public int Id { get; set; }

        [DisplayName("Skriv in namnet på Annonstypen")]
        [Required(ErrorMessage = "Namn på Annonstypen är obligatoriskt")]
        [StringLength(20, ErrorMessage = "Annonstypen får ha max 20 tecken")]
        public string Name { get; set; }

        public virtual ICollection<Advertisement> Advertisements { get; set; }
    }
   
}
