namespace SportBookingWebAPI.Dtos
{
    public class BookingViewModel
    {
        public int BookingId { get; set; }
        public string CourtName { get; set; }
        public string SubCourtName { get; set; }
        public string Address { get; set; }
        public double? PricePerHour { get; set; }
        public string FullName { get; set; }
        public string Status { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Description { get; set; }
    }
}
