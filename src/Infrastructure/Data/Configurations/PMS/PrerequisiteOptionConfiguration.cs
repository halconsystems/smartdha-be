using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations.PMS;
public class PrerequisiteOptionConfiguration : IEntityTypeConfiguration<PrerequisiteOption>
{
    public void Configure(EntityTypeBuilder<PrerequisiteOption> builder)
    {
        builder.ToTable("PrerequisiteOptions");

        builder.HasOne(x => x.PrerequisiteDefinition)
               .WithMany() // keep simple (or add navigation later)
               .HasForeignKey(x => x.PrerequisiteDefinitionId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => new { x.PrerequisiteDefinitionId, x.Value }).IsUnique();
    }
}

