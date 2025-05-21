namespace SportBookingWebAPI.Dtos
{
    public class CourtEditModel
    {
        public int CourtId { get; set; }
        public string CourtName { get; set; }
        public string Address { get; set; }
        public Double PricePerHour { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int CategoryId { get; set; }
        public IFormFile? ImageFile { get; set; }
    }
}