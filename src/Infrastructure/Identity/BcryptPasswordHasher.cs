using BC = BCrypt.Net.BCrypt;
using Wholesale.Application.Common.Interfaces;

namespace Wholesale.Infrastructure.Identity;

public class BcryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)   => BC.HashPassword(password);
    public bool   Verify(string password, string hash) => BC.Verify(password, hash);
}
