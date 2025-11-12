using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.ComplaintsManagement.Commands.CreateComplaint;
public class CreateComplaintRequest
{
    public string Title { get; set; } = default!;
    public string Notes { get; set; } = default!;
    public string CategoryCode { get; set; } = default!;
    public string PriorityCode { get; set; } = default!;
    public double? Lat { get; set; }
    public double? Lng { get; set; }

    // Multiple images from mobile (form-data)
    public List<IFormFile>? Images { get; set; }
}

