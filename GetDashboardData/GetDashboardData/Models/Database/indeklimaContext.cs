using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GetDashboardData.Models.Database
{
    public partial class indeklimaContext : DbContext
    {
        private static IConfiguration _config;

        public indeklimaContext(IConfiguration config)
        {
            _config = config;
        }

        public indeklimaContext(DbContextOptions<indeklimaContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Temperatur> Temperaturs { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(_config["DbConnectionString"]);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Temperatur>(entity =>
            {
                entity.HasKey(e => new { e.Dato, e.Tidspunkt })
                    .HasName("PK__Temperat__DAF567423CA3787A");

                entity.ToTable("Temperatur");

                entity.Property(e => e.Dato)
                    .HasColumnType("date")
                    .HasColumnName("dato");

                entity.Property(e => e.Tidspunkt)
                    .HasColumnType("time(0)")
                    .HasColumnName("tidspunkt");

                entity.Property(e => e.Grader)
                    .HasColumnType("numeric(3, 1)")
                    .HasColumnName("grader");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
