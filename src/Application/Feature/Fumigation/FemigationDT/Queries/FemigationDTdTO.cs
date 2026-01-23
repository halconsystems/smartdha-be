using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;

namespace DHAFacilitationAPIs.Application.Feature.Fumigation.FemigationDT.Queries;

public class FemigationDTdTO
{
    public Guid Id { get; set; }
    public Settings Name { get; set; }
    public Domain.Enums.ValueType ValueType { get; set; }
    public string? Value { get; set; }
    public string? Code { get; set; }
    public string? DisplayName { get; set; }
    public bool IsDiscount { get; set; }
    public bool? IsActive { get; set; }
}
