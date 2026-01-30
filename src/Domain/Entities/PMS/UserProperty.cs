using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities.PMS;
public class UserProperty : BaseAuditableEntity
{
    // 🔹 Property identifiers
    [MaxLength(50)]
    public string? PlotNo { get; set; }          // PLOT_NO

    [MaxLength(50)]
    public string? PlotNoAlt { get; set; }       // PLTNO

    [MaxLength(100)]
    public string? StreetName { get; set; }      // STNAME

    [MaxLength(100)]
    public string? SubDivision { get; set; }     // SUBDIV

    [MaxLength(50)]
    public string? PropertyType { get; set; }    // PTYPE

    [MaxLength(50)]
    public string? Phase { get; set; }            // PHASE

    [MaxLength(50)]
    public string? Extension { get; set; }        // EXT

    [MaxLength(50)]
    public string? NominalArea { get; set; }      // NOMEAEA

    [MaxLength(50)]
    public string? ActualSize { get; set; }       // ACTUAL_SIZE

    [MaxLength(50)]
    public string? StreetCode { get; set; }       // STREET1COD

    // 🔹 DHA internal keys
    public string? PropertyPk { get; set; }       // PLOTPK
    public string? MemberPk { get; set; }         // MEMPK

    [MaxLength(20)]
    public string? MemberNo { get; set; }         // MEMNO

    [MaxLength(50)]
    public string? Category { get; set; }         // CAT

    [MaxLength(150)]
    public string? MemberName { get; set; }       // NAME

    public DateTime? ApplicationDate { get; set; } // APPLIDATE

    [MaxLength(20)]
    public string? OwnerCnic { get; set; }        // NIC

    [MaxLength(20)]
    public string? CellNo { get; set; }            // CELLNO

    public bool? AllResidentialPlot { get; set; } // ALLRESPLOT
}


