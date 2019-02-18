using System;
using System.CodeDom.Compiler;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit.Abstractions;

namespace EfCore_Demo.Setup
{
    public class EfTest<T> : IDisposable where T: DbContext
    {
        private readonly SqliteConnection _sqlConnection;

        protected readonly ITestOutputHelper Output;
        protected readonly T DbContext;
        protected readonly T SecondDbContext;

        public EfTest(ITestOutputHelper output, Func<DbContextOptions<T>, T> ctxCreate, bool useSqlServer = false,
            bool logToOutput = false)
        {
            Output = output;

            var optionsBuilder = new DbContextOptionsBuilder<T>();
            if (useSqlServer)
            {
                optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=EfCore_Demo;User ID=sa; Password=strongPwd123!;");
                //optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EFProviders.InMemory;Trusted_Connection=True;ConnectRetryCount=0");
            }
            else
            {
                _sqlConnection = new SqliteConnection("DataSource=:memory:");
                _sqlConnection.Open();

                optionsBuilder.UseSqlite(_sqlConnection);
            }

            if (logToOutput)
            {
                optionsBuilder.UseLoggerFactory(new XUnitLoggerFactory(Output));
            }

            DbContext = ctxCreate(optionsBuilder.Options);
            DbContext.Database.EnsureDeleted();
            DbContext.Database.EnsureCreated();

            SecondDbContext = ctxCreate(optionsBuilder.Options);
        }

        protected void OutputDbScript()
        {
            var script = DbContext.Database.GenerateCreateScript();
            Output.WriteLine(script);
        }

        protected void Separator()
        {
            Output.WriteLine(string.Empty);
            Output.WriteLine("------------------------");
            Output.WriteLine(string.Empty);
        }

        protected void DumpObject(object obj)
        {
            var stringBuilder = new StringBuilder();
            var serializer = new JsonSerializer
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };
            serializer.Serialize(new IndentedTextWriter(new StringWriter(stringBuilder)), obj);

            string pretty = JToken.Parse(stringBuilder.ToString()).ToString(Formatting.Indented);
            Output.WriteLine(pretty);
        }

        public void Dispose()
        {
            DbContext.Dispose();
            SecondDbContext.Dispose();
            _sqlConnection?.Close();
        }
    }

    class XUnitLoggerFactory : ILoggerFactory
    {
        private readonly ITestOutputHelper _output;

        public XUnitLoggerFactory(ITestOutputHelper output)
        {
            _output = output;
        }
        public void AddProvider(ILoggerProvider provider)
        {
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new XUnitLogger(_output);
        }

        public void Dispose()
        {
        }
    }

    class XUnitLogger : ILogger, IDisposable
    {
        private readonly ITestOutputHelper _output;

        public XUnitLogger(ITestOutputHelper output)
        {
            _output = output;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return this;
        }

        public void Dispose()
        {
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (logLevel == LogLevel.Information || logLevel == LogLevel.Debug)
            {
                _output.WriteLine(formatter(state, exception));
            }
        }
    }
}