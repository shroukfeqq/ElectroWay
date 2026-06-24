using ElctroWay.Data;
using ElctroWay.Models.Identity;
using ElctroWay.Models.Provider;
using ElctroWay.Repositories.Interfaces;

namespace ElctroWay.Repositories.Implementations
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddProviderProfileAsync(ProviderProfile profile)
        {
            await _context.ProviderProfiles.AddAsync(profile);
        }

        public async Task AddDocumentsAsync(List<ProviderDocument> documents)
        {
            await _context.ProviderDocuments.AddRangeAsync(documents);
        }
    }
}