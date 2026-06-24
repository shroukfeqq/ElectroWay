using ElctroWay.Models.Identity;

namespace ElctroWay.Service.Interfaces
{
    public interface IOcrService
    {
        OcrResult VerifyAsync(
            string frontIdPath,
            string backIdPath,
            string selfiePath,
            string fullNameFromForm);
    }
}
