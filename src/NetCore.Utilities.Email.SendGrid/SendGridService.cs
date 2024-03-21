using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace ICG.NetCore.Utilities.Email.SendGrid;

/// <inheritdoc />
public class SendGridService : IEmailService
{
    private readonly ISendGridMessageBuilder _messageBuilder;
    private readonly ISendGridSender _sender;
    private readonly SendGridServiceOptions _serviceOptions;

    /// <summary>
    ///     DI Capable Constructor for SendGrid message delivery using MimeKit/MailKit
    /// </summary>
    /// <param name="serviceOptions"></param>
    /// <param name="messageBuilder"></param>
    /// <param name="sender"></param>
    public SendGridService(IOptions<SendGridServiceOptions> serviceOptions, ISendGridMessageBuilder messageBuilder,
        ISendGridSender sender)
    {
        _messageBuilder = messageBuilder;
        _sender = sender;
        _serviceOptions = serviceOptions.Value;
    }

    /// <inheritdoc />
    public string AdminEmail => _serviceOptions?.AdminEmail;

    /// <inheritdoc />
    public string AdminName => _serviceOptions?.AdminName;

    /// <inheritdoc />
    public Task<bool> SendMessageToAdministratorAsync(string subject, string bodyHtml)
    {
        //Force to address
        return SendMessageAsync(_serviceOptions.AdminEmail, null, subject, bodyHtml);
    }

    /// <inheritdoc />
    public Task<bool> SendMessageToAdministratorAsync(IEnumerable<string> ccAddressList, string subject,
        string bodyHtml)
    {
        return SendMessageAsync(_serviceOptions.AdminEmail, ccAddressList, subject, bodyHtml);
    }

    /// <inheritdoc />
    public Task<bool> SendMessageAsync(string toAddress, string subject, string bodyHtml)
    {
        //Call full overload
        return SendMessageAsync(toAddress, null, subject, bodyHtml);
    }

    /// <inheritdoc />
    public Task<bool> SendMessageAsync(string toAddress, string subject, string bodyHtml,
        List<KeyValuePair<string, string>> tokens)
    {
        return SendMessageAsync(toAddress, null, subject, bodyHtml, null, "");
    }

    /// <inheritdoc />
    public Task<bool> SendMessageAsync(string toAddress, IEnumerable<string> ccAddressList, string subject,
        string bodyHtml)
    {
        return SendMessageAsync(toAddress, ccAddressList, subject, bodyHtml, null, "");
    }

    /// <inheritdoc />
    public Task<bool> SendMessageAsync(string toAddress, IEnumerable<string> ccAddressList, string subject,
        string bodyHtml, List<KeyValuePair<string, string>> tokens)
    {
        return SendMessageAsync(toAddress, ccAddressList, subject, bodyHtml, tokens, "");
    }


    /// <inheritdoc />
    public Task<bool> SendMessageAsync(string toAddress, IEnumerable<string> ccAddressList, string subject,
        string bodyHtml,
        List<KeyValuePair<string, string>> tokens,
        string templateName, string senderKeyName = "")
    {
        if (tokens != null)
            foreach (var item in tokens)
                bodyHtml = bodyHtml.Replace(item.Key, item.Value);

        //Get the message to send
        var toSend = _messageBuilder.CreateMessage(_serviceOptions.AdminEmail, _serviceOptions.AdminName, toAddress,
            ccAddressList, subject,
            bodyHtml, templateName);

        //Determine the key to use
        var apiKey = _serviceOptions.SendGridApiKey;
        if (!string.IsNullOrEmpty(senderKeyName))
            apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

        //Send
        return _sender.SendMessage(apiKey, toSend);
    }

    /// <inheritdoc />
    public Task<bool> SendWithReplyToAsync(string replyToAddress, string replyToName, string toAddress, string subject,
        string bodyHtml)
    {
        //Call full overload
        return SendWithReplyToAsync(replyToAddress, replyToName, toAddress, null, subject, bodyHtml);
    }

    /// <inheritdoc />
    public Task<bool> SendWithReplyToAsync(string replyToAddress, string replyToName, string toAddress, string subject,
        string bodyHtml, List<KeyValuePair<string, string>> tokens)
    {
        return SendWithReplyToAsync(replyToAddress, replyToName, toAddress, null, subject, bodyHtml, null, "");
    }

    /// <inheritdoc />
    public Task<bool> SendWithReplyToAsync(string replyToAddress, string replyToName, string toAddress,
        IEnumerable<string> ccAddressList, string subject, string bodyHtml)
    {
        return SendWithReplyToAsync(replyToAddress, replyToName, toAddress, ccAddressList, subject, bodyHtml, null, "");
    }

    /// <inheritdoc />
    public Task<bool> SendWithReplyToAsync(string replyToAddress, string replyToName, string toAddress,
        IEnumerable<string> ccAddressList, string subject, string bodyHtml, List<KeyValuePair<string, string>> tokens)
    {
        return SendWithReplyToAsync(replyToAddress, replyToName, toAddress, ccAddressList, subject, bodyHtml, tokens,
            "");
    }


    /// <inheritdoc />
    public Task<bool> SendWithReplyToAsync(string replyToAddress, string replyToName, string toAddress,
        IEnumerable<string> ccAddressList, string subject, string bodyHtml,
        List<KeyValuePair<string, string>> tokens,
        string templateName, string senderKeyName = "")
    {
        if (string.IsNullOrEmpty(replyToAddress))
            throw new ArgumentNullException(nameof(replyToAddress));

        if (tokens != null)
            foreach (var item in tokens)
                bodyHtml = bodyHtml.Replace(item.Key, item.Value);

        //Get the message to send
        var toSend = _messageBuilder.CreateMessage(_serviceOptions.AdminEmail, _serviceOptions.AdminName, toAddress,
            ccAddressList, subject,
            bodyHtml, templateName);

        if (!string.IsNullOrEmpty(replyToAddress))
        {
            var replyTo = new EmailAddress(replyToAddress);
            if (!string.IsNullOrEmpty(replyToName))
                replyTo.Name = replyToName;
            toSend.ReplyTo = replyTo;
        }

        //Determine the key to use
        var apiKey = _serviceOptions.SendGridApiKey;
        if (!string.IsNullOrEmpty(senderKeyName))
            apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

        //Send
        return _sender.SendMessage(apiKey, toSend);
    }

    /// <inheritdoc />
    public Task<bool> SendMessageWithAttachmentAsync(string toAddress, IEnumerable<string> ccAddressList,
        string subject,
        byte[] fileContent, string fileName, string bodyHtml, List<KeyValuePair<string, string>> tokens,
        string templateName = "", string senderKeyName = "")
    {
        //Get the message to send
        var toSend = _messageBuilder.CreateMessageWithAttachment(_serviceOptions.AdminEmail, _serviceOptions.AdminName,
            toAddress,
            ccAddressList, fileContent, fileName, subject, bodyHtml, templateName);

        //Determine the key to use
        var apiKey = _serviceOptions.SendGridApiKey;
        if (!string.IsNullOrEmpty(senderKeyName))
            apiKey = _serviceOptions.AdditionalApiKeys[senderKeyName];

        //Send
        return _sender.SendMessage(apiKey, toSend);
    }
}