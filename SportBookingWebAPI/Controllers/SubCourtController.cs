using BusinessObject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportBookingWebAPI.Dtos.Court;

namespace SportBookingWebAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubCourtController : ControllerBase
	{
		private readonly EXE201_Rental_Sport_Field1Context _context;

		public SubCourtController(EXE201_Rental_Sport_Field1Context context)
		{
			_context = context;
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<IEnumerable<SubCourt>>> GetSubCourtsByCourtId(int id)
		{
			if (id == 0)
			{
				return BadRequest("Invalid Court ID");
			}

			var subCourts = await _context.SubCourts
				.Where(s => s.CourtId == id)
				.ToListAsync();

			if (subCourts == null || !subCourts.Any())
			{
				return NotFound("No subcourts found for this court.");
			}

			return Ok(subCourts);
		}

	}
}
