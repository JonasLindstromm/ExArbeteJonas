using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class RemovedAdv
    {
        public int Id { get; set; }
        public string MemberName { get; set; }

        [Required]
        public int AdvTypeId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Price { get; set; }

        [Required]
        public string Place { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public string ImageFileName { get; set; }

        public virtual AdType AdvType { get; set; }
        public ICollection<RemovedEqm> RemovedEqms { get; set; }        
    }
}
