using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.Views
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class FluentApiContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbQuery<TeamsView> TeamsView { get; set; }

        public FluentApiContext(DbContextOptions<FluentApiContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
               modelBuilder
                    .Query<TeamsView>().ToView("MyView");
        }
    }
    
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public DateTime Created { get; set; } 

        public ICollection<Member> Members { get; set; }
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Team Team { get; set; }
    }

    public class TeamsView
    {
        public static string SQL =  @"CREATE VIEW MyView AS 
        SELECT t.Name, Count(m.Id) as MembersCount 
        FROM Teams t
        JOIN Members m on m.TeamId = t.Id
        GROUP BY t.Name";

        public string Name { get; set; }
        public int MembersCount { get; set; }
    }
    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<FluentApiContext>
    {
        static bool UseSqlServer = true;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new FluentApiContext(opt), useSqlServer: UseSqlServer)
        {
        }

        [Fact]
        public async Task Test()
        {
            DbContext.Database.ExecuteSqlCommand(TeamsView.SQL);

            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team",
                Members = new List<Member>
                {
                    new Member
                    {
                        FirstName = "P1",
                        LastName = "L1"
                    },
                    new Member
                    {
                        FirstName = "P2",
                        LastName = "L2"
                    }
                }
            });
            DbContext.SaveChanges();
            
            var view = await DbContext.TeamsView.ToListAsync();

            DumpObject(view);
        }
    }
}