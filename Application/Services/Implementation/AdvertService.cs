using Application.Dtos.AdvertDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces.Repository;
using Domain.Models;
using Domain.Primitives;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Services.Implementation
{
    public class AdvertService : IAdvertService
    {
        private readonly IAdvertRepository _advertRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IMapper _mapper;

        public AdvertService(
            IAdvertRepository advertRepository,
            IAttachmentService attachmentService,
            IMapper mapper)
        {
            _advertRepository = advertRepository;
            _attachmentService = attachmentService;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAdvertAsync(CreateAdvertDto dto, Guid userId, CancellationToken cancellationToken = default)
        {
            var advert = _mapper.Map<Advert>(dto);
            advert.UserId = userId;
            advert.CreatedAt = DateTime.UtcNow;

            if (dto.Images != null && dto.Images.Any())
            {
                advert.Attachments = dto.Images.Select(image => new Attachment
                {
                    Id = Guid.NewGuid(),
                    FileName = image.FileName,
                    FilePath = string.Empty, // Временное значение
                    UploadDate = DateTime.UtcNow
                }).ToList();
            }

            await _advertRepository.AddAsync(advert, cancellationToken);
            await _advertRepository.SaveChangesAsync(cancellationToken);

            if (dto.Images != null && dto.Images.Any())
            {
                foreach (var (image, attachment) in dto.Images.Zip(advert.Attachments, (i, a) => (i, a)))
                {
                    var uploaded = await _attachmentService.UploadAttachmentAsync(
                        image,
                        advert.Id,
                        cancellationToken);
            
                    attachment.FileName = uploaded.FileName;
                    attachment.FilePath = uploaded.FilePath;
                }

                await _advertRepository.UpdateAsync(advert, cancellationToken);
                await _advertRepository.SaveChangesAsync(cancellationToken);
            }

            return advert.Id;
        }
        public async Task<List<AdvertShortInfoDto>> GetByFilterLatestAdvertsAsync(
            int count, 
            Guid? categoryId = null, 
            Guid? cityId = null,
            CancellationToken cancellationToken = default)
        {
            var adverts = await _advertRepository.GetLatestAsync(count, categoryId, cityId);
            return _mapper.Map<List<AdvertShortInfoDto>>(adverts);
        }

        public async Task UpdateAdvertAsync(
            UpdateAdvertDto dto, 
            Guid userId, 
            CancellationToken cancellationToken = default)
        {
            var advert = await _advertRepository.GetByIdAsync(
                dto.Id)
                ?? throw new KeyNotFoundException("Advert not found");

            if (advert.UserId != userId)
                throw new UnauthorizedAccessException("You can only update your own adverts");

            // Обновляем основные поля
            advert.Title = dto.Title;
            advert.Description = dto.Description;
            advert.Price = dto.Price;
            advert.ContactPhone = dto.ContactPhone;
            advert.CityId = dto.CityId;
            advert.CategoryId = dto.CategoryId;

            // Удаление указанных изображений
            if (dto.DeletedImageIds != null && dto.DeletedImageIds.Any())
            {
                foreach (var imageId in dto.DeletedImageIds)
                {
                    var attachment = advert.Attachments.FirstOrDefault(a => a.Id == imageId);
                    if (attachment != null)
                    {
                        await _attachmentService.DeleteAttachmentAsync(imageId, cancellationToken);
                        advert.Attachments.Remove(attachment);
                    }
                }
            }

            // Добавление новых изображений
            if (dto.NewImages != null && dto.NewImages.Any())
            {
                foreach (var image in dto.NewImages)
                {
                    var attachment = await _attachmentService.UploadAttachmentAsync(
                        image, 
                        advert.Id, 
                        cancellationToken);
                    
                    advert.Attachments.Add(new Attachment
                    {
                        FileName = attachment.FileName,
                        FilePath = attachment.FilePath,
                        AdvertId = advert.Id,
                        UploadDate = DateTime.UtcNow
                    });
                }
            }

            await _advertRepository.UpdateAsync(advert, cancellationToken);
            await _advertRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<Advert> GetById(Guid id)
        {
            return  await _advertRepository.GetByIdAsync(id);
            
        }

        public async Task<Advert> GetWithDetailsAsync(Guid
            advertId)
        {
            return await _advertRepository.GetWithDetailsAsync(
                             advertId)
                         ?? throw new KeyNotFoundException("Advert not found");
        }

        public async Task DeleteAdvertAsync(
            Guid advertId, 
            Guid userId, 
            CancellationToken cancellationToken = default)
        {
            var advert = await _advertRepository.GetWithDetailsAsync(
                advertId)
                ?? throw new KeyNotFoundException("Advert not found");

            if (advert.UserId != userId)
                throw new UnauthorizedAccessException("You can only delete your own adverts");

            // Удаляем все вложения
            foreach (var attachment in advert.Attachments.ToList())
            {
                await _attachmentService.DeleteAttachmentAsync(attachment.Id, cancellationToken);
            }

            await _advertRepository.DeleteAsync(advert, cancellationToken);
            await _advertRepository.SaveChangesAsync(cancellationToken);
        }

        public async Task<AdvertDetailsDto> GetAdvertDetailsAsync(
            Guid advertId, 
            CancellationToken cancellationToken = default)
        {
            var advert = await _advertRepository.GetWithDetailsAsync(
                advertId)
                ?? throw new KeyNotFoundException("Advert not found");

            return _mapper.Map<AdvertDetailsDto>(advert);
        }

        public async Task<List<AdvertShortInfoDto>> GetLatestAdvertsAsync(
            int count, 
            CancellationToken cancellationToken = default)
        {
            var adverts = await _advertRepository.GetLatestAsync(count);
            return _mapper.Map<List<AdvertShortInfoDto>>(adverts);
        }

        public async Task<List<AdvertShortInfoDto>> GetAdvertsByCategoryAsync(
            Guid categoryId, 
            CancellationToken cancellationToken = default)
        {
            var adverts = await _advertRepository.GetByCategoryAsync(categoryId);
            return _mapper.Map<List<AdvertShortInfoDto>>(adverts);
        }

        public async Task<List<AdvertShortInfoDto>> GetAdvertsByCityAsync(
            Guid cityId, 
            CancellationToken cancellationToken = default)
        {
            var adverts = await _advertRepository.GetByCityAsync(cityId);
            return _mapper.Map<List<AdvertShortInfoDto>>(adverts);
        }

        public async Task<List<AdvertShortInfoDto>> GetUserAdvertsAsync(
            Guid userId, 
            CancellationToken cancellationToken = default)
        {
            var adverts = await _advertRepository.GetByUserIdAsync(userId);
            return _mapper.Map<List<AdvertShortInfoDto>>(adverts);
        }

        public async Task ToggleAdvertStatusAsync(
            Guid advertId, 
            Guid userId, 
            AdvertStatus newStatus,
            CancellationToken cancellationToken = default)
        {
            var advert = await _advertRepository.GetByIdAsync(advertId, cancellationToken)
                ?? throw new KeyNotFoundException("Advert not found");

            if (advert.UserId != userId)
                throw new UnauthorizedAccessException("You can only modify your own adverts");

            await _advertRepository.UpdateAsync(advert, cancellationToken);
            await _advertRepository.SaveChangesAsync(cancellationToken);
        }
    }
}