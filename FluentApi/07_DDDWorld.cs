using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.DDDWorld
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
            var teamBuilder = modelBuilder.Entity<Team>();
            
            teamBuilder
                .Property<int>("_id")
                .ValueGeneratedNever()
                .HasColumnName("Id");;
            teamBuilder
                .HasKey("_id");
            teamBuilder.HasMany(t => t.Members)
                .WithOne()
                .HasForeignKey("TeamId")
                .OnDelete(DeleteBehavior.Cascade);
            teamBuilder.Property<string>("_name")
                .HasColumnName("Name");
            teamBuilder.Property<Size>(p => p.Size)
                .HasConversion(s => s.ToString(), i => (Size)Enum.Parse(typeof(Size), i));
                //.HasConversion(new EnumToNumberConverter<Size, byte>());

            var memberBuilder = modelBuilder.Entity<Member>();
            memberBuilder
                .Property<int>("_id")
                .ValueGeneratedNever()
                .HasColumnName("Id");;
            memberBuilder
                .HasKey("_id");
            memberBuilder.Property<string>("_firstName")
                .HasColumnName("FirstName");
            memberBuilder.Property<string>("_lastName")
                .HasColumnName("LastName");
        }
    }


    public class Team
    {
        private int _id;
        private string _name;
        private DateTime _created;
        public Size Size { get; }
        public List<Member> Members { get; private set; }

        public Team(int id, string name, Size size)
        {
            _id = id;
            _name = name;
            _created = DateTime.UtcNow;
            Size = size;

            Members = new List<Member>();
        }
    }

    public enum Size
    {
        S,
        M,
        L
    }

    public class Member
    {
        private int _id;
        private string _firstName;
        private string _lastName;

        public Member(int id, string firstName, string lastName)
        {
            _id = id;
            _firstName = firstName;
            _lastName = lastName;
        }
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
    }
}