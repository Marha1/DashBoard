using Microsoft.AspNetCore.Http;

namespace Application.Dtos.AdvertDtos;

public sealed record CreateAdvertDto(
    string Title,
    string Description,
    decimal Price,
    string ContactPhone,
    Guid CityId,
    Guid CategoryId,
    List<IFormFile>? Images);