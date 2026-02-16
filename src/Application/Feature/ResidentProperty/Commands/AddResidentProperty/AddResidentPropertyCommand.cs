using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.ResidentProperty.Commands.AddResidentProperty;
public class AddResidentPropertyCommand : IRequest<AddResidentPropertyResponse>
{
    public CategoryType CategoryType { get; set; } 
    public PropertyType Type{ get; set; } 
    public Phase Phase { get; set; } 
    public Zone Zone { get; set; } 
    public string StreetNo { get; set; } = string.Empty;
    public string Khayaban { get; set; } = string.Empty;
    public int Floor { get; set; } 
    public int PlotNo { get; set; } 
    public string Plot { get; set; } = string.Empty; 
    public IFormFile? ProofOfPossession  { get; set; } 
    public IFormFile?  UtilityBillAttachment { get; set; } 
}
