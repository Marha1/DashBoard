using Application.Dtos.CityDtos;
using Domain.Primitives;
using System.Threading;

namespace Application.Services.Interfaces
{
    public interface ICityService
    {
        Task<Guid> CreateCityAsync(CityCreateDto dto, CancellationToken cancellationToken = default);
        Task UpdateCityAsync(CityUpdateDto dto, CancellationToken cancellationToken = default);
        Task DeleteCityAsync(Guid cityId, CancellationToken cancellationToken = default);
        Task<List<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default);
        Task<List<CityDto>> GetActiveCitiesAsync();
        Task ToggleCityStatusAsync(Guid cityId, CancellationToken cancellationToken = default);
        Task<CityDto> GetCityByIdAsync(Guid id, CancellationToken cancellationToken = default);
    }
}