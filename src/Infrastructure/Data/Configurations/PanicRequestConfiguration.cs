using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations;
public class PanicRequestConfiguration : IEntityTypeConfiguration<PanicRequest>
{
    public void Configure(EntityTypeBuilder<PanicRequest> b)
    {
        b.ToTable("PanicRequests");
        b.HasKey(x => x.Id);

        // Ser is identity → generated on add, never updated
        b.Property(x => x.Ser)
         .ValueGeneratedOnAdd()
         .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        b.Property(x => x.RowVersion)
         .IsRowVersion(); // SQL rowversion for optimistic concurrency

        b.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
        b.Property(x => x.Longitude).HasColumnType("decimal(9,6)");

        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.EmergencyType);
        b.HasIndex(x => x.Created);
        b.HasIndex(x => new { x.Status, x.Created });

        b.HasMany(x => x.Actions)
         .WithOne(x => x.PanicRequest)
         .HasForeignKey(x => x.PanicRequestId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(x => x.LocationTrail)
         .WithOne(x => x.PanicRequest)
         .HasForeignKey(x => x.PanicRequestId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}

public class PanicActionLogConfiguration : IEntityTypeConfiguration<PanicActionLog>
{
    public void Configure(EntityTypeBuilder<PanicActionLog> b)
    {
        b.ToTable("PanicActionLogs");
        b.HasKey(x => x.Id);
        b.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
        b.Property(x => x.Longitude).HasColumnType("decimal(9,6)");
        b.HasIndex(x => new { x.PanicRequestId, x.Created });
    }
}

public class PanicLocationUpdateConfiguration : IEntityTypeConfiguration<PanicLocationUpdate>
{
    public void Configure(EntityTypeBuilder<PanicLocationUpdate> b)
    {
        b.ToTable("PanicLocationUpdates");
        b.HasKey(x => x.Id);
        b.Property(x => x.Latitude).HasColumnType("decimal(9,6)");
        b.Property(x => x.Longitude).HasColumnType("decimal(9,6)");
        b.HasIndex(x => new { x.PanicRequestId, x.RecordedAt });
    }
}

