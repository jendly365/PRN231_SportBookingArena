using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SportBookingWebAPI.Controllers;
using SportBookingWebAPI.Dtos;
using System.Text;

namespace SportBookingWebClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiBaseUrl = "https://localhost:7071/api/auth/login"; // Thay bằng URL API thực tế của bạn
        private readonly string _apiRegister = "https://localhost:7071/api/auth"; // Thay bằng URL API thực tế của bạn

        public AuthController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
		public async Task<IActionResult> Login(LoginRequest request)
		{
			try
			{
				if (!ModelState.IsValid)
				{
					return View(request);
				}

				var json = JsonConvert.SerializeObject(request);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				// Gọi API đăng nhập
				var response = await _httpClient.PostAsync($"{_apiBaseUrl}/api/auth/login", content);

				if (!response.IsSuccessStatusCode)
				{
					ModelState.AddModelError("", "Invalid credentials");
					return View(request);
				}

				var responseContent = await response.Content.ReadAsStringAsync();
				var result = JsonConvert.DeserializeObject<dynamic>(responseContent);
				string token = result?.token;

				if (string.IsNullOrEmpty(token))
				{
					ModelState.AddModelError("", "Token not received from server.");
					return View(request);
				}

				// Lưu token vào session
				HttpContext.Session.SetString("UserToken", token);

				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
				ModelState.AddModelError("", $"An error occurred: {ex.Message}");
				return View(request);
			}
		}



		[HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiRegister}/register", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login", "Auth");
            }

            TempData["Error"] = "Đăng ký thất bại! Email có thể đã tồn tại.";
            return View(model);
        }


        [HttpGet]
        public IActionResult RegisterOwner()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterOwner(RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync($"{_apiRegister}/registerOwner", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Đăng ký thành công!";
                return RedirectToAction("Login", "Auth");
            }

            TempData["Error"] = "Đăng ký thất bại! Email có thể đã tồn tại.";
            return View(model);
        }
    }
}
