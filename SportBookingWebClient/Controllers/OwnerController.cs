using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SportBookingWebAPI.Dtos;
using SportBookingWebAPI.Dtos.Category;
using SportBookingWebAPI.Dtos.Court;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SportBookingWebClient.Controllers
{
    public class OwnerController : Controller
    {

        private readonly ILogger<CourtController> _logger;
        private readonly HttpClient client;
        private readonly HttpClient _client;
        private readonly string apiUrl = "https://localhost:7071/api/Court/listbyOwner";

        private readonly string apiCateUrl = "https://localhost:7071/api/Category";
        private readonly string BookingURL = "https://localhost:7071/api/Booking/owner";


        public OwnerController(ILogger<CourtController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            client = httpClientFactory.CreateClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Add(contentType);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        public IActionResult OwnerSite()
        {
            return View();
        }

        public IActionResult AddNewCourt()
        {
            return View();
        }

        public async Task<IActionResult> CourtList(int userId)
        {
            var apiUrl = $"https://localhost:7071/api/Court/listbyOwner?userId={userId}";
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<ListFeatureCourtDTO>());

            var jsonData = await response.Content.ReadAsStringAsync();
            var courtList = JsonConvert.DeserializeObject<List<ListFeatureCourtDTO>>(jsonData);

            // Gọi API lấy danh sách category
            var categoryResponse = await client.GetAsync("https://localhost:7071/api/Category");
            if (categoryResponse.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                ViewBag.Categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryData);
            }

            return View(courtList);
        }
        [HttpGet]
        public async Task<IActionResult> BookingList(int userId)
        {
            var apiUrl = $"{BookingURL}/{userId}";
            var response = await client.GetAsync(apiUrl);

            if (!response.IsSuccessStatusCode)
                return View(new List<BookingDetails>());

            var jsonData = await response.Content.ReadAsStringAsync();
            var courtList = JsonConvert.DeserializeObject<List<BookingDetails>>(jsonData);

            return View(courtList);  // Đảm bảo trả về View thay vì trả về JSON.
        }


        [HttpGet]
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
            List<Category> data = System.Text.Json.JsonSerializer.Deserialize<List<Category>>(strData, options);

            return data; // Trả về danh sách thay vì View
        }
        [HttpPost]
        public async Task<IActionResult> AddNewCourt(CourtCreateModel model)
        {
            if (!ModelState.IsValid)
            {
                // Gọi API lấy danh sách categories ngay khi kiểm tra dữ liệu không hợp lệ
                var categoryResponse = await client.GetAsync(apiCateUrl);
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var categoryData = await categoryResponse.Content.ReadAsStringAsync();
                    ViewBag.Categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryData);
                }
                return View(model); // Nếu không hợp lệ, giữ lại trang và hiển thị lỗi
            }

            try
            {
                // Thêm các dữ liệu và gọi API tạo sân
                using var formData = new MultipartFormDataContent();
                formData.Add(new StringContent(model.Name), "Name");
                formData.Add(new StringContent(model.Location), "Location");
                formData.Add(new StringContent(model.PricePerHour.ToString()), "PricePerHour");
                formData.Add(new StringContent(model.StartDate.ToString("o")), "StartDate");
                formData.Add(new StringContent(model.EndDate.ToString("o")), "EndDate");
                formData.Add(new StringContent(model.CategoryId.ToString()), "CategoryId");

                // Thêm ảnh vào request nếu có
                if (model.Image != null && model.Image.Length > 0)
                {
                    using var stream = model.Image.OpenReadStream();
                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.Image.ContentType);
                    formData.Add(fileContent, "Image", model.Image.FileName);
                }

                // Gửi yêu cầu POST đến API
                var apiUrl = "https://localhost:7071/api/Court/addnewcourt";
                var response = await client.PostAsync(apiUrl, formData);

                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Message = "Thêm sân thành công!";
                    ModelState.Clear();
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Lỗi API: {errorMessage}");
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Lỗi hệ thống: {ex.Message}");
            }

            // Sau khi gửi form, gọi API lấy danh sách categories lại
            var categoryResponseAfterSubmit = await client.GetAsync(apiCateUrl);
            if (categoryResponseAfterSubmit.IsSuccessStatusCode)
            {
                var categoryData = await categoryResponseAfterSubmit.Content.ReadAsStringAsync();
                ViewBag.Categories = JsonConvert.DeserializeObject<List<CategoryDto>>(categoryData);
            }

            return View(model);
        }







    }
}
