using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.Basic
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

        public FluentApiContext(DbContextOptions<FluentApiContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .Property(t => t.Name)
                .HasDefaultValue(Guid.NewGuid().ToString("N"))
                .IsRequired();

            modelBuilder.Entity<Team>()
                .Property(t => t.Created)
                .HasDefaultValue(DateTime.UtcNow)
                .IsRequired();

                
            modelBuilder.Entity<Member>()
                .Property(t => t.FirstName)
                .HasColumnType("nvarchar(200)")
                .IsRequired();

            
            modelBuilder.Entity<Member>()
                .HasOne(m => m.MyTeam)
                .WithMany(t => t.Members);
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
        public Team MyTeam { get; set; }
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
        public void Output_Script()
        {
            OutputDbScript();
        }


    }
}