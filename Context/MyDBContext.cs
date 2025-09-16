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
        => optionsBuilder.UseSqlServer("Data Source=LAB4-PC26\\LAB2PC26;Initial Catalog=dbDemo;Integrated Security=True;Trust Server Certificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailInvite>(entity =>
        {
            entity.HasKey(e => e.EmailInviteId).HasName("PK__EmailInv__C4092CC4DD4E81BA");

            entity.ToTable("EmailInvite");

            entity.HasIndex(e => e.EmailAddress, "UQ_MarketingEmailInvite_Email").IsUnique();

            entity.Property(e => e.EmailInviteId).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.EmailAddress).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
        });

        modelBuilder.Entity<EmailSent>(entity =>
        {
            entity.HasKey(e => e.EmailSent1).HasName("PK__EmailSen__521E02865E667573");

            entity.ToTable("EmailSent");

            entity.Property(e => e.EmailSent1)
                .HasDefaultValueSql("(newid())")
                .HasColumnName("EmailSent");
            entity.Property(e => e.Message).HasMaxLength(255);
            entity.Property(e => e.Subject).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
