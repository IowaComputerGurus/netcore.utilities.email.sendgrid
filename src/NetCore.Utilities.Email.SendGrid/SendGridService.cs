using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace ICG.NetCore.Utilities.Email.SendGrid
{
    /// <inheritdoc />
    public class SendGridService : IEmailService
    {
        private readonly SendGridServiceOptions _serviceOptions;
        private readonly ISendGridMessageBuilder _messageBuilder;
        private readonly ISendGridSender _sender;

        /// <inheritdoc />
        public string AdminEmail => _serviceOptions?.AdminEmail;

        /// <inheritdoc />
        public string AdminName => _serviceOptions?.AdminName;
        
        /// <summary>
        ///     DI Capable Constructor for SendGrid message delivery using MimeKit/MailKit
        /// </summary>
        /// <param name="serviceOptions"></param>
        /// <param name="messageBuilder"></param>
        /// <param name="sender"></param>
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
        public bool SendMessage(string toAddress, string subject, string bodyHtml, List<KeyValuePair<string, string>> tokens)
        {
            return SendMessage(toAddress, null, subject, bodyHtml, null, "");
        }

        /// <inheritdoc />
        public bool SendMessage(string toAddress, IEnumerable<string> ccAddressList, string subject, string bodyHtml)
        {
            return SendMessage(toAddress, ccAddressList, subject, bodyHtml, null, "");
        }

        /// <inheritdoc />
        public bool SendMessage(string toAddress, IEnumerable<string> ccAddressList, string subject, string bodyHtml, List<KeyValuePair<string, string>> tokens)
        {
            return SendMessage(toAddress, ccAddressList, subject, bodyHtml, tokens, "");
        }

        
        /// <inheritdoc />
        public bool SendMessage(string toAddress, IEnumerable<string> ccAddressList, string subject, string bodyHtml,
            List<KeyValuePair<string, string>> tokens,
            string templateName, string senderKeyName = "")
        {
            if (tokens != null)
            {
                foreach (var item in tokens)
                {
                    bodyHtml = bodyHtml.Replace(item.Key, item.Value);
                }
            }

            //Get the message to send
            var toSend = _messageBuilder.CreateMessage(_serviceOptions.AdminEmail, _serviceOptions.AdminName, toAddress, ccAddressList, subject,
                bodyHtml, templateName);

            //Determine the key to use
            var apiKey = _serviceOptions.SendGridApiKey;
            if (!string.IsNullOrEmpty(senderKeyName))
                apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

            //Send
            return _sender.SendMessage(apiKey, toSend).GetAwaiter().GetResult();
        }

        /// <inheritdoc />
        public bool SendMessageWithAttachment(string toAddress, IEnumerable<string> ccAddressList, string subject,
            byte[] fileContent, string fileName, string bodyHtml, List<KeyValuePair<string, string>> tokens, string templateName = "", string senderKeyName = "")
        {
            //Get the message to send
            var toSend = _messageBuilder.CreateMessageWithAttachment(_serviceOptions.AdminEmail, _serviceOptions.AdminName, toAddress,
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