using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.Owned
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
                .OwnsMany(p => p.Members, builder =>
                {
                    builder.HasForeignKey("MemberId");
                    builder.Property<int>("Id")
                        .ValueGeneratedOnAdd();;
                    builder.HasKey("Id");
                });

            modelBuilder.Entity<Member>()
                .OwnsOne(m => m.Data);
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
        public PersonalData Data { get; set; }
    }

    public class PersonalData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    /*
        -----------------------
                TESTS
        -----------------------
    */


    public class Tests : EfTest<FluentApiContext>
    {
        static bool UseSqlServer = true;
        static bool LogToOutput = false;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new FluentApiContext(opt), useSqlServer: UseSqlServer,
            logToOutput: LogToOutput)
        {
        }

        [Fact]
        public void Output_Script()
        {
            OutputDbScript();
        }

        [Fact]
        public async Task Query()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team",
                Members = new[] {
                    new Member 
                    {
                        Data = new PersonalData
                        {
                            FirstName = "Pawel",
                            LastName = "Maga"
                        }
                    },
                }
            });
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            DumpObject(myTeam);
        }

        [Fact]
        public async Task NullableTypes()
        {
            DbContext.Teams.Add(new Team 
            { 
                Name = "Impact Team",
                Members = new[] {
                    new Member 
                    {
                        Data = new PersonalData { FirstName = "", LastName = "" }
                    },
                }
            });
            DbContext.SaveChanges();

            var myTeam = await DbContext.Teams.FirstAsync();
            DumpObject(myTeam);
        }
    }
}