using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
    // DbContext provides the database connection, data access/query and where
    // to define database model or "tables" via DbSet
    // DbSet can be said to be a representation of a table and data
    // you use a DbContext object to access tables/DbSets and you use DbSets
    // to get access, create, update, delet table data

    // to update entities/db enter following in Package Manager Console:
    // add-migration <comment>
    // update-database
    public class CityInfoContext : DbContext
    {
        public DbSet<City> Cities { get; set; } = null!; // null forgiving operator to ignore warning since we know that it wont be null
        public DbSet<PointOfInterest> PointOfInterests { get; set; } = null!;

        // this is necessary to configure our DbContext so it can know where to connect to
        // used when we register our dbcontext in program.cs
        public CityInfoContext(DbContextOptions<CityInfoContext> options)
            : base(options)
        {

        }

        // this allows us to provide the DB with initial data
        // which is also called seeing the DB 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>()
                .HasData(
                new City("New York City")
                {
                    Id = 1,
                    Description = "The one wiwth that big park."
                },
                new City("Antwerp")
                {
                    Id=2,
                    Description = "The one with the cathedral that was never really finished."
                },
                new City("Paris")
                {
                    Id=3,
                    Description = "The one with the big tower."
                });

            modelBuilder.Entity<PointOfInterest>()
                .HasData(
                new PointOfInterest("Central Park")
                {
                    Id = 1,
                    CityId = 1,
                    Description = "The most visited urban park in the United States."
                },
                new PointOfInterest("Empire State Building")
                {
                    Id = 2,
                    CityId = 1,
                    Description = "A 102-story skyscraper located in Midtown Manhattan."
                },
                new PointOfInterest("Cathedral")
                {
                    Id = 3,
                    CityId = 2,
                    Description = "A Gothic style cathedral, conceived by architects Jan and Pietro."
                },
                new PointOfInterest("Antwerp Central Station")
                {
                    Id = 4,
                    CityId = 2,
                    Description = "The finest example of railway architecture in Belgium."
                },
                new PointOfInterest("Eiffel Tower")
                {
                    Id = 5,
                    CityId = 3,
                    Description = "A wrought iron lattice tower in the Champ de Mars."
                },
                new PointOfInterest("The Louvre")
                {
                    Id = 6,
                    CityId = 3,
                    Description = "The world's largest museum."
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
