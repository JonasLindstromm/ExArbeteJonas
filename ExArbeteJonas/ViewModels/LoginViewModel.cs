using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class LoginViewModel
    {
        [DisplayName("Skriv in ditt användarnamn")]
        [Required(ErrorMessage = "Användarnamn är obligatoriskt")]
        public string AnvandarNamn { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Skriv in ditt lösenord")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt")]
        public string Losenord { get; set; }
    }   
}
