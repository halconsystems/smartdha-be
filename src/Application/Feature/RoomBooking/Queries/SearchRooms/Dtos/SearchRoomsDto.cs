namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public class SearchRoomsDto
{
    public string ResidenceTypeName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public Guid RoomId { get; set; }
    public string Name { get; set; } = default!;
    public string RoomNo { get; set; } = default!;
    public decimal? Price { get; set; } = default;
    public decimal? Ratings { get; set; } = default;
    public DateOnly CheckInDate { get; set; }
    public TimeOnly CheckInTimeOnly { get; set; } // Just the time portion
    public DateOnly CheckOutDate { get; set; }
    public TimeOnly CheckOutTimeOnly { get; set; } // Just the time portion
    public string DefaultImage { get; set; } = string.Empty;
    public DateTime AvailabilityFrom { get; set; } = default!;
    public DateTime AvailabilityTo { get; set; } = default!;

}
