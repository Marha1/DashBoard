namespace Application.Dtos.AdvertDtos;

public sealed record AdvertShortInfoDto(
    Guid Id,
    string Title,
    decimal Price,
    string? MainImageUrl,
    DateTime CreatedAt);
