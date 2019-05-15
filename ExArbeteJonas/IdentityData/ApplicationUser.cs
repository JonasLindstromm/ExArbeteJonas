using ExArbeteJonas.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.IdentityData
{
    public class ApplicationUser : IdentityUser
    {
        public string Name { get; set; }
        public ICollection<Advertisement> ActualAds { get; set; }
    }
}
