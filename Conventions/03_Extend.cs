using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.Conventions.Extend
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */

    public class ConventionsContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }

        public ConventionsContext(DbContextOptions<ConventionsContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                entityType.Relational().TableName = "Eco" + entityType.DisplayName();

                base.OnModelCreating(modelBuilder);
            }
        }
    }
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Member> Members { get; set; }
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Team Team { get; set; }
    }


    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<ConventionsContext>
    {
        public Tests(ITestOutputHelper output) 
            : base(output, opt => new ConventionsContext(opt), useSqlServer: false)
        {
        }

        [Fact]
        public void Output_Script()
        {
            OutputDbScript();
        }
    }
}

