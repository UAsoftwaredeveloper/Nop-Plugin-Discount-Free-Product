using Nop.Core;

namespace Nop.Plugin.Misc.MySms
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class MySmsDefaults
    {
      
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Misc.MySms";

        /// <summary>
        /// Gets a plugin partner name
        /// </summary>
        public static string PartnerName => "NOPCOMMERCE";

        /// <summary>
        /// Gets a user agent used to request Sendinblue services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CURRENT_VERSION}";



    }
}