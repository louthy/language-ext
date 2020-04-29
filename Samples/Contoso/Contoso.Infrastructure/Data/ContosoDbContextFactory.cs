using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Contoso.Infrastructure.Data
{
    public class ContosoDbContextFactory : IDesignTimeDbContextFactory<ContosoDbContext>
    {
        public ContosoDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();
            string localConnection = "Server=(localdb)\\mssqllocaldb;Database=Contoso;Trusted_Connection=True;Application Name=Contoso;";
            optionsBuilder.UseSqlServer(localConnection);
            return new ContosoDbContext(optionsBuilder.Options);
        }
    }
}
