using Application.Dtos.AccountDtos;
using Application.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Presentation.ViewModels.Account;

namespace Presentation.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IEmailSender _emailSender;

        public AccountController(
            IAccountService accountService, 
            IEmailSender emailSender)
        {
            _accountService = accountService;
            _emailSender = emailSender;
        }

        // Регистрация
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var dto = new RegisterDto(
                model.UserName,
                model.Email,
                model.Password,
                model.FirstName,
                model.Surname,
                model.LastName);

            try
            {
                await _accountService.RegisterUserAsync(dto);
                await _accountService.SendEmailConfirmedCode(model.Email);
                return RedirectToAction("ConfirmEmail", new { email = model.Email });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // Подтверждение email
        [HttpGet]
        public IActionResult ConfirmEmail(string email)
        {
            var model = new ConfirmEmailViewModel { Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                await _accountService.EmailConfirmed(model.Email, model.ConfirmCode);
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        // Вход
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var result = await _accountService.LoginAsync(new LoginDto(model.Email, model.Password));
        
                Response.Cookies.Append("AuthToken", result.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false, // Для локальной разработки
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTimeOffset.Now.AddDays(30),
                    Path = "/"
                });

                // Проверка ролей из сервиса
                if (result.Roles.Contains("Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("Index", "Home");
        }

        // Восстановление пароля
        [HttpGet]
        public IActionResult ForgotPassword() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _emailSender.SendResetPasswordEmailAsync(model.Email, Guid.NewGuid().ToString());
            return RedirectToAction("Login");
        }
    }
}