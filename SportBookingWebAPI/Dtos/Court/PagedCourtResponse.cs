namespace SportBookingWebAPI.Dtos.Court
{
	public class PagedCourtResponse
	{
		public List<ListFeatureCourtDTO> Courts { get; set; } = new List<ListFeatureCourtDTO>();
		public int TotalPages { get; set; }
		public int PageNumber { get; set; }
	}
}
