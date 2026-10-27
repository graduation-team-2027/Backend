
using AISmokingDetection.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CameraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CameraController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveCameras()
        {
            var cameras = await _context.Cameras
                .AsNoTracking()
                .Where(c => c.IsActive && c.Location.IsActive)
                .Select(c => new
                {
                    c.CameraId,
                    c.Name,
                    c.CameraCode,
                    c.LocationId,
                    LocationName = c.Location.Name,
                    c.IsActive
                })
                .ToListAsync();

            return Ok(cameras);
        }
    }
}
