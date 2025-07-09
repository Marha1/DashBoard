using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IUserAdvertRepository: IRepository<UserAdvert>
{
    public Task<UserAdvert?> GetAdvertstWithAttachmentsAsync(Guid requestId,
        CancellationToken cancellationToken = default);


}