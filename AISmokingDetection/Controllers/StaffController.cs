
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StaffController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllStaff()
        {
            var staff = await _context.Staff
                .Select(s => new
                {
                    s.StaffId,
                    s.FullName,
                    s.Email,
                    s.Role,
                    s.IsActive
                })
                .ToListAsync();

            return Ok(staff);
        }

        [HttpPost]
        public async Task<IActionResult> CreateStaff(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Staff created successfully",
                staff.StaffId,
                staff.FullName,
                staff.Email,
                staff.Role,
                staff.IsActive
            });
        }

    }
}
