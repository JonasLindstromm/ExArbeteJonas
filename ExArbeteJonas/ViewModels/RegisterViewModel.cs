using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class RegisterViewModel
    {
        [DisplayName("Skriv in Namn")]
        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [StringLength(40, ErrorMessage = "Förnamn får ha max 40 tecken")]
        public string Namn { get; set; }

        [DisplayName("Skriv in Email")]
        [Required(ErrorMessage = "Email är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Email får ha max 50 tecken")]
        [EmailAddress(ErrorMessage = "Ogiltig emailadress")]
        public string Email { get; set; }      

        [DisplayName("Skriv in telefonnummer")]
        [StringLength(20, ErrorMessage = "Telefonnummer får ha max 20 tecken")]
        [Phone(ErrorMessage = "Ogiltigt telefonnummer")]
        public string Telefon { get; set; }

        [DisplayName("Skriv in Användarnamn")]
        [Required(ErrorMessage = "Användarnamn är obligatoriskt")]
        [StringLength(20, ErrorMessage = "Användarnamn får ha max 20 tecken")]
        public string AnvandarNamn { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Skriv in Lösenord")]
        [Required(ErrorMessage = "Lösenord är obligatoriskt")]
        [StringLength(20, ErrorMessage = "Lösenordet måste ha minst {2} och högst {1} tecken.", MinimumLength = 6)]
        public string Losenord { get; set; }
    }   
    
}
