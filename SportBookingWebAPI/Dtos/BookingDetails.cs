namespace SportBookingWebAPI.Dtos
{
    public class BookingDetails
    {
        public int BookingID { get; set; }
        public int CustomerID { get; set; }
        public string? CustomerName { get; set; }
        public string? CustomerPhone { get; set; }
        public string? MainCourtName { get; set; }
        public string? SubCourtName { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int TotalHours { get; set; }
        public string? BookingStatus { get; set; }
        public double? PricePerHour { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? BookingDate { get; set; }
    }
}
