using FacialRecognitionTimekeepingAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class TimekeepingContext : DbContext
    {
        public TimekeepingContext(DbContextOptions<TimekeepingContext> options) : base(options) { }

        public DbSet<TimekeepingPerson> TimekeepingPeople { get; set; }
        public DbSet<TimekeepingRecord> TimekeepingRecords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TimekeepingRecord>()
                .HasKey(r => new { r.AliasId, r.TimekeepingRecordUnixTimestampSeconds });

            modelBuilder.Entity<TimekeepingRecord>(entity =>
            {
                entity.HasOne(r => r.TimekeepingPerson)
                    .WithMany(p => p.TimekeepingRecords)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
