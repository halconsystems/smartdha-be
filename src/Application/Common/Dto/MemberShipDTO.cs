using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DHAFacilitationAPIs.Application.Common.Dto;

public class MemberShipDTO
{
    public string? MemPk { get; set; }  
    public string? FatherName { get; set; }
    public string? FatherCnic {  get; set; }
    public string? FatherPhoto {  get; set; }
    public string? Motherame { get; set; }
    public string? Mothernic { get; set; }
    public string? MotherPhoto { get; set; }

    public string? PhoneNumber { get; set; }
    public string? PresentPostalAddress {  get; set; }
    public string? Email {  get; set; }

    public List<ChildrenDTO>? AdultChildren {  get; set; }
    public List<ChildrenDTO>? teeenChildren {  get; set; }

}

public class ChildrenDTO
{
    public string? Name { get; set; }
    public string? Cnic { get; set; }
    public string? Photo { get; set; }
    public string? RegistrationNo { get; set; }
}
