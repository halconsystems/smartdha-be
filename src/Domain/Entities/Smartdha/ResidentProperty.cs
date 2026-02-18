using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.Smartdha;
public class ResidentProperty : BaseAuditableEntity
{
    public CategoryType Category { get; set; }
    public PropertyType Type { get; set; }
    public Phase Phase { get; set; }
    public Zone Zone { get; set; }
    public string StreetNo  { get; set; } = string.Empty;
    public string Khayaban  { get; set; } = string.Empty;
    public int Floor { get; set; }
    public int PlotNo { get; set; }
    public string Plot { get; set; } = string.Empty;
    public DateTime? PropertyTagDate { get; set; }
    public ResidenceStatusDha PossessionType { get; set; }
    public string? ProofOfPossession { get; set; }
    public string? UtilityBillAttachment { get; set; } 
}
