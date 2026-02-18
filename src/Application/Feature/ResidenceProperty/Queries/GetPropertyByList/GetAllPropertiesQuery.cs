using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Queries.GetPropertyByList;

namespace DHAFacilitationAPIs.Application.Feature.Property.Queries;

public class GetAllPropertiesQuery : IRequest<Result<List<GetAllPropertiesResponse>>>
{
    public string Id { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

