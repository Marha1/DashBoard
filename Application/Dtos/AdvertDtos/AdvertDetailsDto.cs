namespace Application.Dtos.AdvertDtos;

public sealed class AdvertDetailsDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public string ContactPhone { get; set; }
    public string CityName { get; set; }
    public string CategoryName { get; set; }
    public List<string> ImageUrls { get; set; }
    public DateTime CreatedAt { get; set; }
}