namespace Wholesale.Application.Common.Interfaces;

/// <summary>
/// Şifre hashleme ve doğrulama işlemleri için uygulama katmanı sözleşmesi.
/// Implementasyon Infrastructure katmanındadır (BCrypt).
/// </summary>
public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string password, string hash);
}
