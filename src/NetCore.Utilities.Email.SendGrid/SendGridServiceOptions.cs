using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ICG.NetCore.Utilities.Email.SendGrid
{
    /// <summary>
    ///     Configuration options for use with the <see cref="SendGridService" />
    /// </summary>
    public class SendGridServiceOptions
    {
        /// <summary>
        ///     The email address defining the administrator contact
        /// </summary>
        [Display(Name = "Admin Email")]
        public string AdminEmail { get; set; }

        /// <summary>
        ///     An optional name for the administrative user
        /// </summary>
        [Display(Name = "Admin Name")]
        public string AdminName { get; set; }

        /// <summary>
        ///     The SendGrid API key that needs to
        /// </summary>
        public string SendGridApiKey { get; set; }

        /// <summary>
        ///     Optional additional API Keys for sending outbound emails
        /// </summary>
        public Dictionary<string, string> AdditionalApiKeys { get; set; }

        /// <summary>
        ///     If selected outbound emails will be sent with the default template unless a special template is requested
        /// </summary>
        [Display(Name = "Always Template Emails")]
        public bool AlwaysTemplateEmails { get; set; }

        /// <summary>
        ///     If selected and email sent via a non-production environment the current environment will be added as a suffix
        /// </summary>
        [Display(Name = "Add Environment Suffix")]
        public bool AddEnvironmentSuffix { get; set; }
    }
}