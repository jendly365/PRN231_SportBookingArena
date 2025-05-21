namespace SportBookingWebAPI.Dtos.Booking
{
    public class BookingDTO
    {
        public int? BookingId { get; set; }
        public int? SubCourtId { get; set; }
        public int? CourtTypeId { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
    }
}
