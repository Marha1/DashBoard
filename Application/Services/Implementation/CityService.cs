using Application.Dtos.CityDtos;
using AutoMapper;
using Domain.Interfaces.Repository;
using Domain.Models;
using Domain.Primitives;
using System.Threading;
using Application.Services.Interfaces;

namespace Application.Services.Implementation
{
    public class CityService : ICityService
    {
        private readonly ICityRepository _cityRepository;
        private readonly IMapper _mapper;

        public CityService(ICityRepository cityRepository, IMapper mapper)
        {
            _cityRepository = cityRepository;
            _mapper = mapper;
        }

        public async Task<Guid> CreateCityAsync(CityCreateDto dto, CancellationToken cancellationToken = default)
        {
            var city = _mapper.Map<City>(dto);
            city.IsActive = true;
            await _cityRepository.AddAsync(city, cancellationToken);
            await _cityRepository.SaveChangesAsync(cancellationToken);
            return city.Id;
        }

        public async Task UpdateCityAsync(CityUpdateDto dto, CancellationToken cancellationToken = default)
        {
            var city = await _cityRepository.GetByIdAsync(dto.Id, cancellationToken)
                ?? throw new KeyNotFoundException("City not found");
            
            city.Name = dto.Name;
            city.IsActive = dto.IsActive;
            
            await _cityRepository.UpdateAsync(city, cancellationToken);
            await _cityRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteCityAsync(Guid cityId, CancellationToken cancellationToken = default)
        {
            var city = await _cityRepository.GetByIdAsync(cityId, cancellationToken);
            if (city != null)
            {
                await _cityRepository.DeleteAsync(city, cancellationToken);
                await _cityRepository.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task<List<CityDto>> GetAllCitiesAsync(CancellationToken cancellationToken = default)
        {
            var cities = await _cityRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<List<CityDto>>(cities);
        }

        public async Task<List<CityDto>> GetActiveCitiesAsync()
        {
            var cities = await _cityRepository.GetActiveCitiesAsync();
            return _mapper.Map<List<CityDto>>(cities);
        }

        public async Task ToggleCityStatusAsync(Guid cityId, CancellationToken cancellationToken = default)
        {
            await _cityRepository.ToggleActivityAsync(cityId);
        }

        public async Task<CityDto> GetCityByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var city = await _cityRepository.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<CityDto>(city);
        }
    }
}