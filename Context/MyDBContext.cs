using System;
using System.Collections.Generic;
using Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Context;

public partial class MyDBContext : DbContext
{
    public MyDBContext()
    {
    }

    public MyDBContext(DbContextOptions<MyDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<EmailInvite> EmailInvites { get; set; }

    public virtual DbSet<EmailSent> EmailSents { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-TVI6OFS\\SQLEXPRESS;Initial Catalog=dbDemo;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.AccountId).HasName("PK__Account__349DA5A66204F09F");

            entity.ToTable("Account");

            entity.HasIndex(e => e.Username, "UQ__Account__536C85E4E6CBE5C7").IsUnique();

            entity.Property(e => e.AccountId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.EmployeeId).HasMaxLength(50);
            entity.Property(e => e.RoleId).HasMaxLength(50);
            entity.Property(e => e.Salt).HasMaxLength(10);
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Employee).WithOne(p => p.Accounts)
                .HasForeignKey<Account>(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__Employe__47DBAE45");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Account__RoleId__48CFD27E");
        });

        modelBuilder.Entity<EmailInvite>(entity =>
        {
            entity.HasKey(e => e.EmailInviteId).HasName("PK__EmailInv__C4092CC4938CADF1");

            entity.ToTable("EmailInvite");

            entity.Property(e => e.EmailInviteId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CollectedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailAddress).HasMaxLength(255);
        });

        modelBuilder.Entity<EmailSent>(entity =>
        {
            entity.HasKey(e => e.EmailSentId).HasName("PK__EmailSen__456E5F9B344DDCFB");

            entity.ToTable("EmailSent");

            entity.Property(e => e.EmailSentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId).HasName("PK__Employee__7AD04F118295D173");

            entity.ToTable("Employee");

            entity.Property(e => e.EmployeeId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.EmployeeName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(50);
            entity.Property(e => e.Status).HasDefaultValue(true);
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Role__8AFACE1AAC3A1627");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .HasMaxLength(50)
                .HasDefaultValueSql("(CONVERT([nvarchar](50),newid()))");
            entity.Property(e => e.RoleName).HasMaxLength(150);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
