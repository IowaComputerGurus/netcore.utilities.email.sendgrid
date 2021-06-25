using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace ICG.NetCore.Utilities.Email.SendGrid
{
    /// <summary>
    ///     Represents an SendGrid service that can be used to send outbound email messages.  Internally the current concrete
    ///     implementation will utilize the MailKit/MimeKit services per the recommendation of Microsoft.
    /// </summary>
    /// <remarks>
    ///     In your project you should use this service only.
    /// </remarks>
    public interface ISendGridService
    {
        /// <summary>
        /// Returns the configured administrator email for the SendGrid service
        /// </summary>
        string AdminEmail { get; }

        /// <summary>
        ///     Shortcut for sending an email to the administrator, only requiring the subject and body.
        /// </summary>
        /// <param name="subject">The message subject</param>
        /// <param name="bodyHtml">The message body</param>
        bool SendMessageToAdministrator(string subject, string bodyHtml);

        /// <summary>
        /// Sends a message to the administrator as well as the additional contacts provided.
        /// </summary>
        /// <param name="ccAddressList">Additional email addresses to add to the CC line</param>
        /// <param name="subject">The email subject</param>
        /// <param name="bodyHtml">The HTML content of the email</param>
        bool SendMessageToAdministrator(IEnumerable<string> ccAddressList, string subject, string bodyHtml);

        /// <summary>
        ///     Sends a message to the specified recipient, with the supplied subject and body
        /// </summary>
        /// <param name="toAddress">Who is receiving the email</param>
        /// <param name="subject">The message subject</param>
        /// <param name="bodyHtml">The message body</param>
        bool SendMessage(string toAddress, string subject, string bodyHtml);

        /// <summary>
        ///     Sends a message to the specified recipient, and CC's with the supplied subject and body
        /// </summary>
        /// <param name="toAddress">Who is receiving the email</param>
        /// <param name="ccAddressList">Additional CC'ed emails</param>
        /// <param name="subject">The message subject</param>
        /// <param name="bodyHtml">The message body</param>
        /// <param name="templateName">The optional custom template to override with</param>
        /// <param name="senderKeyName">The custom key for API usage if needed</param>
        bool SendMessage(string toAddress, IEnumerable<string> ccAddressList, string subject, string bodyHtml, string templateName = "", string senderKeyName = "");

        /// <summary>
        ///  Creates a message with an attachment
        /// </summary>
        /// <param name="toAddress">The to address for the message</param>
        /// <param name="ccAddressList">The address(ses) to add a CC's</param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="fileContent">Attachment Content</param>
        /// <param name="fileName">Attachment file name</param>
        /// <param name="bodyHtml">The HTML body contents</param>
        /// <param name="templateName">The optional custom template to override with</param>
        /// <param name="senderKeyName">The custom key for API usage if needed</param>
        /// <returns></returns>
        bool SendMessageWithAttachment(string toAddress, IEnumerable<string> ccAddressList, string subject,
            byte[] fileContent, string fileName, string bodyHtml, string templateName = "", string senderKeyName = "");
    }

    /// <inheritdoc />
    public class SendGridService : ISendGridService
    {
        private readonly SendGridServiceOptions _serviceOptions;
        private readonly ISendGridMessageBuilder _messageBuilder;
        private readonly ISendGridSender _sender;

        /// <inheritdoc />
        public string AdminEmail => _serviceOptions?.AdminEmail;
        
        /// <summary>
        ///     DI Capable Constructor for SendGrid message delivery using MimeKit/MailKit
        /// </summary>
        /// <param name="serviceOptions"></param>
        /// <param name="messageBuilder"></param>
        public SendGridService(IOptions<SendGridServiceOptions> serviceOptions, ISendGridMessageBuilder messageBuilder, ISendGridSender sender)
        {
            _messageBuilder = messageBuilder;
            _sender = sender;
            _serviceOptions = serviceOptions.Value;
        }

        /// <inheritdoc />
        public bool SendMessageToAdministrator(string subject, string bodyHtml)
        {
            //Force to address
            return SendMessage(_serviceOptions.AdminEmail, null, subject, bodyHtml);
        }

        /// <inheritdoc />
        public bool SendMessageToAdministrator(IEnumerable<string> ccAddressList, string subject, string bodyHtml)
        {
            return SendMessage(_serviceOptions.AdminEmail, ccAddressList, subject, bodyHtml);
        }

        /// <inheritdoc />
        public bool SendMessage(string toAddress, string subject, string bodyHtml)
        {
            //Call full overload
            return SendMessage(toAddress, null, subject, bodyHtml);
        }

        /// <inheritdoc />
        public bool SendMessage(string toAddress, IEnumerable<string> ccAddressList, string subject, string bodyHtml, string templateName = "", string senderKeyName = "")
        {
            //Get the message to send
            var toSend = _messageBuilder.CreateMessage(_serviceOptions.AdminEmail, toAddress, ccAddressList, subject,
                bodyHtml, templateName);

            //Determine the key to use
            var apiKey = _serviceOptions.SendGridApiKey;
            if (!string.IsNullOrEmpty(senderKeyName))
                apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

            //Send
            return _sender.SendMessage(apiKey, toSend).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public bool SendMessageWithAttachment(string toAddress, IEnumerable<string> ccAddressList, string subject, byte[] fileContent, string fileName, string bodyHtml, string templateName = "", string senderKeyName = "")
        {
            //Get the message to send
            var toSend = _messageBuilder.CreateMessageWithAttachment(_serviceOptions.AdminEmail, toAddress,
                ccAddressList, fileContent, fileName, subject, bodyHtml, templateName);

            //Determine the key to use
            var apiKey = _serviceOptions.SendGridApiKey;
            if (!string.IsNullOrEmpty(senderKeyName))
                apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

            //Send
            return _sender.SendMessage(apiKey, toSend).GetAwaiter().GetResult();
        }
    }
}