using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.UserFamily.Queries.UserFamilyById;

public class GetUserFamilyByIdQuery : IRequest<Result<GetUserFamilybyIdQueryResponse>>
{
    public Guid Id { get; set; }
}
