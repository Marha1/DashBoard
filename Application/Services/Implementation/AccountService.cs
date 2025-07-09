using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Application.Dtos.AccountDtos;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Application.Services.Implementation;

public class AccountService : IAccountService
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly UserManager<AppUser> _userManager;
    private readonly IEmailSender _emailSender;

    public AccountService(UserManager<AppUser> userManager,
        IConfiguration configuration, IMapper mapper, IEmailSender emailSender)
    {
        _userManager = userManager;
        _configuration = configuration;
        _mapper = mapper;
        _emailSender = emailSender;
    }

    /// <summary>
    ///     Генерация,сохранение в бд,отправка токена на почту для сброса
    /// </summary>
    /// <param name="email"></param>
    /// <exception cref="Exception"></exception>
    public async Task SendResetPasswordEmailAsync(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var resetToken = new Random().Next(100000, 999999).ToString("D6"); // 6-значный токен
            // Сохранение токена в таблицу AspNetUserTokens
            var result =
                await _userManager.SetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken", resetToken);
            if (!result.Succeeded)
                throw new Exception(
                    $"Failed to store password reset token: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            await _emailSender.SendResetPasswordEmailAsync(email, resetToken);
        }
        catch (Exception ex)
        {
            // Логируем все ошибки
            Console.WriteLine($"Error in SendResetPasswordEmailAsync: {ex.Message}");
            throw; // Можно повторно выбросить исключение, чтобы обработать его выше
        }
    }


    /// <summary>
    ///     Сброс пароля
    /// </summary>
    /// <param name="email"></param>
    /// <param name="resetToken"></param>
    /// <param name="newPassword"></param>
    /// <exception cref="Exception"></exception>
    public async Task ResetPasswordAsync(string email, string resetToken, string newPassword)
    {
        // Найти пользователя
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) throw new Exception("User not found");

        // Получить токен из AspNetUserTokens
        var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
        if (storedToken != resetToken) throw new Exception("Invalid or expired password reset token");

        // Изменить пароль вручную
        var resetResult = await _userManager.RemovePasswordAsync(user);
        if (!resetResult.Succeeded)
            throw new Exception(
                $"Failed to remove the old password: {string.Join(", ", resetResult.Errors.Select(e => e.Description))}");

        var setPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
        if (!setPasswordResult.Succeeded)
            throw new Exception(
                $"Failed to set the new password: {string.Join(", ", setPasswordResult.Errors.Select(e => e.Description))}");

        // Удалить токен после успешного сброса
        await _userManager.RemoveAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
    }


    /// <summary>
    ///     Регистрация пользователя
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Сообщение</returns>
    /// <exception cref="Exception"></exception>
    public async Task<string> RegisterUserAsync(RegisterDto dto)
    {
        var user = _mapper.Map<User>(dto);
        user.UserName = dto.UserName;
        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            throw new Exception(
                $"User registration failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");

        var roleResult = await _userManager.AddToRoleAsync(user, "User");
        if (!roleResult.Succeeded)
            throw new Exception(
                $"Assigning role failed: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");

        return "User registered successfully";
    }

    /// <summary>
    ///     Авторизация
    /// </summary>
    /// <param name="dto"></param>
    /// <returns>Токен авторизации</returns>
    /// <exception cref="UnauthorizedAccessException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public async Task<UserInfo> LoginAsync(LoginDto dto)
{
    var user = await _userManager.FindByEmailAsync(dto.Login) ??
               await _userManager.FindByNameAsync(dto.Login);

    if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
        throw new UnauthorizedAccessException("Invalid login or password");

    if (!user.EmailConfirmed) throw new UnauthorizedAccessException("Email is not confirmed");

    if (await _userManager.IsLockedOutAsync(user)) throw new UnauthorizedAccessException("User is locked out.");

    var roles = await _userManager.GetRolesAsync(user);

    var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? "Unknown"),
        new(ClaimTypes.Email, user.Email ?? string.Empty)
    };
    claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ??
                                     throw new InvalidOperationException("JWT Key is not configured or is invalid"));
    if (key.Length < 32)
        throw new InvalidOperationException("JWT Key length must be at least 256 bits (32 bytes).");

    var issuer = _configuration["Jwt:Issuer"] ??
                 throw new InvalidOperationException("JWT Issuer is not configured.");
    var audience = _configuration["Jwt:Audience"] ??
                   throw new InvalidOperationException("JWT Audience is not configured.");

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.UtcNow.AddHours(1),
        Issuer = issuer,
        Audience = audience,
        SigningCredentials =
            new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var tokenHandler = new JwtSecurityTokenHandler();
    var token = tokenHandler.CreateToken(tokenDescriptor);

    return new UserInfo
    {
        Id = user.Id,
        UserName = user.UserName ?? string.Empty,
        Email = user.Email ?? string.Empty,
        Roles = roles.ToList(),
        Token = tokenHandler.WriteToken(token)
    };
}


    public async Task SendEmailConfirmedCode(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) throw new Exception("User not found");

            var confirm = new Random().Next(100000, 999999).ToString("D6"); // 6-значный токен
            var result =
                await _userManager.SetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken", confirm);
            if (!result.Succeeded)
                throw new Exception(
                    $"Failed to store password reset token: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            await _emailSender.SendEmailConfirmedCode(email, confirm);
        }
        catch (Exception ex)
        {
            // Логируем все ошибки
            Console.WriteLine($"Error in SendResetPasswordEmailAsync: {ex.Message}");
            throw; // Можно повторно выбросить исключение, чтобы обработать его выше
        }
    }

    public async Task EmailConfirmed(string email, string confirmedCode)
    {
        var user = await _userManager.FindByEmailAsync(email);
        try
        {
            // Найти пользователя
            if (user == null) throw new Exception("User not found");

            // Получить токен из AspNetUserTokens
            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
            if (storedToken != confirmedCode) throw new Exception("Invalid or expired password reset token");
            user.EmailConfirmed = true;
            // Удалить токен после успешного сброса
            await _userManager.RemoveAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await _userManager.RemoveAuthenticationTokenAsync(user, "PasswordReset", "ResetToken");
            throw;
            
        }
    }
}