using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using SportBookingWebAPI.Dtos;
using System.Net.Http.Headers;

namespace SportBookingWebClient.Controllers
{
	public class BookingController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly HttpClient client;
		private readonly string apiUrl = "https://localhost:7071/api/Booking";
		private readonly string apiCateUrl = "";


		public BookingController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			client = httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

        // Action để hiển thị danh sách booking của người dùng
        public async Task<IActionResult> ViewBookingCus(int userId)
        {
            var response = await client.GetAsync($"{apiUrl}/{userId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var bookings = JsonSerializer.Deserialize<List<BookingViewModel>>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return View("ViewBookingCus", bookings);  // Đảm bảo trả về View với tên 'ViewBookingCus'
            }
            else
            {
                ViewBag.ErrorMessage = "Không thể lấy dữ liệu từ API. Vui lòng thử lại sau.";
                return View("ViewBookingCus", new List<BookingViewModel>());
            }
        }



    }
}
