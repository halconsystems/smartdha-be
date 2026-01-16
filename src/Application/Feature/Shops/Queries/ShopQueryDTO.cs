using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Feature.Shops.Queries;

public class ShopQueryDTO
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public string? DisplayName { get; set; } 
    public string? Code { get; set; }
    public string? Address { get; set; }
    public float? Latitude { get; set; } 
    public float? Longitude { get; set; } 
    public string? OwnerName { get; set; } 
    public string? OwnerEmail { get; set; } 
    public string? OwnerPhone { get; set; }
    public string? ShopPhoneNumber { get; set; }
}
