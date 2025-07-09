using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Interfaces;

public interface IAttachmentService
{
    Task<Attachment> UploadAttachmentAsync(IFormFile file, Guid requestId, CancellationToken cancellationToken);
    Task<bool> DeleteAttachmentAsync(Guid attachmentId, CancellationToken cancellationToken);
}