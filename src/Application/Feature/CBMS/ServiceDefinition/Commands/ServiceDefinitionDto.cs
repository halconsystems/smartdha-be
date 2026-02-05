using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.CBMS.ServiceDefinition.Commands;
public record ServiceDefinitionDto(
    Guid Id,
    Guid ClubServiceCategoryId,
    string CategoryName,
    string Name,
    string Code,
    bool IsQuantityBased,
    bool? IsActive
);

public record CreateServiceDefinitionDto(
    Guid ClubServiceCategoryId,
    string Name,
    string Code,
    bool IsQuantityBased
);

public record UpdateServiceDefinitionDto(
    Guid Id,
    Guid ClubServiceCategoryId,
    string Name,
    string Code,
    bool IsQuantityBased,
    bool IsActive
);

