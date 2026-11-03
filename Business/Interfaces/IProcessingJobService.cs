
using AISmokingDetection.DTOs.ProcessingJob;

namespace AISmokingDetection.Business.Interfaces
{
    public interface IProcessingJobService
    {
        Task<ProcessingJobResponseDto> CreateJobAsync(CreateProcessingJobDto dto);

        Task<ProcessingJobResponseDto?> GetJobByIdAsync(int id);

        Task MarkAsProcessingAsync(int id);

        Task CompleteJobAsync(int id);

        Task FailJobAsync(int id, string errorCode);
    }
}