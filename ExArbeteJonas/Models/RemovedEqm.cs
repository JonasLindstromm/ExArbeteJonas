using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class RemovedEqm
    {
        public int Id { get; set; }

        public int RemovedAdId { get; set; }
       
        [Required]
        public int EqTypeId { get; set; }

        [Required]       
        public string Make { get; set; }
       
        public string Model { get; set; }
       
        public string Size { get; set; }
       
        public string Length { get; set; }

        public virtual RemovedAdv RemovedAd { get; set; }
        public virtual EquipmentType EqType { get; set; }
    }
}
