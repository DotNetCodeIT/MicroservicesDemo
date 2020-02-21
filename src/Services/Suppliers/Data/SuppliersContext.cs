using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Suppliers.Models;

namespace Suppliers.Data
{
    public class SuppliersContext : DbContext
    {
        public SuppliersContext (DbContextOptions<SuppliersContext> options)
            : base(options)
        {
        }

        public DbSet<Suppliers.Models.Supplier> Supplier { get; set; }
    }
}
