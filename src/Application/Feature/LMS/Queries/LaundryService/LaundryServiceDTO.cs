using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.LMS.Queries.LaundryService;

public class LaundryServiceDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; }    
    public string? DisplayName { get; set; }
    public string? Code { get; set; }
}
