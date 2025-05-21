using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SportBookingWebAPI.Dtos.Court;
using SportBookingWebClient.Models;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using BusinessObject.Models;

namespace SportBookingWebClient.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly HttpClient client;
		private readonly string apiUrl = "https://localhost:7071/api/Court";
		private readonly string apiCateUrl = "https://localhost:7071/api/Category";


		public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
		{
			_logger = logger;
			client = httpClientFactory.CreateClient();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}
		public async Task<IActionResult> Index()
		{
			string DTOUrl = $"{apiUrl}/featured";
				HttpResponseMessage response = await client.GetAsync(DTOUrl);
				if (!response.IsSuccessStatusCode)
				{
					return View(new List<ListFeatureCourtDTO>()); // Trả về danh sách rỗng để tránh lỗi
				}
				string strData = await response.Content.ReadAsStringAsync();
				var options = new JsonSerializerOptions
				{
					PropertyNameCaseInsensitive = true
				};
				List<ListFeatureCourtDTO> data = JsonSerializer.Deserialize<List<ListFeatureCourtDTO>>(strData, options);
			ViewBag.Categories =await getCategory();
				return View(data);
		
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

	}
}
