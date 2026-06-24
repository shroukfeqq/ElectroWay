using ElctroWay.DTOs;
using ElctroWay.DTOs.AuthDto;
using ElctroWay.DTOs.Common;

namespace ElctroWay.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<object>> RegisterProviderAsync(ProviderRegisterDto dto);

        Task<ApiResponse<object>> RegisterDriverAsync(DriverRegisterDto dto);

        Task<ApiResponse<object>> LoginAsync(LoginDto dto);
    }
}