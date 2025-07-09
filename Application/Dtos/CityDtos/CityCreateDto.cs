namespace Application.Dtos.CityDtos
{
    public sealed record CityCreateDto(string Name);
    public sealed  record CityUpdateDto(Guid Id, string Name, bool IsActive);
    public sealed record CityDto(Guid Id, string Name, bool IsActive);
}