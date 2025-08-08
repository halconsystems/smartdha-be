namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.Rooms;

public class AvailableRoomDto
{
    public Guid RoomId { get; set; }
    public string? Name { get; set; } = default!;
    public string? No { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Ratings { get; set; }
    public List<string> Images { get; set; } = new();
}
