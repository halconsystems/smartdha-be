using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Application.Feature.ResidenceProperty.Command.CreateProperty;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.Property.Command;

public class CreatePropertyCommand : IRequest<CreatePropertyResponse>
{
    public int Category { get; set; }
    public int Type { get; set; }
    public int Phase { get; set; }
    public int Zone { get; set; }
    public string Khayaban { get; set; } = string.Empty;
    public int Floor { get; set; } = 0;
    public string StreetNo { get; set; } = string.Empty;
    public int PlotNo { get; set; } = 0;
    public string Plot { get; set; } = string.Empty;
    public int PossessionType { get; set; }
    public IFormFile? ProofOfPossessionImage { get; set; } 
    public IFormFile? UtilityBillAttachment { get; set; } 
}
