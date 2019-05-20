using ExArbeteJonas.IdentityData;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ExArbeteJonas.Models
{
    public class MarketContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<AdType> AdType { get; set; }
        public virtual DbSet<Advertisement> Advertisement { get; set; }
        public virtual DbSet<AdvRule> AdvRule { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<EquipmentType> EquipmentType { get; set; }       

        public MarketContext(DbContextOptions<MarketContext> options)
          : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

    }
}
