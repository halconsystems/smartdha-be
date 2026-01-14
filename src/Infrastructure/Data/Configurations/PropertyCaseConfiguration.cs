using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations;


public class PropertyCaseConfiguration
    : IEntityTypeConfiguration<PropertyCase>
{
    public void Configure(EntityTypeBuilder<PropertyCase> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => x.CaseNo).IsUnique();

        // Property
        builder.HasOne(x => x.UserProperty)
               .WithMany()
               .HasForeignKey(x => x.UserPropertyId)
               .OnDelete(DeleteBehavior.Restrict);

        // Process
        builder.HasOne(x => x.Process)
               .WithMany()
               .HasForeignKey(x => x.ProcessId)
               .OnDelete(DeleteBehavior.Restrict);

        // Current Step
        builder.HasOne(x => x.CurrentStep)
               .WithMany()
               .HasForeignKey(x => x.CurrentStepId)
               .OnDelete(DeleteBehavior.NoAction);
    }
}

