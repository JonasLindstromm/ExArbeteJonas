using ExArbeteJonas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class AdsViewModel
    {
        public string PageHeading { get; set; }
        public List<Advertisement> CurrentAds { get; set; }
    }
}
