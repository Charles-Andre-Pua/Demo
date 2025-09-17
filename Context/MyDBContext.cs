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

    public virtual DbSet<EmailInvite> EmailInvites { get; set; }

    public virtual DbSet<EmailSent> EmailSents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-TVI6OFS\\SQLEXPRESS;Initial Catalog=dbDemo;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailInvite>(entity =>
        {
            entity.HasKey(e => e.EmailInviteId).HasName("PK__EmailInv__C4092CC46E23F4B9");

            entity.ToTable("EmailInvite");

            entity.Property(e => e.EmailInviteId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CollectedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailAddress).HasMaxLength(255);
        });

        modelBuilder.Entity<EmailSent>(entity =>
        {
            entity.HasKey(e => e.EmailSentId).HasName("PK__EmailSen__456E5F9BA0319F63");

            entity.ToTable("EmailSent");

            entity.Property(e => e.EmailSentId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.SentAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
