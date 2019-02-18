using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.PK_FK
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class DataAnnotationsContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Member> Members { get; set; }

        public DataAnnotationsContext(DbContextOptions<DataAnnotationsContext> options) : base(options) {}
    }
    
    public class Team
    {
        [Key]
        public int EcoId { get; set; }
        //[Key]
        public string Name { get; set; }

        public ICollection<Member> Members { get; set; }
    }

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Team Team { get; set; }

        [ForeignKey(nameof(Team))]
        public int MyTeamId { get; set; }
    }


    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<DataAnnotationsContext>
    {
        static bool UseSqlServer = false;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new DataAnnotationsContext(opt), useSqlServer: UseSqlServer)
        {
        }

        [Fact]
        public void Output_Script()
        {
            OutputDbScript();
        }

    }
}