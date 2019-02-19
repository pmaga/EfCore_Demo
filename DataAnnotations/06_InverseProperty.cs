using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.InverseProperty
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class DataAnnotationsContext : DbContext
    {
        public DbSet<Member> Members { get; set; }
        public DbSet<UserStory> UserStories { get; set; }

        public DataAnnotationsContext(DbContextOptions<DataAnnotationsContext> options) : base(options) {}
    }
    

    public class Member
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [InverseProperty(nameof(UserStory.CreatedBy))]
        public ICollection<UserStory> CreatedStories { get; set; }

        [InverseProperty(nameof(UserStory.UpdatedBy))]
        public ICollection<UserStory> UpdatedStories { get; set; }
    }

    public class UserStory
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public Member CreatedBy { get; set; }
        public Member UpdatedBy { get; set; }
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
        public void Output_Script_Fail()
        {
            OutputDbScript();
        }

        [Fact]
        public void Output_Script()
        {
            // => uncomment InverseProperty
            OutputDbScript();
        }
    }
}