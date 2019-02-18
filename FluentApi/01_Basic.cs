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
        [Column("First_Name", Order = 1, TypeName="varchar(20)")]
        public string FirstName { get; set; }
        [Column("Last_Name", Order = 2), StringLength(200, MinimumLength = 5)]
        public string LastName { get; set; }
        [Required]
        public Team Team { get; set; }
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

/*

 */