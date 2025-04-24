using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Sms
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class MySmsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the API key
        /// </summary>
        public string ApiUrl { get; set; }
        public string Authorization { get; set; }

        /// <summary>
        /// Gets or sets the identifier of list to synchronize contacts
        /// </summary>
        public string sender { get; set; }

        /// <summary>
        /// Gets or sets the identifier of unsubscribe event webhook
        /// </summary>
        public string route { get; set; }        

        /// <summary>
        /// Gets or sets the SMTP key (Master password)
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SMTP (for transactional emails)
        /// </summary>
        public string Method { get; set; }


        /// <summary>
        /// Gets or sets the identifier of email account (for transactional emails)
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use SMS notifications
        /// </summary>
        

    }
}