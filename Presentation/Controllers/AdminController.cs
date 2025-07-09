using System.Security.Claims;
using Application.Dtos.AdvertDtos;
using Application.Dtos.CategoryDtos;
using Application.Dtos.CityDtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Presentation.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICityService _cityService;
        private readonly ICategoryService _categoryService;
        private readonly IAdvertService _advertService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(
            ICityService cityService,
            ICategoryService categoryService,
            IAdvertService advertService,
            ILogger<AdminController> logger)
        {
            _cityService = cityService;
            _categoryService = categoryService;
            _advertService = advertService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("Admin panel accessed");
            return View();
        }

        #region Управление городами

        public async Task<IActionResult> Cities()
        {
            var cities = await _cityService.GetAllCitiesAsync();
            return View(cities);
        }

        [HttpGet]
        public IActionResult CreateCity() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCity(CityCreateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _cityService.CreateCityAsync(model);
            return RedirectToAction("Cities");
        }

        [HttpGet]
        public async Task<IActionResult> EditCity(Guid id)
        {
            var city = await _cityService.GetCityByIdAsync(id);
            return View(new CityUpdateDto(
                Id: city.Id,
                Name: city.Name,
                IsActive: city.IsActive
            ));
        }

        [HttpPost]
        public async Task<IActionResult> EditCity(CityUpdateDto model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _cityService.UpdateCityAsync(model);
            return RedirectToAction("Cities");
        }

        #endregion

        #region Управление категориями

        public async Task<IActionResult> Categories()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return View(categories);
        }

        [HttpGet]
        public IActionResult CreateCategory()
        {
            ViewBag.Categories = _categoryService.GetAllCategoriesAsync().Result;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreateDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            await _categoryService.CreateCategoryAsync(model);
            return RedirectToAction("Categories");
        }

        [HttpGet]
        public async Task<IActionResult> EditCategory(Guid id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();

            return View(new CategoryUpdateDto(
                Id: category.Id,
                Name: category.Name,
                Image: null,
                ParentId: category.ParentId
            ));
        }

        [HttpPost]
        public async Task<IActionResult> EditCategory(CategoryUpdateDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            await _categoryService.UpdateCategoryAsync(model);
            return RedirectToAction("Categories");
        }

        #endregion

        #region Управление объявлениями

        [HttpGet]
        public async Task<IActionResult> Adverts()
        {
            var adverts = await _advertService.GetLatestAdvertsAsync(100);
            return View(adverts);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAdvert()
        {
            ViewBag.Cities = new SelectList(await _cityService.GetAllCitiesAsync(), "Id", "Name");
            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdvert(CreateAdvertDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = new SelectList(await _cityService.GetAllCitiesAsync(), "Id", "Name");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name");
                return View(model);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authorized");            
            await _advertService.CreateAdvertAsync(model, Guid.Parse(userId));
            return RedirectToAction("Adverts");
        }

        [HttpGet]
        public async Task<IActionResult> EditAdvert(Guid id)
        {
            var advert = await _advertService.GetById(id);
            if (advert == null)
            {
                return NotFound();
            }

            // Получаем данные для выпадающих списков
            var cities = (await _cityService.GetAllCitiesAsync())?.ToList() ?? new List<CityDto>();
            var categories = (await _categoryService.GetAllCategoriesAsync())?.ToList() ?? new List<CategoryDto>();

            var model = new UpdateAdvertDto
            {
                Id = advert.Id,
                Title = advert.Title,
                Description = advert.Description,
                Price = advert.Price,
                ContactPhone = advert.ContactPhone,
                CityId = advert.CityId,
                CategoryId = advert.Category?.Id ?? Guid.Empty
            };

            // Создаем SelectList с явной проверкой
            ViewBag.Cities = cities.Any() 
                ? new SelectList(cities, "Id", "Name", model.CityId) 
                : new SelectList(new List<CityDto>(), "Id", "Name");
    
            ViewBag.Categories = categories.Any()
                ? new SelectList(categories, "Id", "Name", model.CategoryId)
                : new SelectList(new List<CategoryDto>(), "Id", "Name");

            ViewBag.ExistingImages = advert.Attachments?
                .Select(a => new { Id = a.Id, FilePath = a.FilePath })
                .ToList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdvert(UpdateAdvertDto model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cities = new SelectList(await _cityService.GetAllCitiesAsync(), "Id", "Name", model.CityId);
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Name", model.CategoryId);
                return View(model);
            }
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authorized");    
            await _advertService.UpdateAdvertAsync(model, Guid.Parse(userId)); 
            return RedirectToAction("Adverts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAdvert(Guid id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new UnauthorizedAccessException();
            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not authorized");    
            await _advertService.DeleteAdvertAsync(id, Guid.Parse(userId));
            return RedirectToAction("Adverts");
        }

        #endregion
    }
}