using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace ICG.NetCore.Utilities.Email.SendGrid
{
    /// <summary>
    /// The service takes an incoming request and build the proper SendGrid message structure for composing the messages
    /// </summary>
    public interface ISendGridMessageBuilder
    {
        /// <summary>
        /// Creates a simple message for sending with a custom template
        /// </summary>
        /// <param name="from">Who the message is from</param>
        /// <param name="fromName">The name of the sender</param>
        /// <param name="to">Who the message is to</param>
        /// <param name="cc">An optional listing of CC addresses</param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="bodyHtml">The Email's HTML content</param>
        /// <param name="templateName">The name of the template to use</param>
        /// <returns></returns>
        SendGridMessage CreateMessage(string from, string fromName, string to, IEnumerable<string> cc, string subject, string bodyHtml,
            string templateName = "");

        /// <summary>
        /// Creates a simple message for sending with a custom template and attachment
        /// </summary>
        /// <param name="from">Who the message is from</param>
        /// <param name="fromName">The name of the sender</param>
        /// <param name="to">Who the message is to</param>
        /// <param name="cc">An optional listing of CC addresses</param>
        /// <param name="fileContent">The content of the attachment in bytes</param>
        /// <param name="fileName">The desired name for the file attachment</param>
        /// <param name="subject">The subject of the message</param>
        /// <param name="bodyHtml">The Email's HTML content</param>
        /// <param name="templateName">The name of the template to use</param>
        SendGridMessage CreateMessageWithAttachment(string from, string fromName, string to, IEnumerable<string> cc,
            byte[] fileContent, string fileName, string subject, string bodyHtml, string templateName = "");
    }

    /// <inheritdoc cref="ISendGridMessageBuilder"/>
    public class SendGridMessageBuilder : ISendGridMessageBuilder
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmailTemplateFactory _emailTemplateFactory;
        private readonly SendGridServiceOptions _serviceOptions;
        private readonly ILogger _logger;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="emailTemplateFactory"></param>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        public SendGridMessageBuilder(IHostingEnvironment hostingEnvironment, IEmailTemplateFactory emailTemplateFactory, 
            IOptions<SendGridServiceOptions> options, ILogger<SendGridMessageBuilder> logger)
        {
            _hostingEnvironment = hostingEnvironment;
            _emailTemplateFactory = emailTemplateFactory;
            _serviceOptions = options.Value;
            _logger = logger;
        }
        
        /// <inheritdoc />
        public SendGridMessage CreateMessage(string from, string fromName, string to, IEnumerable<string> cc, string subject, string bodyHtml, string templateName = "")
        {
            //Validate inputs
            if (string.IsNullOrEmpty(from))
                throw new ArgumentNullException(nameof(from));
            if (string.IsNullOrEmpty(to))
                throw new ArgumentNullException(nameof(to));
            if (string.IsNullOrEmpty(subject))
                throw new ArgumentNullException(nameof(subject));
            if (string.IsNullOrEmpty(bodyHtml))
                throw new ArgumentNullException(nameof(bodyHtml));

            //Get addresses
            var fromAddress = new EmailAddress(from);
            if (!string.IsNullOrEmpty(fromName))
                fromAddress.Name = fromName;
            var recipients = new List<EmailAddress> {new EmailAddress(to)};
            if (cc != null)
            {
                foreach (var item in cc)
                {
                    try
                    {
                        recipients.Add(new EmailAddress(item));
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, $"Unable to add {item} to email copy list");
                    }
                }
            }
            
            //Handle subjects
            if (_serviceOptions.AddEnvironmentSuffix && !_hostingEnvironment.IsProduction())
                subject = $"{subject} ({_hostingEnvironment.EnvironmentName})";

            //Perform templating
            if (_serviceOptions.AlwaysTemplateEmails && string.IsNullOrEmpty(templateName))
                bodyHtml = _emailTemplateFactory.BuildEmailContent(subject, bodyHtml);
            else if (!string.IsNullOrEmpty(templateName))
                bodyHtml = _emailTemplateFactory.BuildEmailContent(subject, bodyHtml,
                    templateName: templateName);

            //Get body text
            var plainTextBody = Regex.Replace(bodyHtml, "<[^>]*>", "");
            
            //Build message
            if (recipients.Count == 1)
                return MailHelper.CreateSingleEmail(fromAddress, recipients[0], subject, plainTextBody, bodyHtml);

            return MailHelper.CreateSingleEmailToMultipleRecipients(fromAddress, recipients, subject, plainTextBody,
                bodyHtml);
        }

        /// <inheritdoc />
        public SendGridMessage CreateMessageWithAttachment(string from, string fromName, string to, IEnumerable<string> cc,
            byte[] fileContent, string fileName, string subject, string bodyHtml, string templateName = "")
        {
            //Build the  basic message
            var toSend = CreateMessage(from, fromName, to, cc, subject, bodyHtml, templateName);

            //Attach file
            toSend.Attachments = new List<Attachment>
            {
                new Attachment
                {
                    Content = Convert.ToBase64String(fileContent),
                    Filename = fileName,
                    Disposition = "attachment"
                }
            };
            return toSend;
        }

    }
}
