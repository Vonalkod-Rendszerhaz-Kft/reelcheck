using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRH.ConnectionStringStore;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Validation;
using System.Data.Entity;
using ReelCheck.Core.Migrations;

namespace ReelCheck.Core.DAL
{
    public class DataBaseContext : DbContext
    {
        /// <summary>
        /// Adatbázis context
        /// </summary>
        public DataBaseContext()
            : base(VRHConnectionStringStore.GetSQLConnectionString("DbConnection"))
        {
            this.Configuration.LazyLoadingEnabled = true;
            Database.SetInitializer<DataBaseContext>(
                new MigrateDatabaseToLatestVersion<DataBaseContext, MigrationConfiguration>());
        }

        /// <summary>
        /// Felismert tekercsek
        /// </summary>
        public DbSet<Reel> Reels { get; set; }
    }
}
