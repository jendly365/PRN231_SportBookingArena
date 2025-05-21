namespace SportBookingWebAPI.Dtos
{
    public class CreateBookingDTO
    {
        public int userID { get; set; }
        public int SubCourtId { get; set; }
        public int CourtTypeId { get; set; }
        public string? Description { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public String CheckoutImage { get; set; }
    }
}
