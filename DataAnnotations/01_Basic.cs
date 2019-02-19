using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.Basic
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Team>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getutcdate()");
        }
    }
    
    [Table("Teams")]
    public class Team
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
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


    public class Tests : EfTest<DataAnnotationsContext>
    {
        static bool UseSqlServer = true;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new DataAnnotationsContext(opt), useSqlServer: UseSqlServer)
        {
        }

        [Fact]
        public void Output_Script()
        {
            OutputDbScript();
        }

        [Fact]
        public async Task DateTime_Identity()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team" ,
                Members = new[] {
                    new Member { FirstName = "Pawel", LastName = "Maga" }
            }});
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            DumpObject(myTeam);
        }

    }
}