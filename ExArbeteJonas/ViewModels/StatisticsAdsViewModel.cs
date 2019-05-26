using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class StatisticsAdsViewModel
    {
        public string Heading { get; set; }
        public List<string> EqTypeNames { get; set; }
        public List<string> AdTypeNames { get; set; }
        public IDictionary<string, List<int>> Statistics { get; set; }
    }
}

