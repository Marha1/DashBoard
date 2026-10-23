namespace Domain.ValueObject;
/// <summary>
/// Вложенная сущность
/// </summary>
public record FullName(string FirstName, string LastName, string? MiddleName);
