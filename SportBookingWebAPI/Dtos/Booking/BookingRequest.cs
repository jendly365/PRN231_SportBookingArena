namespace SportBookingWebAPI.Dtos.Booking
{
    public class BookingRequest
    {
        public int UserId { get; set; }
        public int SubCourtId { get; set; }
        public string Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public IFormFile CheckoutImg { get; set; }
    }
}
