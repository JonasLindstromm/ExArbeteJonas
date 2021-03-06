﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }       
        public string UserName { get; set; }

        [DisplayName("Skriv in Namn")]
        [Required(ErrorMessage = "Namn är obligatoriskt")]
        [StringLength(40, ErrorMessage = "Förnamn får ha max 40 tecken")]
        public string Name { get; set; }

        [DisplayName("Skriv in Email")]
        [Required(ErrorMessage = "Email är obligatoriskt")]
        [StringLength(50, ErrorMessage = "Email får ha max 50 tecken")]
        [EmailAddress(ErrorMessage = "Ogiltig emailadress")]
        public string Email { get; set; }

        [DisplayName("Skriv in Telefonnummer")]
        [StringLength(20, ErrorMessage = "Telefonnummer får ha max 20 tecken")]
        [Phone(ErrorMessage = "Ogiltigt telefonnummer")]
        public string Phone { get; set; }      
    }
}
