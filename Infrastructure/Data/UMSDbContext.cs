
using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.Models.VedioMetaData;

namespace Infrastructure.Data
{
    public class UMSDbContext : DbContext
    {
        public UMSDbContext()
        {
        }
        public UMSDbContext(DbContextOptions<UMSDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define relationships and constraints here if necessary
            modelBuilder.Entity<UserWatchHistory>()
                .HasKey(uwh => uwh.Id);

            modelBuilder.Entity<UserWatchHistory>()
                .HasOne(uwh => uwh.User)
                .WithMany(u => u.WatchHistories)
                .HasForeignKey(uwh => uwh.UserId);

            modelBuilder.Entity<UserWatchHistory>()
                .HasOne(uwh => uwh.Video)
                .WithMany(v => v.WatchHistories)
                .HasForeignKey(uwh => uwh.VideoId);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<VideoMetadata> VideoMetadatas { get; set; }
        public DbSet<UserWatchHistory> UserWatchHistories { get; set; }
        public DbSet<RatingVedio> RatingVedios { get; set; }

    }
}
