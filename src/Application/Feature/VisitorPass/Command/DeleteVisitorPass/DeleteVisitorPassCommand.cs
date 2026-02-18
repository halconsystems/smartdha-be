using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.VisitorPass.Command.DeleteVisitorPass;

public class DeleteVisitorPassCommand : IRequest<Result<DeleteVisitorPassResponse>>
{
    public Guid Id { get; set; }
}
