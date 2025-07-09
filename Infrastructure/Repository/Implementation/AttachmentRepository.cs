    using Domain.Interfaces.Repository;
    using Domain.Models;
    using Microsoft.EntityFrameworkCore;

    namespace Infrastructure.Repository.Implementation;
    /// <summary>
    /// Реализация репозитория для вложений
    /// </summary>
    public class AttachmentRepository : BaseRepository<Attachment>, IAttachmentRepository
    {
        public AttachmentRepository(ApplicationContext context) : base(context) { }

        public async Task<ICollection<Attachment>> GetAttachmentsByRequestIdAsync(Guid requestId, CancellationToken cancellationToken = default)
        {
            return await _context.Attachments
                .Where(a => a.AdvertId == requestId)
                .ToListAsync(cancellationToken);
        }
    }