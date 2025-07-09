using Application.Dtos.AdvertDtos;
using FluentValidation;

namespace Application.Validation;

public class CreateAdvertDtoValidator : AbstractValidator<CreateAdvertDto>
{
    public CreateAdvertDtoValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).NotEmpty().MaximumLength(1000);
        RuleFor(x => x.Price).GreaterThan(0);
        RuleFor(x => x.ContactPhone).NotEmpty().Matches(@"^\+?[0-9\s\-\(\)]{10,}$");
        RuleFor(x => x.CityId).NotEmpty();
        RuleFor(x => x.CategoryId).NotEmpty();
        RuleForEach(x => x.Images)
            .Must(image => image.Length < 5 * 1024 * 1024) // 5MB
            .WithMessage("Image size must be less than 5MB");
    }
}