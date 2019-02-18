using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.Concurrency_Timestamp
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class DataAnnotationsContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public DataAnnotationsContext(DbContextOptions<DataAnnotationsContext> options) : base(options) {}
    }
    
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }


    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<DataAnnotationsContext>
    {
        static bool UseSqlServer = true;
        static bool LogToOutput = true;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new DataAnnotationsContext(opt), useSqlServer: UseSqlServer,
                logToOutput: LogToOutput)
        {
        }

        [Fact]
        public async Task Concurrency()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team"
            });
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            myTeam.Name = "Eco Team";
            DbContext.Update(myTeam);
            DbContext.SaveChanges();
        }

        [Fact]
        public async Task Concurrency_Fail()
        {
            // logToOutput => false
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team"
            });
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            var myTeam2 = await SecondDbContext.Teams.FirstAsync();

            myTeam.Name = "Eco Team";
            DbContext.Update(myTeam);
            DbContext.SaveChanges();

            myTeam2.Name = "Eco Team2";
            SecondDbContext.Update(myTeam2);
            SecondDbContext.SaveChanges();
        }

    }
}