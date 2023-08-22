using Microsoft.EntityFrameworkCore;
using SportsClub.Models;

namespace SportsClub.Data
{
    public class SportsDbContext: DbContext //inheritence - drives from DbContext class
    {

        //constructor - DbContextOptions carries all the required configuration information such as connection string, database provider, etc. Passed from Program.cs
        public SportsDbContext(DbContextOptions<SportsDbContext> options): base(options) { } //base initializes a new instanse of DbContext class using the specified options

        //create connections between DbContext and database tables
        public DbSet<Fan> Fans { get; set; } //entity type Fan object
        public DbSet<SportClub> SportClubs { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<News> News { get; set; }

        //you can use data annotation in model classes or fluent api here and override OnModelCreating for additional configurations
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fan>().ToTable("Fan");
            modelBuilder.Entity<SportClub>().ToTable("Sportclub");
            modelBuilder.Entity<Subscription>()
                .ToTable("Subscription")
                .HasKey(m => new { m.SportClubId, m.FanId });
            modelBuilder.Entity<News>()
                .ToTable("News")
                .HasKey(m => new { m.NewsId, m.SportClubId });
        }

    }
}
