using System;
using System.Collections.Generic;
using CourtOnlineWeek5.Models;
using Microsoft.EntityFrameworkCore;

namespace CourtOnlineWeek5.Data;

public partial class CourtCaseDatabaseContext : DbContext
{
    public CourtCaseDatabaseContext()
    {
    }

    public CourtCaseDatabaseContext(DbContextOptions<CourtCaseDatabaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Charge> Charges { get; set; }

    public virtual DbSet<Court> Courts { get; set; }

    public virtual DbSet<CourtCase> CourtCases { get; set; }

    public virtual DbSet<CourtCaseDefendant> CourtCaseDefendants { get; set; }

    public virtual DbSet<Defendant> Defendants { get; set; }

    public virtual DbSet<Judge> Judges { get; set; }

    public virtual DbSet<OriginalCase> OriginalCases { get; set; }

    public virtual DbSet<Prosecutor> Prosecutors { get; set; }

   // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
     //   => optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CourtOnlineWeek5DB;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Charge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Charges__3214EC07FC40ED48");

            entity.HasOne(d => d.CourtCase).WithMany(p => p.Charges).HasConstraintName("FK_Charges_CourtCases");
        });

        modelBuilder.Entity<Court>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Courts__3214EC075632CF21");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        modelBuilder.Entity<CourtCase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CourtCas__3214EC0719BF172D");

            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getutcdate())");
            entity.Property(e => e.Status).HasDefaultValue("Open");

            entity.HasOne(d => d.Court).WithMany(p => p.CourtCases).HasConstraintName("FK_CourtCases_Courts");

            entity.HasOne(d => d.Judge).WithMany(p => p.CourtCases).HasConstraintName("FK_CourtCases_Judges");

            entity.HasOne(d => d.OriginalCase).WithMany(p => p.CourtCases)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CourtCases_OriginalCases");

            entity.HasOne(d => d.Prosecutor).WithMany(p => p.CourtCases).HasConstraintName("FK_CourtCases_Prosecutors");
        });

        modelBuilder.Entity<CourtCaseDefendant>(entity =>
        {
            entity.Property(e => e.Role).HasDefaultValue("Accused");

            entity.HasOne(d => d.CourtCase).WithMany(p => p.CourtCaseDefendants).HasConstraintName("FK_CCD_CourtCases");

            entity.HasOne(d => d.Defendant).WithMany(p => p.CourtCaseDefendants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CCD_Defendants");
        });

        modelBuilder.Entity<Defendant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Defendan__3214EC07C01ABE77");

            entity.Property(e => e.Nationality).HasDefaultValue("South African");
        });

        modelBuilder.Entity<Judge>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Judges__3214EC07873C6808");

            entity.Property(e => e.IsActive).HasDefaultValue(true);

            entity.HasOne(d => d.Court).WithMany(p => p.Judges).HasConstraintName("FK_Judges_Courts");
        });

        modelBuilder.Entity<OriginalCase>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Original__3214EC078414C7EF");

            entity.Property(e => e.Priority).HasDefaultValue("Normal");
        });

        modelBuilder.Entity<Prosecutor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Prosecut__3214EC07F3B4291C");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
