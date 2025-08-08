namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.RoomDetails;

public class RoomDetailsDto
{
    public string? Name { get; set; } = default!;
    public string No { get; set; } = default!;
    public decimal Price { get; set; }
    public decimal? Ratings { get; set; }
    public List<string> Images { get; set; } = new();
    public List<string>? Services { get; set; } = new();
}
