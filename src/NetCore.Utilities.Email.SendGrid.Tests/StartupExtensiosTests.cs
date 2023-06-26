using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace ICG.NetCore.Utilities.Email.SendGrid.Tests
{
    public class StartupExtensionsTests
    {
        [Fact]
        public void Configuration_ShouldMapAllValues()
        {
            //Arrange
            var collection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            collection.UseIcgNetCoreUtilitiesEmailSendGrid(configuration);
            var services = collection.BuildServiceProvider();

            //Act
            var myConfig = services.GetService<IOptions<SendGridServiceOptions>>();

            //Assert
            Assert.NotNull(myConfig);
            var values = myConfig.Value;
            Assert.Equal("test@test.com", values.AdminEmail);
            Assert.Equal("TestKey", values.SendGridApiKey);
            Assert.Single(values.AdditionalApiKeys);
            var specialKeyValue = values.AdditionalApiKeys["SpecialSender"];
            Assert.Equal("SpecialKey", specialKeyValue);

            Assert.True(values.AlwaysTemplateEmails);
            Assert.True(values.AddEnvironmentSuffix);
        }
        

        [Fact]
        public void ServiceCollection_ShouldRegisterSendGridService()
        {
            //Arrange
            var collection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            collection.AddSingleton(new Mock<IHostEnvironment> ().Object);
            collection.UseIcgNetCoreUtilitiesEmailSendGrid(configuration);
            collection.AddLogging();
            var services = collection.BuildServiceProvider();

            //Act
            var result = services.GetService<IEmailService>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<SendGridService>(result);
        }


        [Fact]
        public void ServiceCollection_ShouldRegisterSendMessageBuilder()
        {
            //Arrange
            var collection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            collection.AddSingleton(new Mock<IHostEnvironment>().Object);
            collection.AddLogging();
            collection.UseIcgNetCoreUtilitiesEmailSendGrid(configuration);
            var services = collection.BuildServiceProvider();

            //Act
            var result = services.GetService<ISendGridMessageBuilder>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<SendGridMessageBuilder>(result);
        }

        [Fact]
        public void ServiceCollection_ShouldRegisterSendGridSender()
        {
            //Arrange
            var collection = new ServiceCollection();
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            collection.AddSingleton(new Mock<IHostEnvironment>().Object);
            collection.UseIcgNetCoreUtilitiesEmailSendGrid(configuration);
            var services = collection.BuildServiceProvider();

            //Act
            var result = services.GetService<ISendGridSender>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<SendGridSender>(result);
        }
    }
}