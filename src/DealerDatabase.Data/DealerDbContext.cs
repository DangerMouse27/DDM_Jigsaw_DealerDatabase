using DealerDatabase.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DealerDatabase.Data;

public class DealerDbContext(DbContextOptions<DealerDbContext> options) : DbContext(options)
{
    public DbSet<Dealer> Dealers => Set<Dealer>();

    public DbSet<DealerSourceLink> DealerSourceLinks => Set<DealerSourceLink>();

    public DbSet<DealerFieldAttribution> DealerFieldAttributions => Set<DealerFieldAttribution>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dealer>(entity =>
        {
            entity.Property(d => d.Name).IsRequired().HasMaxLength(300);
            entity.Property(d => d.LegalName).HasMaxLength(300);
            entity.Property(d => d.TradingName).HasMaxLength(300);

            entity.Property(d => d.CompanyNumber).HasMaxLength(16);
            entity.Property(d => d.VatNumber).HasMaxLength(16);
            entity.Property(d => d.VatValidationStatus).HasMaxLength(32);
            entity.Property(d => d.FcaFrn).HasMaxLength(16);
            entity.Property(d => d.MarketcheckDealerId).HasMaxLength(32);
            entity.Property(d => d.SafMemberId).HasMaxLength(32);
            entity.Property(d => d.IcoRegistrationNumber).HasMaxLength(32);

            entity.Property(d => d.Postcode).HasMaxLength(16);
            entity.Property(d => d.RegisteredPostcode).HasMaxLength(16);
            entity.Property(d => d.Phone).HasMaxLength(32);
            entity.Property(d => d.Email).HasMaxLength(256);
            entity.Property(d => d.Website).HasMaxLength(512);
            entity.Property(d => d.WebsiteDomain).HasMaxLength(256);
            entity.Property(d => d.Sources).IsRequired().HasMaxLength(256);
            entity.Property(d => d.LineageTag).IsRequired().HasMaxLength(2000);
            entity.Property(d => d.Directors).HasMaxLength(2000);
            entity.Property(d => d.FinanceCalculator).HasMaxLength(256);

            entity.HasIndex(d => d.CompanyNumber).IsUnique();
            entity.HasIndex(d => d.VatNumber).IsUnique();
            entity.HasIndex(d => d.FcaFrn).IsUnique();
            entity.HasIndex(d => d.MarketcheckDealerId).IsUnique();
            entity.HasIndex(d => d.SafMemberId).IsUnique();
            entity.HasIndex(d => d.IcoRegistrationNumber).IsUnique();
            entity.HasIndex(d => d.Name);
            entity.HasIndex(d => d.Postcode);
            entity.HasIndex(d => d.LineageTag);
        });

        modelBuilder.Entity<DealerSourceLink>(entity =>
        {
            entity.Property(l => l.SourceName).IsRequired().HasMaxLength(64);
            entity.Property(l => l.SourceRecordKey).IsRequired().HasMaxLength(128);

            entity.HasOne(l => l.Dealer)
                .WithMany(d => d.SourceLinks)
                .HasForeignKey(l => l.DealerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(l => new { l.SourceName, l.SourceRecordKey }).IsUnique();
        });

        modelBuilder.Entity<DealerFieldAttribution>(entity =>
        {
            entity.Property(a => a.FieldName).IsRequired().HasMaxLength(64);
            entity.Property(a => a.SourceName).IsRequired().HasMaxLength(64);
            entity.Property(a => a.SourceRecordKey).IsRequired().HasMaxLength(128);
            entity.Property(a => a.ValuePreview).HasMaxLength(512);

            entity.HasOne(a => a.Dealer)
                .WithMany(d => d.FieldAttributions)
                .HasForeignKey(a => a.DealerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(a => new { a.DealerId, a.FieldName });
        });
    }
}
