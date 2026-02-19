using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Queries.GetVisitorPassPdfQuery;
public class GetVisitorPassPdfQuery : IRequest<Result<string>>
{
    public Guid Id { get; set; }
}
