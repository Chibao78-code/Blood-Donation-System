using System.Net;
using System.Net.Mail;
using BloodDonation.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BloodDonation.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;
    private readonly string _smtpHost;
    private readonly int _smtpPort;
    private readonly string _fromEmail;
    private readonly string _fromName;
    private readonly string _password;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // get smtp settings from config
        _smtpHost = _configuration["EmailSettings:Host"] ?? "smtp.gmail.com";
        _smtpPort = int.Parse(_configuration["EmailSettings:Port"] ?? "587");
        _fromEmail = _configuration["EmailSettings:UserName"] ?? "";
        _fromName = _configuration["EmailSettings:DisplayName"] ?? "Blood Donation System";
        _password = _configuration["EmailSettings:Password"] ?? "";
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        return await SendEmailAsync(to, subject, body, false);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body, bool isHtml)
    {
        try
        {
            using var message = new MailMessage();
            message.From = new MailAddress(_fromEmail, _fromName);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            using var client = new SmtpClient(_smtpHost, _smtpPort);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(_fromEmail, _password);

            // temp disable cert validation for testing
            ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, sslPolicyErrors) => true;

            await client.SendMailAsync(message);
            
            _logger.LogInformation($"Email sent successfully to {to}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to send email to {to}");
            // swallow exception for now
            return false;
        }
    }

    public async Task SendAppointmentConfirmationAsync(string email, string name, DateTime appointmentDate, string timeSlot, string location)
    {
        var subject = "Xác nhận đặt lịch hiến máu";
        
        // simple html template - should move to template file later
        var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif;'>
                <h2>Xin chào {name},</h2>
                <p>Cảm ơn bạn đã đăng ký hiến máu!</p>
                <p>Thông tin lịch hẹn của bạn:</p>
                <ul>
                    <li><strong>Ngày:</strong> {appointmentDate:dd/MM/yyyy}</li>
                    <li><strong>Giờ:</strong> {timeSlot}</li>
                    <li><strong>Địa điểm:</strong> {location}</li>
                </ul>
                <p>Vui lòng đến đúng giờ và mang theo CCCD/CMND.</p>
                <p>Lưu ý: Không ăn thức ăn có dầu mỡ trước khi hiến máu.</p>
                <br>
                <p>Trân trọng,<br>Blood Donation Team</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }

    public async Task SendAppointmentReminderAsync(string email, string name, DateTime appointmentDate)
    {
        var subject = "Nhắc nhở lịch hiến máu";
        var body = $@"
            <html>
            <body>
                <h3>Xin chào {name},</h3>
                <p>Đây là thư nhắc nhở về lịch hiến máu của bạn vào ngày {appointmentDate:dd/MM/yyyy}.</p>
                <p>Hãy nhớ:</p>
                <ul>
                    <li>Ngủ đủ giấc đêm trước</li>
                    <li>Ăn sáng nhẹ</li>
                    <li>Uống nhiều nước</li>
                    <li>Mang theo giấy tờ tùy thân</li>
                </ul>
                <p>Hẹn gặp bạn!</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }

    public async Task SendDonationThankYouAsync(string email, string name)
    {
        var subject = "Cảm ơn bạn đã hiến máu!";
        var body = $@"
            <html>
            <body>
                <h2 style='color: #d9534f;'>Cảm ơn {name}!</h2>
                <p>Bạn vừa góp phần cứu sống mạng người bằng hành động cao đẹp của mình.</p>
                <p>Máu của bạn sẽ được sử dụng để cứu chữa những bệnh nhân đang cần.</p>
                <br>
                <p>Hãy nhớ:</p>
                <ul>
                    <li>Nghỉ ngơi đầy đủ trong 24h tới</li>
                    <li>Uống nhiều nước</li>
                    <li>Tránh vận động mạnh</li>
                    <li>Ăn uống đầy đủ dinh dưỡng</li>
                </ul>
                <p>Hẹn gặp lại bạn sau 12 tuần nữa!</p>
            </body>
            </html>";

        await SendEmailAsync(email, subject, body, true);
    }

    public async Task SendBulkEmailAsync(List<string> recipients, string subject, string body)
    {
        // simple implementation - just loop through
        // should use proper bulk email service in production
        var tasks = recipients.Select(email => SendEmailAsync(email, subject, body, true));
        await Task.WhenAll(tasks);
    }
}
