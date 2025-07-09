using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IAttachmentRepository : IRepository<Attachment>
{
    Task<ICollection<Attachment>> GetAttachmentsByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default);
}