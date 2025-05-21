namespace SportBookingWebAPI.Dtos
{
    public class CourtCreateModel
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public double PricePerHour { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public IFormFile? Image { get; set; }
        public int CategoryId { get; set; }
    }
}
