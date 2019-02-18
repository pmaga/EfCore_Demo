using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.Concurrency
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class FluentApiContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public FluentApiContext(DbContextOptions<FluentApiContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .Property(p => p.RowVersion)
                .IsRowVersion();

            modelBuilder.Entity<Team>()
                .Property(p => p.Name)
                .IsConcurrencyToken();
        }
    }
    
     public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public byte[] RowVersion { get; set; }
    }

    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<FluentApiContext>
    {
        static bool UseSqlServer = true;
        static bool LogToOutput = true;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new FluentApiContext(opt), useSqlServer: UseSqlServer,
            logToOutput: LogToOutput)
        {
        }

        [Fact]
        public async Task ConcurrencyCheck()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team"
            });
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            myTeam.Name = "Eco Team";
            DbContext.SaveChanges();
        }
    }
}