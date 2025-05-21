namespace SportBookingWebAPI.Dtos.Court
{
    public class ListFeatureCourtDTO
    {
        public int CourtId { get; set; }
        public int? CategoryId { get; set; }
        public int? UserId { get; set; }
        public string? CourtName { get; set; }
        public string? Address { get; set; }
        public string? CourtDescription { get; set; }
        public int? Quantity { get; set; }
        public double? PricePerHour { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsFeatured { get; set; }
        public string? CategoryName { get; set; }
        public string? FullName { get; set; }
        public string? LinkMap { get; set; }
        public string? Location { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

    }
}
