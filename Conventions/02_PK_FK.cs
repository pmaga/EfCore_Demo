using System.Collections.Generic;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.Conventions.PK_FK
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

        [Fact]
        public async Task Result_Async()
        {
            Seed();

            var me = await DbContext.Members.FirstAsync();
            DumpObject(me);

            Separator();

            var myTeam = await DbContext.Teams.FirstAsync();
            DumpObject(myTeam);
        }

        private void Seed()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team" ,
                Members = new[] {
                    new Member { FirstName = "Pawel", LastName = "Maga" }
            }});
            DbContext.SaveChanges();
        }
    }

