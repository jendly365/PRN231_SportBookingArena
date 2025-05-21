using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportBookingWebAPI.Dtos;

namespace SportBookingWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly EXE201_Rental_Sport_Field1Context _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BookingController(EXE201_Rental_Sport_Field1Context context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }
        [HttpGet("{userId}")]
        public IActionResult GetBookingsByUserId(int userId)
        {
            // Truy vấn dữ liệu từ DB và map vào ViewModel
            var bookings = _context.Bookings
                .Where(b => b.UserId == userId)
                .Include(b => b.SubCourt)
                .ThenInclude(sc => sc.Court) // Đảm bảo lấy CourtName và Address từ Court
                .Include(b => b.User) // Lấy FullName của User
                .Select(b => new BookingViewModel
                {
                    BookingId = b.BookingId,
                    CourtName = b.SubCourt.Court.CourtName,
                    SubCourtName = b.SubCourt.SubCourtName,
                    Address = b.SubCourt.Court.Address,
                    PricePerHour = b.SubCourt.Court.PricePerHour,
                    FullName = b.SubCourt.Court.User.FullName,  // Lấy FullName của chủ Court
                    Status = b.Status,
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    Description = b.Description,
                    CreateDate = b.CreatedAt

                })
                .ToList();

            return Ok(bookings);
        }


        [HttpPost("cancel-booking")]
        public IActionResult CancelBooking([FromBody] BookingRequest request)
        {
            var bookingId = request.BookingId;
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return BadRequest("Không tìm thấy booking với ID: " + bookingId);
            }

            if (booking.Status == null)
            {
                return BadRequest("Trạng thái của booking không hợp lệ.");
            }

            // Lấy thời gian hiện tại và thời gian bắt đầu của booking
            var now = DateTime.UtcNow;
            var startDate = booking.StartTime; // Giả sử bạn có trường này trong bảng Bookings

            // Tính thời gian còn lại để hủy
            var timeDifference = startDate - now;

            // Kiểm tra nếu timeDifference có giá trị
            if (timeDifference.HasValue)
            {
                string refundMessage = string.Empty;
                var totalHours = timeDifference.Value.TotalHours;

                // Kiểm tra các điều kiện hủy và thông báo hoàn tiền
                if (totalHours >= 12)
                {
                    refundMessage = "Bạn đã được hoàn 100% tiền.";
                }
                else if (totalHours >= 6)
                {
                    refundMessage = "Bạn đã được hoàn 50% tiền.";
                }
                else if (totalHours >= 2)
                {
                    refundMessage = "Không hoàn tiền.";
                }

                // Kiểm tra nếu booking chưa hủy và cập nhật trạng thái
                if (booking.Status != "Cancelled")
                {
                    booking.Status = "Cancelled";
                    _context.SaveChanges(); // Lưu thay đổi vào database

                    return Ok(new { message = "Hủy booking thành công", refundMessage });
                }
                return BadRequest("Booking đã được hủy rồi");
            }
            else
            {
                return BadRequest("Không thể tính toán thời gian hủy.");
            }
        }

        [HttpPost("confirmed")]
        public IActionResult ConfirmBooking([FromBody] BookingRequest request)
        {
            var bookingId = request.BookingId;
            var booking = _context.Bookings.FirstOrDefault(b => b.BookingId == bookingId);

            if (booking == null)
            {
                return BadRequest("Không tìm thấy booking với ID: " + bookingId);
            }

            if (booking.Status == null)
            {
                return BadRequest("Trạng thái của booking không hợp lệ.");
            }

            // Lấy thời gian hiện tại và thời gian bắt đầu của booking
            var now = DateTime.UtcNow;
            var startDate = booking.StartTime; // Giả sử bạn có trường này trong bảng Bookings

            // Tính thời gian còn lại để hủy
            var timeDifference = startDate - now;

            // Kiểm tra nếu timeDifference có giá trị
            if (timeDifference.HasValue)
            {
                string refundMessage = string.Empty;
                var totalHours = timeDifference.Value.TotalHours;

                // Kiểm tra các điều kiện hủy và thông báo hoàn tiền
                if (totalHours >= 12)
                {
                    refundMessage = "Bạn đã được hoàn 100% tiền.";
                }
                else if (totalHours >= 6)
                {
                    refundMessage = "Bạn đã được hoàn 50% tiền.";
                }
                else if (totalHours >= 2)
                {
                    refundMessage = "Không hoàn tiền.";
                }

                // Kiểm tra nếu booking chưa hủy và cập nhật trạng thái
                if (booking.Status != "Confirmed")
                {
                    booking.Status = "Confirmed";
                    _context.SaveChanges(); // Lưu thay đổi vào database

                    return Ok(new { message = "Hủy booking thành công", refundMessage });
                }
                return BadRequest("Booking đã được hủy rồi");
            }
            else
            {
                return BadRequest("Không thể tính toán thời gian hủy.");
            }
        }

        public class BookingRequest
        {
            public int BookingId { get; set; }
        }

        [HttpGet("owner/{ownerUserId}")]
        public async Task<ActionResult<IEnumerable<BookingDetails>>> GetBookingsByCourtOwner(int ownerUserId)
        {
            try
            {
                var bookings = await (from booking in _context.Bookings
                                      join subCourt in _context.SubCourts
                                      on booking.SubCourtId equals subCourt.SubCourtId
                                      join court in _context.Courts
                                      on subCourt.CourtId equals court.CourtId
                                      join user in _context.Users
                                      on booking.UserId equals user.UserId
                                      where court.UserId == ownerUserId
                                      select new BookingDetails
                                      {
                                          BookingID = booking.BookingId,
                                          CustomerID = user.UserId,
                                          CustomerName = user.FullName,
                                          CustomerPhone = user.PhoneNumber,
                                          MainCourtName = court.CourtName,
                                          SubCourtName = subCourt.SubCourtName,
                                          StartTime = booking.StartTime,
                                          EndTime = booking.EndTime,
                                          TotalHours = (booking.StartTime.HasValue && booking.EndTime.HasValue)
                                                      ? (int)(EF.Functions.DateDiffHour(booking.StartTime.Value, booking.EndTime.Value))
                                                      : 0,  // Nếu StartTime hoặc EndTime là null, TotalHours sẽ là 0
                                          BookingStatus = booking.Status,
                                          PricePerHour = subCourt.PricePerHour ?? 0, // Nếu PricePerHour là null, giá trị mặc định là 0
                                          TotalAmount = (booking.StartTime.HasValue && booking.EndTime.HasValue && subCourt.PricePerHour.HasValue)
                                                      ? (int)(EF.Functions.DateDiffHour(booking.StartTime.Value, booking.EndTime.Value) * subCourt.PricePerHour.Value)
                                                      : 0,  // Nếu StartTime, EndTime, hoặc PricePerHour là null, TotalAmount sẽ là 0
                                          BookingDate = booking.CreatedAt
                                      })
                                      .ToListAsync();

                if (bookings == null || !bookings.Any())
                {
                    return NotFound(new { message = "Không tìm thấy booking nào cho chủ sân này." });
                }

                return Ok(bookings);
            }
            catch (Exception ex)
            {
                // Log chi tiết về lỗi
                var errorMessage = $"Error: {ex.Message}, StackTrace: {ex.StackTrace}";
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi lấy danh sách booking.", error = errorMessage });
            }
        }












    }
}
