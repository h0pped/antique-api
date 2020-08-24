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
    }
}
