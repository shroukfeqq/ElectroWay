//using ElctroWay.Models.Identity;
using ElctroWay.Models.Identity;
using ElctroWay.Models.Provider;

namespace ElctroWay.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task AddProviderProfileAsync(ProviderProfile profile);

        Task AddDocumentsAsync(List<ProviderDocument> documents);
    }
}