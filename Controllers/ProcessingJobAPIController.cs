using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.ProcessingJob;
using Microsoft.AspNetCore.Mvc;

namespace AISmokingDetection.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessingJobController : ControllerBase
    {
        private readonly IProcessingJobService _processingJobService;

        public ProcessingJobController(
            IProcessingJobService processingJobService)
        {
            _processingJobService = processingJobService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(CreateProcessingJobDto dto)
        {
            var result = await _processingJobService.CreateJobAsync(dto);

            return CreatedAtAction(
                nameof(GetJobById),
                new { id = result.ProcessingJobId },
                result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetJobById(int id)
        {
            var result = await _processingJobService.GetJobByIdAsync(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPatch("{id:int}/start")]
        public async Task<IActionResult> StartJob(int id)
        {
            await _processingJobService.MarkAsProcessingAsync(id);

            return Ok(new { message = "Job started successfully." });
        }

        [HttpPatch("{id:int}/complete")]
        public async Task<IActionResult> CompleteJob(int id)
        {
            await _processingJobService.CompleteJobAsync(id);

            return Ok(new { message = "Job completed successfully." });
        }

        [HttpPatch("{id:int}/fail")]
        public async Task<IActionResult> FailJob(
            int id,
            [FromBody] string errorCode)
        {
            await _processingJobService.FailJobAsync(id, errorCode);

            return Ok(new { message = "Job failed." });
        }
    }
}