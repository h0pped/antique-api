using antique_api.Models.Antique;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace antique_api.DBContext
{

    public class CTX : DbContext
    {
        public CTX(DbContextOptions<CTX> options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Order> Order { get; set; }
        public DbSet<OrderItem> Items { get; set; }
    }
}
