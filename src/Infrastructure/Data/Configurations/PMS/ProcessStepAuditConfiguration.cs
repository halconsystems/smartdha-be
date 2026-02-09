using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations.PMS;
public class ProcessStepAuditConfiguration
    : IEntityTypeConfiguration<ProcessStepAudit>
{
    public void Configure(EntityTypeBuilder<ProcessStepAudit> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Action)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property<int>("Ser")
           .ValueGeneratedOnAdd()
           .Metadata.SetAfterSaveBehavior(
               PropertySaveBehavior.Ignore
           );
    }
}

