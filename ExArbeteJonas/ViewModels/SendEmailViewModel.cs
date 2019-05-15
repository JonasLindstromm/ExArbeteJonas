using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class SendEmailViewModel
    {
        [DisplayName("Skriv in Meddelandet")]
        [Required(ErrorMessage = "Meddelandet är obligatoriskt")]
        [StringLength(500, ErrorMessage = "Beskrivningen får ha max 500 tecken")]
        public string Message { get; set; }

        [DisplayName("Skriv in Ämne för Meddelandet")]
        [Required(ErrorMessage = "Ämne är obligatoriskt")]
        [StringLength(500, ErrorMessage = "Ämnet får ha max 50 tecken")]
        public string Subject { get; set; }

        public int AdvId { get; set; }
        public string AdvTitle { get; set; }
        public string AdvTypeName { get; set; }
        public string UserName { get; set; }
    }
}
