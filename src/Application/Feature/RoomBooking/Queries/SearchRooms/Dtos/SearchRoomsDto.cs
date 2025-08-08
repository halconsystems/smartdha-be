namespace DHAFacilitationAPIs.Application.Feature.RoomBooking.Queries.SearchRooms;

public class SearchRoomsDto
{
    public Guid CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public List<ResidenceTypeDto> ResidenceTypes { get; set; } = new();
}

public class ResidenceTypeDto
{
    public Guid ResidenceTypeId { get; set; }
    public string? ResidenceTypeName { get; set; }
    public List<RoomDto> Rooms { get; set; } = new();
}

public class RoomDto
{
    public Guid RoomId { get; set; }
    public string? Name { get; set; }
    public string? RoomNo { get; set; }
    public decimal Price { get; set; }
    public decimal? Ratings { get; set; }
    public List<string> Images { get; set; } = new();
}
