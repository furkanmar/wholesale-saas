namespace Wholesale.Application.Common.Models;

/// <summary>
/// Genel amaçlı Id + başlık DTO'su (dropdown listesi vb. için).
/// </summary>
public class LookupDto
{
    public Guid Id { get; init; }
    public string? Title { get; init; }
}
