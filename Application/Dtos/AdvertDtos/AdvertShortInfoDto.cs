namespace Application.Dtos.AdvertDtos;

public class AdvertShortInfoDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public decimal Price { get; set; }
    public string? MainImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CityName { get; set; }
    public string CategoryName { get; set; }
}