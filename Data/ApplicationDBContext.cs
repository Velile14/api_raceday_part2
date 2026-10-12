using Microsoft.EntityFrameworkCore;

namespace api_raceday_part2.Data
{//start of namespace

    public class ApplicationDBContext : DbContext
    {//start of ApplicationDBContext class

        public ApplicationDBContext(DbContextOptions options) : base(options)
        {
        }

        //TODO: add a DbSet<T> property here for each model,
        //once the model classes in /models have real properties.
        //e.g public DbSet<User> Users { get; set;}

    }//end of ApplicationDBContext class

}//end of namespace
