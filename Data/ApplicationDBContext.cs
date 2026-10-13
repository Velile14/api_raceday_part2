using Microsoft.EntityFrameworkCore;

namespace api_raceday_part2.Data
{//start of namespace

    public class ApplicationDBContext : DbContext
    {//start of ApplicationDBContext class

        public ApplicationDBContext(DbContextOptions options) : base(options)
        {//start of constructor

        }//end of constructor

        //TODO: add a DbSet<T> property here for each model,
        //once the model classes in /models have real properties.
        //e.g public DbSet<User> Users { get; set;}
        public DbSet<api_raceday_part2.models.Event> Events { get; set; }

        //configure entity properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {//start of method

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<api_raceday_part2.models.Event>()
                .Property(e => e.Distance)
                .HasPrecision(6, 2);

        }//end of method

    }//end of ApplicationDBContext class

}//end of namespace
