using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using UserHub.Infrastructure.Persistence.Entities;

namespace UserHub.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ConditionStatuses> ConditionStatuses { get; set; }

    public virtual DbSet<LoginLogs> LoginLogs { get; set; }

    public virtual DbSet<RefreshTokens> RefreshTokens { get; set; }

    public virtual DbSet<Roles> Roles { get; set; }

    public virtual DbSet<Sessions> Sessions { get; set; }

    public virtual DbSet<UserStatuses> UserStatuses { get; set; }

    public virtual DbSet<Users> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=userhub;Username=postgres;Password=Skyworx@2025!");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("pgcrypto");

        modelBuilder.Entity<ConditionStatuses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("condition_statuses_pkey");

            entity.ToTable("condition_statuses");

            entity.HasIndex(e => e.Name, "condition_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<LoginLogs>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("login_logs_pkey");

            entity.ToTable("login_logs");

            entity.HasIndex(e => e.UserId, "idx_login_logs_user_id");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.IsLoggedIn)
                .HasDefaultValue(false)
                .HasColumnName("is_logged_in");
            entity.Property(e => e.LoginAt).HasColumnName("login_at");
            entity.Property(e => e.LogoutAt).HasColumnName("logout_at");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.LoginLogs)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_log_user");
        });

        modelBuilder.Entity<RefreshTokens>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens");

            entity.HasIndex(e => e.SessionId, "idx_refresh_tokens_session_id");

            entity.HasIndex(e => e.TokenHash, "refresh_tokens_token_hash_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.ExpiresAt).HasColumnName("expires_at");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.SessionId).HasColumnName("session_id");
            entity.Property(e => e.TokenHash)
                .HasMaxLength(128)
                .HasColumnName("token_hash");

            entity.HasOne(d => d.Session).WithMany(p => p.RefreshTokens)
                .HasForeignKey(d => d.SessionId)
                .HasConstraintName("fk_refresh_token_session");
        });

        modelBuilder.Entity<Roles>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.HasIndex(e => e.DeletedAt, "idx_roles_deleted_at").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.Name, "roles_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.IsSystem)
                .HasDefaultValue(false)
                .HasColumnName("is_system");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("now()")
                .HasColumnName("updated_at");
        });

        modelBuilder.Entity<Sessions>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessions_pkey");

            entity.ToTable("sessions");

            entity.HasIndex(e => e.RevokedAt, "idx_sessions_revoked_at");

            entity.HasIndex(e => e.UserId, "idx_sessions_user_id");

            entity.HasIndex(e => e.LoginLogId, "sessions_login_log_id_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.IpAddress)
                .HasMaxLength(45)
                .HasColumnName("ip_address");
            entity.Property(e => e.LastUsedAt).HasColumnName("last_used_at");
            entity.Property(e => e.LoginLogId).HasColumnName("login_log_id");
            entity.Property(e => e.RevokedAt).HasColumnName("revoked_at");
            entity.Property(e => e.UserAgent).HasColumnName("user_agent");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.LoginLog).WithOne(p => p.Sessions)
                .HasForeignKey<Sessions>(d => d.LoginLogId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_session_login_log");

            entity.HasOne(d => d.User).WithMany(p => p.Sessions)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("fk_session_user");
        });

        modelBuilder.Entity<UserStatuses>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_statuses_pkey");

            entity.ToTable("user_statuses");

            entity.HasIndex(e => e.Name, "user_statuses_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Users>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Nip, "idx_user_nip");

            entity.HasIndex(e => e.DeletedAt, "idx_users_deleted_at").HasFilter("(deleted_at IS NULL)");

            entity.HasIndex(e => e.Email, "users_email_key").IsUnique();

            entity.HasIndex(e => e.Nip, "users_nip_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ConditionStatusId).HasColumnName("condition_status_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("created_at");
            entity.Property(e => e.DeletedAt).HasColumnName("deleted_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Fullname)
                .HasMaxLength(150)
                .HasColumnName("fullname");
            entity.Property(e => e.Nip)
                .HasMaxLength(50)
                .HasColumnName("nip");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.ConditionStatus).WithMany(p => p.Users)
                .HasForeignKey(d => d.ConditionStatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_condition");

            entity.HasOne(d => d.Status).WithMany(p => p.Users)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("fk_user_status");

            entity.HasMany(d => d.Role).WithMany(p => p.User)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRoles",
                    r => r.HasOne<Roles>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("fk_pivot_role"),
                    l => l.HasOne<Users>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("fk_pivot_user"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<int>("UserId").HasColumnName("user_id");
                        j.IndexerProperty<int>("RoleId").HasColumnName("role_id");
                    });
        });
        modelBuilder.HasSequence("user_nip_seq");

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
