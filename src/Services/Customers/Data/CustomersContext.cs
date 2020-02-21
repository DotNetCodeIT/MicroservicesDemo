using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Customers.Models;

namespace Customers.Data
{
    public class CustomersContext : DbContext
    {
        public CustomersContext (DbContextOptions<CustomersContext> options)
            : base(options)
        {
        }

        public DbSet<Customers.Models.Customer> Customer { get; set; }
    }
}
