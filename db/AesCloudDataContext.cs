using System;
using System.Configuration;
using AesCloudDataNet.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

#nullable disable

namespace AesCloudDataNet.DB
{ 
    public partial class AesCloudDataContext : DbContext
    {
        public AesCloudDataContext()
        {
        }

        public AesCloudDataContext(DbContextOptions<AesCloudDataContext> options,
            IConfiguration configuration)
            : base(options)
        {
            Configuration = configuration;
            Database.EnsureCreated();// = false;
        }

        public virtual DbSet<RateToUsd> RateToUsd { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAction> UserActions { get; set; }
        public IConfiguration Configuration { get; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var ConnectionString = Configuration.GetConnectionString("MySqlCinnectionString");

                optionsBuilder.UseMySQL(ConnectionString);

                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                //                optionsBuilder.UseNpgsql("host=localhost;port=5432;database=clouddata;userid=postgres;password=1q1q");
            }
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasAnnotation("Relational:Collation", "English_United States.1252");

        //    modelBuilder.Entity<RateToUsd>(entity =>
        //    {
        //        entity.ToTable("RateToUsd", "andrey1de");

        //        entity.Property(e => e.Id)
        //            .ValueGeneratedNever()
        //            .HasColumnName("id");

        //        entity.Property(e => e.Ask).HasColumnName("ask");

        //        entity.Property(e => e.Bid).HasColumnName("bid");

        //        entity.Property(e => e.Code)
        //            .HasMaxLength(3)
        //            .HasColumnName("code")
        //            .IsFixedLength(true);

        //        entity.Property(e => e.LastRefreshed)
        //            .HasColumnType("timestamp(3) with time zone")
        //            .HasColumnName("last_refreshed");

        //        entity.Property(e => e.Name)
        //            .HasMaxLength(40)
        //            .HasColumnName("name");

        //        entity.Property(e => e.Rate).HasColumnName("rate");

        //        entity.Property(e => e.Stored)
        //            .HasColumnType("timestamp(3) with time zone")
        //            .HasColumnName("stored");
        //    });

        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.ToTable("Users", "andrey1de");

        //        entity.Property(e => e.Id)
        //            .ValueGeneratedNever()
        //            .HasColumnName("id");

        //        entity.Property(e => e.Email).HasColumnName("email");

        //        entity.Property(e => e.Guid).HasColumnName("guid");

        //        entity.Property(e => e.Name).HasColumnName("name");

        //        entity.Property(e => e.Password).HasColumnName("password");

        //        entity.Property(e => e.Severity).HasColumnName("severity");
        //    });

        //    modelBuilder.Entity<UserAction>(entity =>
        //    {
        //        entity.ToTable("UserActions", "andrey1de");

        //        entity.Property(e => e.Id)
        //            .ValueGeneratedNever()
        //            .HasColumnName("id");

        //        entity.Property(e => e.Blob).HasColumnName("blob");

        //        entity.Property(e => e.Guid)
        //            .IsRequired()
        //            .HasMaxLength(16)
        //            .HasColumnName("guid")
        //            .IsFixedLength(true);

        //        entity.Property(e => e.Json).HasColumnName("json");

        //        entity.Property(e => e.NextActionDate)
        //            .HasColumnType("date")
        //            .HasColumnName("next_action_date");

        //        entity.Property(e => e.PriodSec).HasColumnName("priod_sec");

        //        entity.Property(e => e.Type)
        //            .IsRequired()
        //            .HasColumnName("type");

        //        entity.Property(e => e.User).HasColumnName("user");

        //        entity.Property(e => e.UserId).HasColumnName("user_id");
        //    });

        //    OnModelCreatingPartial(modelBuilder);
        //}

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
