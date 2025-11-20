using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class SmartPayLog : BaseAuditableEntity
{
    public string? UserId { get; set; }

    public string ApiName { get; set; } = default!;

    public string RequestJson { get; set; } = default!;

    public string? ResponseJson { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime? RequestDateTime { get; set; }
    public DateTime? ResponseDateTime { get; set; }
    }
