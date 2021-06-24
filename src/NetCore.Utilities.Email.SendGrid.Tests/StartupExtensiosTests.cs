using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace ICG.NetCore.Utilities.Email.SendGrid.Tests
{
    public class StartupExtensiosTests
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
            Assert.Equal("test.SendGrid.com", values.Server);
            Assert.Equal(527, values.Port);
            Assert.True(values.UseSsl);
            Assert.Equal("MySender", values.SenderUsername);
            Assert.Equal("Password", values.SenderPassword);
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
            collection.AddSingleton(new Mock<Microsoft.Extensions.Hosting.IHostingEnvironment>().Object);
            collection.AddSingleton(new Mock<IHostingEnvironment>().Object);
            collection.UseIcgNetCoreUtilitiesEmailSendGrid(configuration);
            var services = collection.BuildServiceProvider();

            //Act
            var result = services.GetService<ISendGridService>();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<SendGridService>(result);
        }
    }
}