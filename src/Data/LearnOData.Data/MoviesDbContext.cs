using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearnOData.Model;

namespace LearnOData.Data
{
    public class MoviesDbContext : DbContext
    {
        public DbSet<Person> People { get; set; }
        public DbSet<Movie> Movies { get; set; }

        public MoviesDbContext() {
            Database.SetInitializer(new MoviesDBInitializer());
            this.Configuration.LazyLoadingEnabled = false;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            // ensure the same person can be added to different collections
            // of friends (self-referencing many-to-many relationship)
            modelBuilder.Entity<Person>().HasMany(m => m.Friends).WithMany();

            modelBuilder.Entity<Person>().HasMany(p => p.Movies)
                .WithRequired(r => r.Person).WillCascadeOnDelete(true);
            
            //modelBuilder.Entity<Person>().HasMany(p => p.VinylRecords)
            //    .WithRequired(r => r.Person).WillCascadeOnDelete(true);

        }
    }
}
