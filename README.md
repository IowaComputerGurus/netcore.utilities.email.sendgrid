# ICG.NetCore.Utilities.Email.SendGrid ![](https://img.shields.io/github/license/iowacomputergurus/netcore.utilities.email.SendGrid.svg)

![Build Status](https://github.com/IowaComputerGurus/netcore.utilities.email/actions/workflows/ci-build.yml/badge.svg)

![](https://img.shields.io/nuget/v/icg.netcore.utilities.email.sendgrid.svg) ![](https://img.shields.io/nuget/dt/icg.netcore.utilities.email.sendgrid.svg)

This library provides an easy to use implementation of SendGrid based email delivery.  This abstraction with proper interfaces allows email implementation inside of your project with little effort and easy to manage integration, and boasts features such as automatic environment name appending as well as robust email templates.

This package depends on the ICG.NetCore.Utilities.Email project for template implementation

## Breaking Changes

Version 7.0 has a breaking change transitioning to Async for all methods!

## Dependencies
The following additional NuGet packages are installed with this extension.

* [SendGrid](https://www.nuget.org/packages/SendGrid/) - For email delivery
* [ICG NET Core Utilities Email](https://github.com/IowaComputerGurus/netcore.utilities.email) - For Email Template Configuration

## Usage

### Installation
Standard installation via HuGet Package Manager
``` powershell
Install-Package ICG.NetCore.Utilities.Email.SendGrid
```

### Setup & Configuration Options
To setup the needed dependency injection items for this library, add the following line in your DI setup.
``` csharp
services.UseIcgNetCoreUtilitiesEmailSendGrid();
```

Additionally you must specify the needed configuration elements within your AppSettings.json file

``` json
  "SendGridServiceOptions": {
    "AdminEmail": "test@test.com",
    "AdminName": "John Smith",
    "SendGridApiKey": "YourKey",
    "AdditionalApiKeys": { "SpecialSender": "SpecialKey" }
    "AlwaysTemplateEmails": true,
    "AddEnvironmentSuffix": true
  },
  "EmailTemplateSettings": {
    "DefaultTemplatePath": "Template.html",
    "AdditionalTemplates": { "SpecialTemplate": "File.html" }
  }
```


| Setting | Description |
| --- | --- |
| AdminEmail | This is the email address used as the "from" address and also for any usage of the "SendToAdministrator" option |
| AdminName | If specified this is the name that will be used for the "From" address on all outbound emails |
| SendGridApiKey | The API Key to use for default sending of email addresses |
| AdditionalApiKeys | These are name/value pairs of additional API keys that could be used for sending emails.  Totally optional |
| AlwaysTemplateEmails | If selected ALL emails sent will be templated, by default using the "DefaultTemplate" as configured |
| AddEnvironmentSuffix | If selected, all outbound emails sent from non-production addresses will have the environment name added to the end of the subject |
| DefaultTemplatePath | The path, relative to the application root, where the default HTML template can be found for emails |
| AdditionalTemplates | These are name/value pairs of additional templates and totally optional |


### Usage

Usage is primarly completed by injecting the IEmailService interface to your respective project, one injected emails can be sent with a single line of code. 

``` csharp
_sendGridService.SendEmail("recipient@me.com", "My Subject", "<p>Hello!</p>");
```
Inline documentation exists for all API methods. We will continue to add more to this documentation in the future (PR's Welcome)

## Related Projects

ICG has a number of other related projects as well

* [AspNetCore.Utilities](https://www.github.com/iowacomputergurus/aspnetcore.utilities)
* [NetCore.Utilities.Email.Smtp](https://www.github.com/iowacomputergurus/netcore.utilities.Email.Smtp)
* [NetCore.Utilities.Spreadsheet](https://www.github.com/iowacomputergurus/netcore.utilities.spreadsheet)
* [NetCore.Utilities.UnitTesting](https://www.github.com/iowacomputergurus/netcore.utilities.unittesting)
