using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Marketing.Models;

namespace Marketing.Data
{
    public class MarketingContext : DbContext
    {
        public MarketingContext (DbContextOptions<MarketingContext> options)
            : base(options)
        {
        }

        public DbSet<Marketing.Models.ProductPrice> ProductPrice { get; set; }
    }
}
