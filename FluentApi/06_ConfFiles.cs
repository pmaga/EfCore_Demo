using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.FluentApi.ConfFiles
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class FluentApiContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public FluentApiContext(DbContextOptions<FluentApiContext> options) : base(options) {}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TeamConfiguration());
        }
    }

    public class TeamConfiguration : IEntityTypeConfiguration<Team>
    {
        public void Configure(EntityTypeBuilder<Team> builder)
        {
            builder.ToTable("Teams");
            builder.Property(p => p.Name)
                .HasMaxLength(150);
            builder.Property(p => p.Created)
                .HasDefaultValueSql("getdate()");
        }
    }

    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; } 
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