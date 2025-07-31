using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.ApprovalRequest.Queries.GetRequestTrackings;
public class PlotWithTrackingDto
{
    // 📌 Plot Info
    public string? PLot_ID { get; set; }
    public string? PLOT_NO { get; set; }
    public string? STNAME { get; set; }
    public string? PLTNO { get; set; }
    public string? SUBDIV { get; set; }
    public string? PTYPE { get; set; }
    public string? PHASE { get; set; }
    public string? EXT { get; set; }
    public decimal? NOMAREA { get; set; }
    public decimal? ACTUAL_SIZE { get; set; }
    public string? STREET1COD { get; set; }
    public string? PLOTPK { get; set; }
    public string? MEMPK { get; set; }

    // 📌 Tracking Info
    public long TrackingId { get; set; }
    public string? TrackingStatus { get; set; }
    public string? TrackingRemarks { get; set; }
    public string? TrackingDescription { get; set; }
    public bool IsCompleted { get; set; } = false;
    public string? ProcessName { get; set; } = string.Empty;
}

