using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations;
public class EmergencyTypeConfiguration : IEntityTypeConfiguration<EmergencyType>
{
    public void Configure(EntityTypeBuilder<EmergencyType> b)
    {
        b.ToTable("EmergencyTypes");
        b.HasKey(x => x.Id);

        b.Property(x => x.Ser)
         .ValueGeneratedOnAdd()
         .Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);

        b.HasIndex(x => x.Code).IsUnique();
        b.HasIndex(x => x.Name);
        b.HasIndex(x => new { x.IsActive, x.IsDeleted, x.DisplayOrder });
    }
}

