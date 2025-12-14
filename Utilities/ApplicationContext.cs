using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using YClimb.Entities;

namespace YClimb.Utilities
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Post> Posts { get; set; } = null!;
        public DbSet<PostImage> PostImages { get; set; } = null!;
        public DbSet<Route> Routes { get; set; }
        public DbSet<RouteImage> RouteImages { get; set; }
        public DbSet<Trainer> Trainers { get; set; } = null!;
        public DbSet<TrainingGroup> TrainingGroups { get; set; } = null!;
        public DbSet<ScheduleTemplate> ScheduleTemplates { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            modelBuilder.Entity<Post>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PostImage>()
                .HasOne(pi => pi.Post)
                .WithMany(p => p.Images)
                .HasForeignKey(pi => pi.PostId)
                .OnDelete(DeleteBehavior.Cascade);



            modelBuilder.Entity<Route>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Route>()
                .HasOne(r => r.Image)
                .WithOne(i => i.Route)
                .HasForeignKey<RouteImage>(i => i.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
            


            modelBuilder.Entity<ScheduleTemplate>()
                .HasOne(st => st.TrainingGroup)
                .WithMany()
                .HasForeignKey(st => st.TrainingGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ScheduleTemplate>()
                .HasOne(st => st.Trainer)
                .WithMany()
                .HasForeignKey(st => st.TrainerId)
                .OnDelete(DeleteBehavior.Cascade);

        }


        public async Task AfterDatabaseCreated()
        {
            if (!Users.Any(u => u.Nickname == "admin"))
            {
                var adminUser = new User("admin", null, PasswordHelper.HashPassword("1"), true);
                Users.Add(adminUser);
                MessageBox.Show("Для тестирования приложения была создана учетка админа (login: admin, password: 1)");
                await SaveChangesAsync();
            }
        }
    }
}
