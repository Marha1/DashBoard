using Microsoft.AspNetCore.Http;

namespace Application.Dtos.AdvertDtos
{
    public class UpdateAdvertDto  // Изменим record на class для простоты
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ContactPhone { get; set; }
        public Guid CityId { get; set; }  // Просто Guid вместо сложных связей
        public Guid CategoryId { get; set; }
        public List<IFormFile>? NewImages { get; set; }
        public List<Guid>? DeletedImageIds { get; set; }
    }
}