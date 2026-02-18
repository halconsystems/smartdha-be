using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;
using MediatR;


namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.AllUserFamily;

public class GetAllUserFamilyQuery : IRequest<Result<List<GetAllUserFamilyQueryResponse>>>
{
    public string Id { get; set; } = null!;
}
