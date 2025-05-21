using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportBookingWebAPI.Dtos;
using SportBookingWebAPI.Dtos.Booking;
using SportBookingWebAPI.Dtos.Court;
using System.Security.Claims;

namespace SportBookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourtController : ControllerBase
    {
        private readonly EXE201_Rental_Sport_Field1Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public CourtController(EXE201_Rental_Sport_Field1Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }


        [HttpGet("featured")]
		public async Task<ActionResult<IEnumerable<ListFeatureCourtDTO>>> GetFeaturedCourts()
		{
			var courts = await _context.Courts
				.Where(c => c.IsFeatured == true)
				.OrderByDescending(c => c.CourtId) // Sắp xếp giảm dần để lấy top 6 mới nhất
				.Take(6) // Giới hạn chỉ lấy 6 mục đầu tiên
				.Select(c => new ListFeatureCourtDTO
				{
					CourtId = c.CourtId,
					CategoryId = c.CategoryId,
					UserId = c.UserId,
					CourtName = c.CourtName,
					Address = c.Address,
					CourtDescription = c.CourtDescription,
					Quantity = c.Quantity,
					PricePerHour = c.PricePerHour,
					ImageUrl = c.ImageUrl,
					IsFeatured = c.IsFeatured,
					CategoryName = c.Category.CategoryName,
					FullName = c.User.FullName,
					Location = c.Address,
					LinkMap = c.LinkMap,
				})
				.ToListAsync();

			if (courts == null || courts.Count == 0)
			{
				return NotFound("Không có sân nào được đánh dấu nổi bật.");
			}

			return Ok(courts);
		}



        [HttpGet]
        public async Task<ActionResult<IEnumerable<ListFeatureCourtDTO>>> GetCourtList(
    [FromQuery] List<int>? categoryIds = null,
    string? searchQuery = null,
    int pageNumber = 1,
    int pageSize = 6)
        {
            if (pageNumber < 1 || pageSize < 1)
            {
                return BadRequest("Số trang và kích thước trang phải lớn hơn 0.");
            }

            var query = _context.Courts.AsQueryable();

            // Lọc theo danh sách CategoryId (nếu có)
            if (categoryIds != null && categoryIds.Any())
            {
                query = query.Where(c => categoryIds.Contains(c.CategoryId ?? 0));
            }

            // Lọc theo từ khóa tìm kiếm (nếu có)
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.CourtName.Contains(searchQuery));
            }

            // Tổng số bản ghi sau khi lọc
            var totalRecords = await query.CountAsync();

            // Áp dụng phân trang và lấy dữ liệu
            var courts = await query
                .OrderByDescending(c => c.CourtId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ListFeatureCourtDTO
                {
                    CourtId = c.CourtId,
                    CategoryId = c.CategoryId,
                    UserId = c.UserId,
                    CourtName = c.CourtName,
                    Address = c.Address,
                    CourtDescription = c.CourtDescription,
                    Quantity = c.Quantity,
                    PricePerHour = c.PricePerHour,
                    ImageUrl = c.ImageUrl,
                    IsFeatured = c.IsFeatured,
                    CategoryName = c.Category.CategoryName,
                    FullName = c.User.FullName,
                    Location = c.Address,
                    LinkMap = c.LinkMap,
                })
                .ToListAsync();

            // Trả về kết quả
            var response = new
            {
                TotalRecords = totalRecords,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
                Courts = courts
            };

            return Ok(response);
        }

		[HttpGet("{id}")]
		public async Task<ActionResult<CourtDetailDTO>> GetCourtDetail(int id)
		{
			if (id == 0)
			{
				return BadRequest("Invalid Court ID");
			}

			var court = await _context.Courts
				.Include(c => c.Category)
				.Include(c => c.User)
				.Include(c => c.SubCourts)
				.ThenInclude(s => s.Reviews)
				.FirstOrDefaultAsync(c => c.CourtId == id);

			if (court == null)
			{
				return NotFound("Court not found");
			}

			var courtDetail = new CourtDetailDTO
			{
				CourtID = court.CourtId,
				CourtName = court.CourtName,
				Address = court.Address,
				CourtDescription = court.CourtDescription,
				Quantity = court.Quantity,
				PricePerHour = court.PricePerHour,
				ImageUrl = court.ImageUrl,
				StartTime = court.StartTime,
				EndTime = court.EndTime,
				LinkMap = court.LinkMap,
				CategoryName = court.Category.CategoryName,
				FullName = court.User.FullName,
				PhoneNumber = court.User.PhoneNumber,
				// Lấy danh sách SubCourts
				SubCourts = court.SubCourts.Select(subCourt => new SubCourtDTO
				{
					SubCourtName = subCourt.SubCourtName,
				}).ToList(),

				// Lấy danh sách Review (Comments)
				Reviews = court.SubCourts
					.SelectMany(s => s.Reviews)
					.Select(review => new ReviewDTO
					{
						Comment = review.Comment,
						ReviewDate = review.ReviewDate,
						FullName = review.User?.FullName,
						PhoneNumber = review.User?.PhoneNumber
					}).ToList()
			};

			return Ok(courtDetail);
		}

        [HttpGet("listforbooking/{id}/{date}")]
        public async Task<ActionResult<CourtDetailDTO>> GetCourtBooking(int id, DateTime date)
        {
            if (id == 0)
            {
                return BadRequest("Invalid Court ID");
            }

            var court = await _context.Courts
                .Include(c => c.Category)
                .Include(c => c.SubCourts)
                .Include(c => c.User) // 👈 Include thông tin người chủ
                .FirstOrDefaultAsync(c => c.CourtId == id);

            if (court == null)
            {
                return NotFound("Court not found");
            }

            // Set khoảng thời gian lọc theo ngày được truyền vào
            var dayStart = date.Date;
            var dayEnd = dayStart.AddDays(1);

            var bookingsOnDate = await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.SubCourt)
                .Where(b => b.StartTime >= dayStart && b.StartTime < dayEnd && b.SubCourt.CourtId == id)
                .Select(b => new BookingDTO
                {
                    BookingId = b.BookingId,
                    SubCourtId = b.SubCourtId,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    BookingDate = b.StartTime.HasValue ? b.StartTime.Value.Date : (DateTime?)null
                })
                .ToListAsync();

            var courtDetailbooking = new CourtDetailDTO
            {
                CourtName = court.CourtName,
                PricePerHour = court.PricePerHour,
                StartTime = court.StartTime,
                EndTime = court.EndTime,
                SubCourts = court.SubCourts.Select(subCourt => new SubCourtDTO
                {
                    CourtName = subCourt.Court.CourtName,
                    SubCourtId = subCourt.SubCourtId,
                    SubCourtName = subCourt.SubCourtName,
                    Address =subCourt.Court.Address,
                    Status = subCourt.Status,
                }).ToList(),

                Bookings = bookingsOnDate,

                Owner = court.User == null ? null : new Dtos.OwnerDTO
                {
                    UserId = court.User.UserId,
                    FullName = court.User.FullName,
                    Email = court.User.Email,
                    Phone = court.User.PhoneNumber,
                    Qr = court.User.Qr

                }
            };

            return Ok(courtDetailbooking);
        }

        [HttpPost("bookings")]
        public async Task<IActionResult> CreateBooking([FromForm] BookingRequest bookingRequest)
        {
            if (bookingRequest == null)
            {
                return BadRequest("Invalid data.");
            }

            var booking = new Booking
            {
                UserId = bookingRequest.UserId,
                SubCourtId = bookingRequest.SubCourtId,
                Status = "Pending",
                Description = bookingRequest.Description,
                CreatedAt = DateTime.Now,
                StartTime = bookingRequest.StartTime,
                EndTime = bookingRequest.EndTime,
                CheckoutImg = bookingRequest.CheckoutImg != null ? SaveImage(bookingRequest.CheckoutImg) : null,
                CourtTypeId = 1  // Gán CourtTypeId = 1 tự động
            };

            try
            {
                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBookingById), new { id = booking.BookingId }, booking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.InnerException?.Message ?? ex.Message });
            }
        }


        // GET: api/court/bookings/{id}
        [HttpGet("bookings/{id}")]
        public async Task<ActionResult<Booking>> GetBookingById(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);

            if (booking == null)
            {
                return NotFound();
            }

            return booking;
        }

        // Hàm lưu ảnh lên server (hoặc có thể lưu vào cloud storage)
        private string SaveImage(IFormFile file)
        {
            var imagesDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "assets", "images");

            // Kiểm tra xem thư mục có tồn tại chưa, nếu chưa thì tạo
            if (!Directory.Exists(imagesDirectory))
            {
                Directory.CreateDirectory(imagesDirectory);
            }

            var filePath = Path.Combine(imagesDirectory, file.FileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return filePath; // Trả về đường dẫn ảnh đã lưu
        }


        [HttpGet("listbyOwner")]
        public async Task<ActionResult<IEnumerable<ListFeatureCourtDTO>>> GetCourtListByOwner([FromQuery] int userId)
        {
            if (userId <= 0)
            {
                return BadRequest("User ID không hợp lệ.");
            }

            var courts = await _context.Courts
                .Where(c => c.UserId == userId)
                .Select(c => new ListFeatureCourtDTO
                {
                    CourtId = c.CourtId,
                    CategoryId = c.CategoryId,
                    UserId = c.UserId,
                    CourtName = c.CourtName,
                    Address = c.Address,
                    CourtDescription = c.CourtDescription,
                    Quantity = _context.SubCourts.Count(sc => sc.CourtId == c.CourtId),
                    PricePerHour = c.PricePerHour,
                    ImageUrl = c.ImageUrl,
                    IsFeatured = c.IsFeatured,
                    CategoryName = c.Category.CategoryName,
                    FullName = c.User.FullName,
                    Location = c.Address,
                    LinkMap = c.LinkMap,
                    StartTime =c.StartTime,
                    EndTime =c.EndTime,
                })
                .ToListAsync();

            return Ok(courts);
        }


        [HttpPost]
        [Route("edit")]
        public async Task<IActionResult> EditCourt([FromForm] CourtEditModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ!" });
            }

            var court = await _context.Courts.FindAsync(model.CourtId);
            if (court == null)
            {
                return NotFound(new { success = false, message = "Sân không tồn tại!" });
            }

            // Cập nhật thông tin sân
            court.CourtName = model.CourtName;
            court.Address = model.Address;
            court.PricePerHour = model.PricePerHour;
            court.StartTime = model.StartTime;
            court.EndTime = model.EndTime;
            court.CategoryId = model.CategoryId;

            // Xử lý hình ảnh nếu có upload mới
            if (model.ImageFile != null)
            {
                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "assets/explore");

                // Kiểm tra nếu thư mục chưa tồn tại thì tạo
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo tên file duy nhất
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(model.ImageFile.FileName)}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Lưu file vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(stream);
                }

                // Cập nhật đường dẫn ảnh
                court.ImageUrl = $"/assets/explore/{fileName}";
            }


            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật sân thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống, thử lại sau!" });
            }
        }


        [HttpGet("subcourts/{courtId}")]
        public async Task<ActionResult<IEnumerable<SubCourtDTO>>> GetSubCourtsByCourtId(int courtId)
        {
            if (courtId <= 0)
            {
                return BadRequest("Court ID không hợp lệ.");
            }

            var subCourts = await _context.SubCourts
                .Where(sc => sc.CourtId == courtId)
                .Select(sc => new SubCourtDTO
                {
                    SubCourtId = sc.SubCourtId,
                    SubCourtName = sc.SubCourtName,
                    Desccription = sc.Description
                })
                .ToListAsync();

            return Ok(subCourts);
        }

        [HttpPost("UpdateSubCourt")]
        public async Task<IActionResult> UpdateSubCourt([FromBody] SubCourt model)
        {
            if (model == null || model.SubCourtId <= 0)
            {
                return BadRequest(new { success = false, message = "Dữ liệu không hợp lệ." });
            }

            var subCourt = await _context.SubCourts.FindAsync(model.SubCourtId);
            if (subCourt == null)
            {
                return NotFound(new { success = false, message = "Không tìm thấy sân phụ." });
            }

            subCourt.SubCourtName = model.SubCourtName;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { success = true, message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi khi cập nhật sân phụ." });
            }
        }


        [HttpPost("addnewcourt")]
        public async Task<IActionResult> AddNewCourt([FromForm] CourtCreateModel model)
        {
            if (model == null)
                return BadRequest("Dữ liệu không hợp lệ!");

            if (model.StartDate >= model.EndDate)
                return BadRequest("Ngày bắt đầu phải nhỏ hơn ngày kết thúc!");

            var category = await _context.Categories.FindAsync(model.CategoryId);
            if (category == null)
                return BadRequest("Category không tồn tại!");

            var court = new Court
            {
                CourtName = model.Name,
                Address = model.Location,
                PricePerHour = model.PricePerHour,
                StartTime = model.StartDate,
                EndTime = model.EndDate,
                CategoryId = model.CategoryId,
                ImageUrl = null // Sẽ cập nhật nếu có ảnh
            };

            if (model.Image != null && model.Image.Length > 0)
            {
                var fileName = $"{Guid.NewGuid()}_{model.Image.FileName}";
                var filePath = Path.Combine("wwwroot/assets/images", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }

                court.ImageUrl = $"/assets/images/{fileName}";
            }

            _context.Courts.Add(court);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(AddNewCourt), new { id = court.CourtId }, court);
        }

    }
}
