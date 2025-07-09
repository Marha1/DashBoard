using System.Security.Claims;
using Application.Mapping;
using Application.Services.Implementation;
using Application.Services.Interfaces;
using Domain.Interfaces.Repository;
using Domain.Models;
using Infrastructure;
using Infrastructure.Repository.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация
var configuration = builder.Configuration;

// MVC и Razor Pages
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Identity с ролями
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

// Настройка авторизации
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
});

// Настройка аутентификации (исправленная версия)
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    options.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"])),
        RoleClaimType = ClaimTypes.Role
    };
});

// Настройка куки (перенесена в отдельный вызов)
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromDays(30);
    options.SlidingExpiration = true;
    options.Cookie.HttpOnly = true;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
        ? CookieSecurePolicy.SameAsRequest 
        : CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

builder.Services.AddSession();

// База данных
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// Репозитории и сервисы
builder.Services.AddScoped<IAdvertRepository, AdvertRepository>();
builder.Services.AddScoped<IAttachmentRepository, AttachmentRepository>();
builder.Services.AddScoped<ICityRepository, CityRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<IAdvertService, AdvertService>();
builder.Services.AddScoped<IAttachmentService, AttachmentService>();
builder.Services.AddScoped<ICityService, CityService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmailSender, EmailSenderService>();

// AutoMapper
builder.Services.AddAutoMapper(config => 
{
    config.AddProfile<UserMappingProfile>();
    config.AddProfile<AdvertMappingProfile>();
    config.AddProfile<CityMappingProfile>();
    config.AddProfile<CategoryMappingProfile>();
});

var app = builder.Build();

// Middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Инициализация ролей при первом запуске
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
        await InitializeRoles(roleManager);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Middleware для обработки JWT из куки
app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["AuthToken"];
    if (!string.IsNullOrEmpty(token))
    {
        try
        {
            var accountService = context.RequestServices.GetRequiredService<IAccountService>();
            var principal = accountService.GetPrincipalFromToken(token);
            
            // Логирование для отладки
            Console.WriteLine($"Successfully validated token for: {principal.Identity.Name}");
            
            var identity = new ClaimsIdentity(
                principal.Claims,
                IdentityConstants.ApplicationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role);
            
            await context.SignInAsync(IdentityConstants.ApplicationScheme, new ClaimsPrincipal(identity));
        }
        catch (SecurityTokenException ex)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogWarning(ex, "Invalid JWT token - removing cookie");
            context.Response.Cookies.Delete("AuthToken");
        }
    }
    await next();
});

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{action=Index}/{id?}",
    defaults: new { controller = "Admin" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();

async Task InitializeRoles(RoleManager<IdentityRole<Guid>> roleManager)
{
    string[] roles = { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }
    }
}