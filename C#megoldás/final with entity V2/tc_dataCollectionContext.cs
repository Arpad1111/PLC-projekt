using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.EntityFrameworkCore.SqlServer;

namespace final_with_entity
{
    public class tc_dataCollectionContext :DbContext
    {
        public DbSet<RecordModell> Chambers { get; set; }
        public string Tablename { get; set; }
        public string ConnectionString { get; set; }
        public string DBMS { get; set; }
        #region OnConfiguring
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (DBMS=="MySQL")
            {
                optionsBuilder.UseMySQL(ConnectionString);
            }
            //"server=localhost;database=tc_datacollection;user=root;password=132435465768"
            if (DBMS=="SQLite")
            {
                optionsBuilder.UseSqlite(ConnectionString);
            }
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, DynamicModelChacheKeyFactory>();
            #endregion
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
                modelBuilder.Entity<RecordModell>(entity =>
                {
                    entity.ToTable(Tablename);
                    entity.HasKey(e => e.Dátum);
                    //entity.Property(e => e.temp_set);
                    //entity.Property(e => e.temp_act);
                    //entity.Property(e => e.hum_set);
                    //entity.Property(e => e.hum_act);
                });
            
            
            
        }


    }
}

