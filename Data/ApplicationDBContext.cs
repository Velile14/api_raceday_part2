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

        public DbSet<api_raceday_part2.models.User> Users { get; set; }

        public DbSet<api_raceday_part2.models.Role> Roles { get; set; }

        public DbSet<api_raceday_part2.models.Category> Categories
        { get; set; }

        public DbSet<api_raceday_part2.models.Enrolment> Enrolments
        { get; set; }

        public DbSet<api_raceday_part2.models.Result> Results
        { get; set; }

        public DbSet<api_raceday_part2.models.EventType> EventTypes
        { get; set; }

        //configure entity properties
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {//start of method

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<api_raceday_part2.models.Event>()
                .Property(e => e.Distance)
                .HasPrecision(6, 2);

            modelBuilder.Entity<api_raceday_part2.models.User>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(u => u.RoleID);

            modelBuilder.Entity<api_raceday_part2.models.Event>()
                .HasOne<api_raceday_part2.models.User>()
                .WithMany()
                .HasForeignKey(e => e.OrganizerID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<api_raceday_part2.models.Category>()
                .HasOne(c => c.Event)
                .WithMany()
                .HasForeignKey(c => c.EventID);

            modelBuilder.Entity<api_raceday_part2.models.Category>()
                .Property(c => c.Distance)
                .HasPrecision(6, 2);

            // Enrolment relationships: configure each relationship only once.
            modelBuilder.Entity<api_raceday_part2.models.Enrolment>()
                .HasOne(e => e.Event)
                .WithMany()
                .HasForeignKey(e => e.EventID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<api_raceday_part2.models.Enrolment>()
                .HasOne(e => e.Category)
                .WithMany()
                .HasForeignKey(e => e.CategoryID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<api_raceday_part2.models.Enrolment>()
                .HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserID)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<api_raceday_part2.models.Result>()
                .HasOne(r => r.Enrolment)
                .WithMany()
                .HasForeignKey(r => r.EnrolmentID);

            modelBuilder.Entity<api_raceday_part2.models.Result>()
                .HasIndex(r => r.EnrolmentID)
                .IsUnique();

            modelBuilder.Entity<api_raceday_part2.models.Event>()
                .HasOne(e => e.EventType)
                .WithMany()
                .HasForeignKey(e => e.EventTypeID);

        }//end of method

    }//end of ApplicationDBContext class

}//end of namespace
