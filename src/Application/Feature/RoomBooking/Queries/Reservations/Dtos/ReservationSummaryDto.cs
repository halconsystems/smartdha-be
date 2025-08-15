public class ReservationListDto
{
    public Guid ReservationId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<ReservationRoomListDto> Rooms { get; set; } = new();
}

public class ReservationRoomListDto
{
    public string? RoomNo { get; set; }
    public string? ClubName { get; set; }
    public string? ResidenceType { get; set; }
    public string? RoomCategory { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}
