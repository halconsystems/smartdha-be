using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Proeperty.Queries.GetPropertyByMemPK;
public class PlotDto
{
    public string PLot_ID { get; set; } = string.Empty;
    public string PLOT_NO { get; set; } = string.Empty;
    public string STNAME { get; set; } = string.Empty;
    public string PLTNO { get; set; } = string.Empty;
    public string SUBDIV { get; set; } = string.Empty;
    public string PTYPE { get; set; } = string.Empty;
    public string PHASE { get; set; } = string.Empty;
    public string EXT { get; set; } = string.Empty;
    public decimal? NOMAREA { get; set; }
    public decimal? ACTUAL_SIZE { get; set;}
    public string STREET1COD { get; set; } = string.Empty;
    public string PLOTPK { get; set; } = string.Empty;
    public string MEMPK { get; set; } = string.Empty;
    public string OwnerName { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty ;
}

