using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Common.Models;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.DeleteLuggagePass;

public class DeleteLuggagePassCommand : IRequest<Result<DeleteLuggagePassResponse>>
{
    public Guid Id { get; set; }
}
