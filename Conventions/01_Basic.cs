using System;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.Conventions.Basic
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class ConventionsContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }

        public ConventionsContext(DbContextOptions<ConventionsContext> options) : base(options) {}
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }


    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<ConventionsContext>
    {
        static bool UseSqlServer = true;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new ConventionsContext(opt), useSqlServer: UseSqlServer)
        {
        }

        [Fact]
        public void DbScript_SqlServer()
        {
            OutputDbScript();
        }

        [Fact]
        public void DbScript_SqlLite()
        {
            // => UseSqlServer = false;
            OutputDbScript();
        }
    }
}

