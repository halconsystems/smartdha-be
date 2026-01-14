using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace DHAFacilitationAPIs.Application.Feature.PMS.Commands.AddCaseAttachment;
public class AddCaseAttachmentsForm
{
    [Required]
    public int CaseId { get; set; } 

    [Required]
    public List<IFormFile> Files { get; set; } = new();

    [Required]
    public List<int> PrerequisitesIDs { get; set; } = new();
}

