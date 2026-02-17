using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LaggagePass.Command.DeleteLaggagePass;

public class DeleteLaggagePassCommand : IRequest<DeleteLaggagePassResponse>
{
    public Guid Id { get; set; }
}
