namespace Application.Dtos.AdvertDtos;

public sealed record AdvertDetailsDto(
    Guid Id,
    string Title,
    string Description,
    decimal Price,
    string ContactPhone,
    string CityName,
    string CategoryName,
    List<string> ImageUrls,
    DateTime CreatedAt);