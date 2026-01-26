using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.PMS;
using DHAFacilitationAPIs.Domain.Enums.PMS;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DHAFacilitationAPIs.Application.Common.Interfaces;
public interface IPMSApplicationDbContext
{
    DbSet<Directorate> Directorates { get; }

    DbSet<ServiceCategory> ServiceCategories { get; }
    DbSet<ServiceProcess> ServiceProcesses { get; }
    DbSet<ProcessStep> ProcessSteps { get; }

    DbSet<PrerequisiteDefinition> PrerequisiteDefinitions { get; }
    DbSet<ProcessPrerequisite> ProcessPrerequisites { get; }

    DbSet<UserProperty> Properties { get; }
    DbSet<PropertyCase> PropertyCases { get; }
    DbSet<CaseStepHistory> CaseStepHistories { get; }
    DbSet<CasePrerequisiteValue> CasePrerequisiteValues { get; }
    DbSet<CaseDocument> CaseDocuments { get; }
    DbSet<CaseVoucher> CaseVouchers { get; }
    DbSet<CasePayment> CasePayments { get;}
    DbSet<FeeDefinition> FeeDefinitions { get; }
    DbSet<FeeSlab> FeeSlabs { get; }
    DbSet<CaseFee> CaseFees { get; }
    DbSet<PrerequisiteOption> PrerequisiteOptions { get; }
    DbSet<FeeOption> FeeOptions { get; }
    DbSet<FeeCategory> FeeCategories { get; }
    DbSet<CaseFeeReceipt> CaseFeeReceipts { get; }
    DbSet<FeeSetting> FeeSettings { get; }
    DbSet<CaseResultDocument> CaseResultDocuments { get; }
    DbSet<CaseRejectRequirement> CaseRejectRequirements { get; }

    DbSet<TEntity> Set<TEntity>() where TEntity : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    DatabaseFacade Database { get; }
}
