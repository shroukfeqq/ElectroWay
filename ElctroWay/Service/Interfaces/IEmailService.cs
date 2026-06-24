namespace ElctroWay.Service.Interfaces
{
    public interface IEmailService
    {
        void SendOtp(string email, string otp);
    }
}
