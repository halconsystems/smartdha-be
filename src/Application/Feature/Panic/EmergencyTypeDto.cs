using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
// Application/Panic/DTOs/EmergencyTypeDto.cs
public record EmergencyTypeDto(Guid Id, int Code, string Name, string? HelplineNumber, string? Description, bool? IsActive);


