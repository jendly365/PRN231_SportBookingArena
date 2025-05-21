using BusinessObject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportBookingWebAPI.Dtos.Court;

namespace SportBookingWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CategoryController : ControllerBase
    {

		private readonly EXE201_Rental_Sport_Field1Context _context;

		public CategoryController(EXE201_Rental_Sport_Field1Context context)
		{
			_context = context;
		}
		[HttpGet]
		public async Task<ActionResult<IEnumerable<Category>>> Get()
		{
			var courts = await _context.Categories
				.ToListAsync();
			return Ok(courts);
		}
	}
}
