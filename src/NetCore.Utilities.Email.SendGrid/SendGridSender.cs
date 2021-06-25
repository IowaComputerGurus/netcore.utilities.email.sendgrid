using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace ICG.NetCore.Utilities.Email.SendGrid
{
    /// <summary>
    ///     Internal service to abstract the actual SendGrid implementation to allow for proper unit testing of the remainder
    ///     of the project.  Not intended for external usage
    /// </summary>
    public interface ISendGridSender
    {
        /// <summary>
        ///     Sends a mail message using the provided API key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        Task<bool> SendMessage(string apiKey, SendGridMessage message);
    }

    /// <inheritdoc />
    public class SendGridSender : ISendGridSender
    {
        /// <inheritdoc />
        public async Task<bool> SendMessage(string apiKey, SendGridMessage message)
        {
            var client = new SendGridClient(apiKey);
            var result = await client.SendEmailAsync(message);
            return result.IsSuccessStatusCode;
        }
    }
}