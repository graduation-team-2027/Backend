using AISmokingDetection.Business.Interfaces;
using AISmokingDetection.DTOs.ProcessingJob;
using AISmokingDetection.Data;
using AISmokingDetection.Model;
using Microsoft.EntityFrameworkCore;

namespace AISmokingDetection.API.Business.Services
{
    public class ProcessingJobService : IProcessingJobService
    {
        private readonly ApplicationDbContext _context;

        public ProcessingJobService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ProcessingJobResponseDto> CreateJobAsync(
            CreateProcessingJobDto dto)
        {
            if (dto.IncidentId.HasValue)
            {
                bool incidentExists = await _context.Incidents
                    .AnyAsync(i => i.IncidentId == dto.IncidentId.Value);

                if (!incidentExists)
                    throw new KeyNotFoundException("Incident not found.");
            }

            var job = new ProcessingJob
            {
                IncidentId = dto.IncidentId,
                Status = ProcessingJobStatus.Queued,
                ModelVersion = dto.ModelVersion,
                DataOrigin = dto.DataOrigin
            };

            _context.ProcessingJobs.Add(job);

            await _context.SaveChangesAsync();

            return MapToResponseDto(job);
        }

        public async Task<ProcessingJobResponseDto?> GetJobByIdAsync(int id)
        {
            var job = await _context.ProcessingJobs
                .FirstOrDefaultAsync(j => j.ProcessingJobId == id);

            if (job == null)
                return null;

            return MapToResponseDto(job);
        }

        public async Task MarkAsProcessingAsync(int id)
        {
            var job = await GetJobOrThrowAsync(id);

            if (job.Status != ProcessingJobStatus.Queued)
                throw new InvalidOperationException(
                    "Only queued jobs can start processing.");

            job.Status = ProcessingJobStatus.Processing;
            job.StartedAtUtc = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task CompleteJobAsync(int id)
        {
            var job = await GetJobOrThrowAsync(id);

            if (job.Status != ProcessingJobStatus.Processing)
                throw new InvalidOperationException(
                    "Only processing jobs can be completed.");

            job.Status = ProcessingJobStatus.Completed;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
        }

        public async Task FailJobAsync(int id, string errorCode)
        {
            var job = await GetJobOrThrowAsync(id);

            if (job.Status != ProcessingJobStatus.Processing)
                throw new InvalidOperationException(
                    "Only processing jobs can fail.");

            if (string.IsNullOrWhiteSpace(errorCode))
                throw new ArgumentException("Error code is required.");

            job.Status = ProcessingJobStatus.Failed;
            job.ErrorCode = errorCode;
            job.CompletedAtUtc = DateTimeOffset.UtcNow;

            await _context.SaveChangesAsync();
        }

        private async Task<ProcessingJob> GetJobOrThrowAsync(int id)
        {
            var job = await _context.ProcessingJobs
                .FirstOrDefaultAsync(j => j.ProcessingJobId == id);

            if (job == null)
                throw new KeyNotFoundException("Processing job not found.");

            return job;
        }

        private static ProcessingJobResponseDto MapToResponseDto(
            ProcessingJob job)
        {
            return new ProcessingJobResponseDto
            {
                ProcessingJobId = job.ProcessingJobId,
                IncidentId = job.IncidentId,
                Status = job.Status,
                ModelVersion = job.ModelVersion,
                DataOrigin = job.DataOrigin,
                StartedAtUtc = job.StartedAtUtc,
                CompletedAtUtc = job.CompletedAtUtc,
                ErrorCode = job.ErrorCode
            };
        }
    }
}