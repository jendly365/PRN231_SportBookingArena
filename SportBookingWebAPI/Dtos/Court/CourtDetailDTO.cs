using SportBookingWebAPI.Dtos.Booking;

namespace SportBookingWebAPI.Dtos.Court
{
	public class CourtDetailDTO
	{
		public int ? CourtID { get; set; }
		public string? CourtName { get; set; }
		public string? Address { get; set; }
		public string? CourtDescription { get; set; }
		public int? Quantity { get; set; }
		public double? PricePerHour { get; set; }
		public string? ImageUrl { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public string? LinkMap { get; set; }
		public string? CategoryName { get; set; }
		public string? FullName { get; set; }
		public string? PhoneNumber { get; set; }
        public OwnerDTO Owner { get; set; }
        // Danh sách SubCourts
        public List<SubCourtDTO>? SubCourts { get; set; } = new List<SubCourtDTO>();

		// Danh sách Comment (Reviews)
		public List<ReviewDTO>? Reviews { get; set; } = new List<ReviewDTO>();
        public List<BookingDTO> Bookings { get; set; } = new List<BookingDTO>();
    }

	public class SubCourtDTO
	{

        public int? SubCourtId { get; set; }
        public string? SubCourtName { get; set; }
        public string? CourtName { get; set; }
        public string? Desccription { get; set; }
        public string? Address { get; set; }
        public bool? Status { get; set; }

    }

    public class ReviewDTO
	{
		public string? Comment { get; set; }
		public DateTime? ReviewDate { get; set; }
		public string? FullName { get; set; }
		public string? PhoneNumber { get; set; }
	}
}
