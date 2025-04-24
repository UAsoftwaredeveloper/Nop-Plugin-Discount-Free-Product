using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.Sms
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class MySmsSettings : ISettings
    {
        /// <summary>
        /// Gets or sets the API Auth Token
        /// </summary>
       
        public string auth_token { get; set; }      
        

        /// <summary>
        /// Gets or sets the identifier of email account (for transactional emails)
        /// </summary>
        public bool Enable { get; set; }

    }
}