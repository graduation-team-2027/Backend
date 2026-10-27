
using AISmokingDetection.Data;
using AISmokingDetection.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ParticipantController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ParticipantController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetActiveParticipants()
        {
            var participants = await _context.Participants
                .AsNoTracking()
                .Where(p => p.IsActive)
                .Select(p => new
                {
                    p.ParticipantId,
                    p.FullName,
                    p.ParticipantCode,
                    p.IsActive
                })
                .ToListAsync();

            return Ok(participants);
        }

        [HttpPost]
        public async Task<IActionResult> EnrollParticipant(
            [FromBody] EnrollParticipantRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FullName) ||
                string.IsNullOrWhiteSpace(request.ParticipantCode))
            {
                return BadRequest(new
                {
                    message = "FullName and ParticipantCode are required."
                });
            }

            var fullName = request.FullName.Trim();
            var participantCode = request.ParticipantCode.Trim();

            if (fullName.Length > 150 || participantCode.Length > 50)
            {
                return BadRequest(new
                {
                    message = "FullName must not exceed 150 characters and ParticipantCode must not exceed 50 characters."
                });
            }

            var codeExists = await _context.Participants
                .AnyAsync(p => p.ParticipantCode == participantCode);

            if (codeExists)
            {
                return Conflict(new
                {
                    message = "ParticipantCode already exists."
                });
            }

            var participant = new Participant
            {
                FullName = fullName,
                ParticipantCode = participantCode,
                IsActive = true
            };

            _context.Participants.Add(participant);
            await _context.SaveChangesAsync();

            return StatusCode(
                StatusCodes.Status201Created,
                new
                {
                    participant.ParticipantId,
                    participant.FullName,
                    participant.ParticipantCode,
                    participant.IsActive
                });
        }
    }

    public class EnrollParticipantRequest
    {
        public string FullName { get; set; } = string.Empty;

        public string ParticipantCode { get; set; } = string.Empty;
    }
}
