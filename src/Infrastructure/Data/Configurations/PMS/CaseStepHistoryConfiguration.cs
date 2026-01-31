using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DHAFacilitationAPIs.Infrastructure.Data.Configurations.PMS;
public class CaseStepHistoryConfiguration
    : IEntityTypeConfiguration<CaseStepHistory>
{
    public void Configure(EntityTypeBuilder<CaseStepHistory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Case)
               .WithMany()
               .HasForeignKey(x => x.CaseId)
               .OnDelete(DeleteBehavior.Restrict); // 🔥 FIX

        builder.HasOne(x => x.Step)
               .WithMany()
               .HasForeignKey(x => x.StepId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

