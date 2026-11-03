using AISmokingDetection.DTOs.Incident;


namespace AISmokingDetection.Business.Interfaces
{
    public interface IIncidentService
    {
        Task<IncidentResponseDto> CreateIncidentAsync(CreateIncidentDto dto);

        Task<List<IncidentResponseDto>> GetAllIncidentsAsync();

        Task<IncidentResponseDto?> GetIncidentByIdAsync(int id);
    }
}