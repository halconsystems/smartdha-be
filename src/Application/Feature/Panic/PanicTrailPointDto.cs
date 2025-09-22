using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic;
public record PanicTrailPointDto(DateTime RecordedAtUtc, decimal Lat, decimal Lng, float? Accuracy);
