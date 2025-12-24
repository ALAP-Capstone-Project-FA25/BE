namespace ALAP.DAL.Interface
{
    public interface IEmailService
    {
        Task<bool> SendEmailAsync(string to, string subject, string templateFileName, Dictionary<string, string> placeholders);
        Task SendWorker(string to, string subject, string templateFileName, Dictionary<string, string> placeholders);
    }
}
