using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RecruiterPathway.Data;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecruiterPathway.Tests.Fixture
{
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedDatabaseFixture()
        {
            Connection = new SqlConnection(@"Server=(localdb)\mssqllocaldb;Database=TestDatabase;Trusted_Connection=True;MultipleActiveResultSets=true");

            SeedData();

            Connection.Open();
        }

        public void Dispose() => Connection.Dispose();

        public DbConnection Connection { get; }

        public DatabaseContext CreateContext(DbTransaction transaction = null)
        {
            var context = new DatabaseContext(
                new DbContextOptionsBuilder<DatabaseContext>().UseSqlServer(Connection).Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private void SeedData()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();

                        Models.SeedDatabase.SeedStudents(context);
                        List<string> guids = new List<string>();
                        Models.SeedDatabase.SeedRecruiters(context, MockedDatabase.GetRecruiterUserManager(context).Object, ref guids);
                        context.SaveChanges();
                    }
                    _databaseInitialized = true;
                }
            }
        }
    }
}
