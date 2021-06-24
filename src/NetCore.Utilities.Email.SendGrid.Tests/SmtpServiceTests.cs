using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ICG.NetCore.Utilities.Email.SendGrid.Tests
{
    public class SendGridServiceTests
    {
        private readonly SendGridServiceOptions _options = new SendGridServiceOptions()
        {
            AdminEmail = "admin@test.com",
            SendGridApiKey = "APIKEY",
            AddEnvironmentSuffix = false,
            AlwaysTemplateEmails = false
        };
        private readonly ISendGridService _service;

        public SendGridServiceTests()
        {
            _service = new SendGridService(new OptionsWrapper<SendGridServiceOptions>(_options));
        }

        [Fact]
        public void AdminEmail_ShouldReturnConfigurationEmail()
        {
            //Arrange
            var expectedEmail = "admin@test.com";

            //Act
            var result = _service.AdminEmail;

            //Assert
            Assert.Equal(expectedEmail, result);
        }

        [Fact]
        public void AdminEmail_ShouldReturnNullWhenNoConfiguration()
        {
            //Arrange
            var testService = new SendGridService(new OptionsWrapper<SendGridServiceOptions>(null));

            //Act
            var result = testService.AdminEmail;

            //Assert
            Assert.Null(result);
        }

        [Fact]
        public void SendToAdministrator_ShouldSend_DefaultingFromAndToAddress()
        {
            //Arrange
            var subject = "Test";
            var message = "Message";

            //Act
            _service.SendMessageToAdministrator(subject, message);

            //Verify
        }

        [Fact]
        public void SendToAdministrator_ShouldSend_DefaultingFromAndToAddress_WithCCRecipients()
        {
            //Arrange
            var subject = "Test";
            var message = "Message";
            var cc = new List<string> {"recipient@test.com"};

            //Act
            _service.SendMessageToAdministrator(cc, subject, message);

            //Verify
        }

        [Fact]
        public void SendMessage_WithoutCCRecipients_ShouldSend_DefaultingFromAddress()
        {
            //Arrange
            var to = "tester@test.com";
            var subject = "test";
            var message = "message";

            //Act
            _service.SendMessage(to, subject, message);

            //Verify
        }

        [Fact]
        public void SendMessage_WithCCRecipients_ShouldSend_DefaultingFromAddress()
        {
            //Arrange
            var to = "tester@test.com";
            var cc = new List<string> {"Person1@test.com"};
            var subject = "test";
            var message = "message";

            //Act
            _service.SendMessage(to, cc, subject, message);

            //Verify
        }

        [Fact]
        public void SendMessageWithAttachment_ShouldSend_DefaultingFromAddress()
        {
            //Arrange
            var to = "tester@test.com";
            var cc = new List<string> { "Person1@test.com" };
            var subject = "test";
            var fileContent = Encoding.ASCII.GetBytes("Testing");
            var fileName = "test.txt";
            var message = "message";

            //Act
            _service.SendMessageWithAttachment(to, cc, subject, fileContent, fileName, message);

            //Assets
        }

        [Fact]
        public void SendMessage_ShouldPassOptionalTemplateName_ToMessageMethods()
        {
            //Arrange
            var to = "tester@test.com";
            var cc = new List<string> { "Person1@test.com" };
            var subject = "test";
            var message = "message";
            var requestedTemplate = "Test";

            //Act
            _service.SendMessage(to, cc, subject, message, requestedTemplate);

            //Assets
        }

        [Fact]
        public void SendMessageWithAttachment_ShouldPassOptionalTemplateName_ToMessageMethods()
        {
            //Arrange
            var to = "tester@test.com";
            var cc = new List<string> { "Person1@test.com" };
            var subject = "test";
            var fileContent = Encoding.ASCII.GetBytes("Testing");
            var fileName = "test.txt";
            var message = "message";
            var requestedTemplate = "Test";

            //Act
            _service.SendMessageWithAttachment(to, cc, subject, fileContent, fileName, message, requestedTemplate);

            //Assets
        }
    }
}
