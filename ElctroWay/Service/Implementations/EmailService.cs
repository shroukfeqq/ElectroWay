using ElctroWay.Service.Interfaces;
using System.Net.Mail;
using System.Net;

namespace ElctroWay.Service.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public void SendOtp(string email, string otp)
        {
            var smtp = new SmtpClient(_config["EmailSettings:Smtp"])
            {
                Port = int.Parse(_config["EmailSettings:Port"]),
                Credentials = new NetworkCredential(
                    _config["EmailSettings:Email"],
                    _config["EmailSettings:AppPassword"]
                ),
                EnableSsl = true
            };

            smtp.Send(
                _config["EmailSettings:Email"],
                email,
                "OTP Code",
                $"Your OTP is {otp}"
            );
        }
    }
}
