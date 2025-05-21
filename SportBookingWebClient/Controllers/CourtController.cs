using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using SportBookingWebAPI.Dtos.Court;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SportBookingWebClient.Controllers
{
	public class CourtController : Controller
	{
		private readonly ILogger<CourtController> _logger;
		private readonly HttpClient client;
		private readonly HttpClient _client;
		private readonly string apiUrl = "https://localhost:7071/api/Court";
		private readonly string apiCateUrl = "https://localhost:7071/api/Category";
		private readonly string subCourturl = "https://localhost:7071/api/SubCourt";


		public CourtController(ILogger<CourtController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			client = httpClientFactory.CreateClient();
			var contentType = new MediaTypeWithQualityHeaderValue("application/json");
			_client = new HttpClient();
			_client.DefaultRequestHeaders.Accept.Add(contentType);
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 6, string searchQuery = "", List<int>? selectedCategoryIds = null)
		{
			string DTOUrl = $"{apiUrl}?pageNumber={pageNumber}&pageSize={pageSize}";

			if (!string.IsNullOrEmpty(searchQuery))
			{
				DTOUrl += $"&searchQuery={searchQuery}";
			}

			if (selectedCategoryIds != null && selectedCategoryIds.Any())
			{
				string categoryFilter = string.Join(",", selectedCategoryIds); // Định dạng đúng
				DTOUrl += $"&categoryId={categoryFilter}"; // Sửa tham số API cho đúng
			}

			HttpResponseMessage response = await client.GetAsync(DTOUrl);

			if (!response.IsSuccessStatusCode)
			{
				return View(new PagedCourtResponse
				{
					Courts = new List<ListFeatureCourtDTO>(),
					TotalPages = 1,
					PageNumber = pageNumber
				});
			}

			string strData = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var result = JsonSerializer.Deserialize<PagedCourtResponse>(strData, options);

			ViewBag.SearchQuery = searchQuery;
			ViewBag.Categories = await getCategory();
			ViewBag.SelectedCategoryIds = selectedCategoryIds ?? new List<int>();

			return View(result);
		}

		public async Task<List<Category>> getCategory()
		{
			string DTOUrl = $"{apiCateUrl}";
			HttpResponseMessage response = await client.GetAsync(DTOUrl);

			if (!response.IsSuccessStatusCode)
			{
				return new List<Category>(); // Trả về danh sách rỗng thay vì View
			}

			string strData = await response.Content.ReadAsStringAsync();
			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			List<Category> data = JsonSerializer.Deserialize<List<Category>>(strData, options);

			return data; // Trả về danh sách thay vì View
		}

		[HttpPost]
		public async Task<IActionResult> Filter(string searchQuery, List<int> selectedCategoryIds)
		{
			return await Index(1, 6, searchQuery, selectedCategoryIds);
		}
		public async Task<IActionResult> getCourtDetail(int id)
		{
			string DTOUrl = $"{apiUrl}/{id}";
			Console.WriteLine(DTOUrl);

			HttpResponseMessage response = await _client.GetAsync(DTOUrl);

			if (!response.IsSuccessStatusCode)
			{
				return NotFound("Không tìm thấy sân!");
			}

			string strData = await response.Content.ReadAsStringAsync();
			Console.WriteLine(strData);

			var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
			var courtDetail = JsonSerializer.Deserialize<CourtDetailDTO>(strData, options);

			if (courtDetail?.SubCourts == null || courtDetail?.Reviews == null)
			{
				Console.WriteLine("SubCourts hoặc Reviews bị null sau khi Deserialize!");
			}
			return View("CourtDetail", courtDetail);
		}


		public async Task<IActionResult> CourtDetail(int id)
		{
			return await getCourtDetail(id); // ✅ Dùng await để lấy kết quả chính xác
		}

        public async Task<IActionResult> getCourtDetailBooking(int id, DateTime? date)
        {
            date ??= DateTime.Today; // Nếu user không chọn ngày, mặc định là hôm nay
            string DTOUrl = $"{apiUrl}/listforbooking/{id}/{date.Value:yyyy-MM-dd}";
            Console.WriteLine(DTOUrl);

            HttpResponseMessage response = await _client.GetAsync(DTOUrl);

            if (!response.IsSuccessStatusCode)
            {
                return NotFound("Không tìm thấy sân!");
            }

            string strData = await response.Content.ReadAsStringAsync();
            Console.WriteLine(strData);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var courtDetail = JsonSerializer.Deserialize<CourtDetailDTO>(strData, options);

            if (courtDetail?.SubCourts == null)
            {
                Console.WriteLine("SubCourts bị null sau khi Deserialize!");
            }
            return View("CourtDetailBooking", courtDetail);
        }



        public async Task<IActionResult> CourtDetailBooking(int id, DateTime? date)
        {
            return await getCourtDetailBooking(id, date);
        }


    }
}
