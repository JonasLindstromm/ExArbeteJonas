﻿using ExArbeteJonas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.ViewModels
{
    public class AdminViewModel
    {
        public List<string> AdTypeNames { get; set; }
        public List<string> EquipmentTypeNames { get; set; }

        public List<AdvRule> CurrentRules { get; set; }
    }
}
