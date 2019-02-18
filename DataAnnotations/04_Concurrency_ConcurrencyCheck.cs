using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.Concurrency_ConcurrencyCheck
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class DataAnnotationsContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DataAnnotationsContext(DbContextOptions<DataAnnotationsContext> options) : base(options) {}
    }

    public class Member
    {
        public int Id { get; set; }
        [ConcurrencyCheck]
        public string FirstName { get; set; }
        [ConcurrencyCheck]
        public string LastName { get; set; }
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
        public async Task ConcurrencyCheck()
        {
            DbContext.Members.Add(new Member 
            { 
                FirstName = "Pawel",
                LastName = "Maga"
            });
            DbContext.SaveChanges();

            var me = await DbContext.Members.FirstAsync();
            me.FirstName = "Paul";
            DbContext.SaveChanges();
        }
    }
}