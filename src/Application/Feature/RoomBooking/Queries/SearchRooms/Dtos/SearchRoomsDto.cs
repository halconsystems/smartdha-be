namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public class SearchRoomsDto
{
    public string ResidenceTypeName { get; set; } = default!;
    public string CategoryName { get; set; } = default!;
    public Guid RoomId { get; set; }
    public string Name { get; set; } = default!;
    public string RoomNo { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal Ratings { get; set; }
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public string DefaultImage { get; set; } = string.Empty;
}
