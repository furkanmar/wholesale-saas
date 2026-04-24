using Wholesale.Domain.Common;

namespace Wholesale.Domain.Entities;

public class Tenant : BaseAuditableEntity
{
    public string Name { get; set; } = string.Empty;

    /// <summary>Benzersiz slug — subdomain veya kod olarak kullanılır (ör. "marifoglu")</summary>
    public string Code { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    public string Plan { get; set; } = "trial";

    public ICollection<User> Users { get; set; } = new List<User>();
}
