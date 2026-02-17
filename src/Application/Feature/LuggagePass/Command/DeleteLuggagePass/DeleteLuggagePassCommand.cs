using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.DeleteLuggagePass;

public class DeleteLuggagePassCommand : IRequest<DeleteLuggagePassResponse>
{
    public Guid Id { get; set; }
}
