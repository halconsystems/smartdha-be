using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LuggagePass.Command.CreateLuggagePass;

public class CreateLuggagePassResponse
{
    public Guid Id { get; set; }
    public string Message { get; set; } = string.Empty;
}

