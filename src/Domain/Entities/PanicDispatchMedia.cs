using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Domain.Entities;
public class PanicDispatchMedia : BaseAuditableEntity
{
    public Guid PanicDispatchId { get; set; }
    public PanicDispatch PanicDispatch { get; set; } = default!;

    public string FilePath { get; set; } = default!;   // relative path
    public PanicDispatchMediaType MediaType { get; set; }

    public string? Caption { get; set; }               // optional
}

