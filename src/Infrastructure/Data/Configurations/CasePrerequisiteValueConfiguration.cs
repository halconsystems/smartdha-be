using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations;
public class CasePrerequisiteValueConfiguration
    : IEntityTypeConfiguration<CasePrerequisiteValue>
{
    public void Configure(EntityTypeBuilder<CasePrerequisiteValue> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasIndex(x => new { x.CaseId, x.PrerequisiteDefinitionId })
               .IsUnique();

        builder.HasOne(x => x.Case)
               .WithMany()
               .HasForeignKey(x => x.CaseId)
               .OnDelete(DeleteBehavior.Restrict); // 🔥 FIX

        builder.HasOne(x => x.PrerequisiteDefinition)
               .WithMany()
               .HasForeignKey(x => x.PrerequisiteDefinitionId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

