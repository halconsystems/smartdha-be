using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Dto;
using DHAFacilitationAPIs.Application.Common.Interfaces;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Infrastructure.Service;

public class EnumService : IEnumService
{
    public object GetEnums(EnumType? enumType)
    {
        if (enumType == null)
        {
            return new Dictionary<string, List<EnumItemDto>>
            {
                { nameof(CategoryType), GetEnumList<CategoryType>() },
                { nameof(MemberType), GetEnumList<MemberType>() },
                { nameof(Zone), GetEnumList<Zone>() },
                { nameof(RelationUserFamily), GetEnumList<RelationUserFamily>() },
                { nameof(ResidenceStatusDha), GetEnumList<ResidenceStatusDha>() },
                { nameof(Phase), GetEnumList<Phase>() },
                { nameof(PropertyTypeCommercial), GetEnumList<PropertyTypeCommercial>() },
                { nameof(PropertyTypeResidential), GetEnumList<PropertyTypeResidential>() },
                { nameof(JobType), GetEnumList<JobType>() },
                { nameof(WorkerCardDeliveryType), GetEnumList<WorkerCardDeliveryType>() },
                { nameof(VisitorPassType), GetEnumList<VisitorPassType>() }
            };
        }

        return enumType switch
        {
            EnumType.CategoryType => GetEnumList<CategoryType>(),
            EnumType.MemberType => GetEnumList<MemberType>(),
            EnumType.Zone => GetEnumList<Zone>(),
            EnumType.RelationUserFamily => GetEnumList<RelationUserFamily>(),
            EnumType.ResidenceStatusDha => GetEnumList<ResidenceStatusDha>(),
            EnumType.Phase => GetEnumList<Phase>(),
            EnumType.PropertyTypeCommercial => GetEnumList<PropertyTypeCommercial>(),
            EnumType.PropertyTypeResidential => GetEnumList<PropertyTypeResidential>(),
            EnumType.JobType => GetEnumList<JobType>(),
            EnumType.WorkerCardDeliveryType => GetEnumList<WorkerCardDeliveryType>(),
            EnumType.VisitorPassType => GetEnumList<VisitorPassType>(),
            _ => throw new ArgumentException("Invalid enum type")
        };
    }

    private List<EnumItemDto> GetEnumList<T>() where T : Enum
    {
        return Enum.GetValues(typeof(T))
            .Cast<T>()
            .Select(e => new EnumItemDto
            {
                Id = Convert.ToInt32(e),
                Name = e.ToString()
            }).ToList();
    }
}
