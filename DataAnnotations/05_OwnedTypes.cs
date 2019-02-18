using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using EfCore_Demo.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace EfCore_Demo.DataAnnotations.OwnedTypes
{
    /*
        -----------------------
            CONFIGURATION
        -----------------------
    */
    
    public class DataAnnotationsContext : DbContext
    {
        public DbSet<Member> Members { get; set; }

        public DataAnnotationsContext(DbContextOptions<DataAnnotationsContext> options) : base(options) {}
    }

    public class Member
    {
        public int Id { get; set; }
        public PersonalData Data { get; set; }
    }

    [Owned]
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


    public class Tests : EfTest<DataAnnotationsContext>
    {
        static bool UseSqlServer = false;
        static bool LogToOutput = false;

        public Tests(ITestOutputHelper output) 
            : base(output, opt => new DataAnnotationsContext(opt), useSqlServer: UseSqlServer,
                logToOutput: LogToOutput)
        {
        }

        [Fact]
        public void Output_Script()
        {
            OutputDbScript();
        }

        [Fact]
        public async Task InsertAndGet()
        {
            DbContext.Members.Add(new Member 
            {
                Data = new PersonalData
                {
                    FirstName = "Pawel",
                    LastName = "Maga"
                }
            });
            DbContext.SaveChanges();

            var me = await DbContext.Members.FirstAsync();
            DumpObject(me);
        }
    }
}