using Application.Dtos.AdvertDtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAdvertService _advertService;
        private readonly ICategoryService _categoryService;
        private readonly ICityService _cityService;

        public HomeController(
            IAdvertService advertService,
            ICategoryService categoryService,
            ICityService cityService)
        {
            _advertService = advertService;
            _categoryService = categoryService;
            _cityService = cityService;
        }

        public async Task<IActionResult> Index(Guid? categoryId, Guid? cityId)
        {
            var adverts = await _advertService.GetByFilterLatestAdvertsAsync(100, categoryId, cityId);
            var categories = await _categoryService.GetAllCategoriesAsync();
            var cities = await _cityService.GetAllCitiesAsync();

            ViewBag.Categories = categories;
            ViewBag.Cities = cities;
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.SelectedCityId = cityId;

            return View(adverts);
        }

        public async Task<IActionResult> Details(Guid id)
        {
            var advert = await _advertService.GetAdvertDetailsAsync(id);
            if (advert == null)
            {
                return NotFound();
            }
            return View(advert);
        }
    }
}