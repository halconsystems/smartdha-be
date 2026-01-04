using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Panic.Commands.CreatePanicReview;
public class CreatePanicReviewRequest
{
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
}
