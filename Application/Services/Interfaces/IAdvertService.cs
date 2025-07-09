using Application.Dtos.AdvertDtos;
using Domain.Primitives;
using System.Threading;
using Domain.Models;

namespace Application.Services.Interfaces
{
    public interface IAdvertService
    {
        Task<Guid> CreateAdvertAsync(
            CreateAdvertDto dto, 
            Guid userId, 
            CancellationToken cancellationToken = default);

        Task UpdateAdvertAsync(
            UpdateAdvertDto dto, 
            Guid userId, 
            CancellationToken cancellationToken = default);

        Task<Advert> GetById(
            Guid id);

        Task DeleteAdvertAsync(
            Guid advertId, 
            Guid userId, 
            CancellationToken cancellationToken = default);

        Task<AdvertDetailsDto> GetAdvertDetailsAsync(
            Guid advertId, 
            CancellationToken cancellationToken = default);

        Task<List<AdvertShortInfoDto>> GetLatestAdvertsAsync(
            int count, 
            CancellationToken cancellationToken = default);

        Task<List<AdvertShortInfoDto>> GetAdvertsByCategoryAsync(
            Guid categoryId, 
            CancellationToken cancellationToken = default);

        Task<List<AdvertShortInfoDto>> GetAdvertsByCityAsync(
            Guid cityId, 
            CancellationToken cancellationToken = default);

        Task<List<AdvertShortInfoDto>> GetUserAdvertsAsync(
            Guid userId, 
            CancellationToken cancellationToken = default);

        Task ToggleAdvertStatusAsync(
            Guid advertId, 
            Guid userId, 
            AdvertStatus newStatus,
            CancellationToken cancellationToken = default);
    }
}