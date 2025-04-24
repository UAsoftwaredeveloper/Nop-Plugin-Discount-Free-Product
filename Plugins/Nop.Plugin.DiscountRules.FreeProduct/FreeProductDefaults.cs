using Nop.Core;

namespace Nop.Plugin.Misc.FreeProduct
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class FreeProductDefaults
    {
      
        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Misc.FreeProducts";

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