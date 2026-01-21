using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DHAFacilitationAPIs.Domain.Entities;
using DHAFacilitationAPIs.Domain.Entities.GBMS;
using DHAFacilitationAPIs.Domain.Entities.LMS;
using DHAFacilitationAPIs.Domain.Enums;
using DHAFacilitationAPIs.Domain.Enums.GBMS;

namespace DHAFacilitationAPIs.Application.Feature.GroundReservations.Queries;

public class BookingDTO
{
    public string? BookingCode { get; set; }
    public string? GroundName { get; set; }
    public string? BookingDescription { get; set; }
    public string? GroundType { get; set; }
    public string? GroundCategory { get; set; }
    public Guid Id { get; set; }
    public Guid GroundId    { get; set; }
    public DateTime BookingDate { get; set; }
    public string? TotalPrice { get; set; }
    public string? SlotsCount { get; set; }
    public List<GroundBookingSlot>? Slots { get; set; }
    public List<Guid>? OrderDTiD { get; set; }
    public List<GroundSetting>? GroundSettings { get; set; }
    public List<GroundImages>? GroundImages { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
    public string? MainImage { get; set; }

}
