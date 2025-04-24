using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Data.Migrations;
using Nop.Plugin.Misc.FreeProduct;
using Nop.Plugin.Misc.FreeProduct.Models;
using Nop.Plugin.Misc.FreeProduct.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Misc.FreeProduct.Controllers
{
    /// <summary>
    /// Represents the Sendinblue plugin
    /// </summary>
    public class FreeProductPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields



        private readonly ILocalizationService _localizationService;


        private readonly ISettingService _settingService;

        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;       
        private readonly IFreeProductManager _freeProductManager;

        #endregion

        #region Ctor

        public FreeProductPlugin(
            ILocalizationService localizationService,
            WidgetSettings widgetSettings, IWebHelper webHelper, ISettingService settingService,
            
            IFreeProductManager freeProductManager)

        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;

            _widgetSettings = widgetSettings;
            
            _freeProductManager = freeProductManager;
        }

        #endregion
        #region utility
        public Task ManageSiteMapAsync(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = FreeProductDefaults.SystemName,
                Title = "Free Product Discount",
                ControllerName = "FreeProducts",
                ActionName = "Configure",
                Visible = true
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == FreeProductDefaults.SystemName);
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem);

            return Task.CompletedTask;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Prepare ConfigurationPageUrl
        /// </summary>
        /// <returns></returns>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/FreeProducts/Configure";
        }
        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings         
            //await _settingService.SaveSettingAsync(new SmsConfigurationManager());
            _widgetSettings.ActiveWidgetSystemNames.Add(FreeProductDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            FreeProductDiscount freeProductDiscount = new FreeProductDiscount()
            {
                DiscountId = 0,
                FreeQuantity = 0,
                From = System.DateTime.MinValue,
                ProductId = 0,
                Published = false,
                To = System.DateTime.MinValue
            };
            await _freeProductManager.InsertAsync(freeProductDiscount);

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Misc.FreeProducts.Fields.ProductId"] = "Select Free Product",
                ["Misc.FreeProducts.Fields.ProductId.Hint"] = "Choose a free Product by Name",
                ["Misc.FreeProducts.Fields.DiscountId"] = "Select Discount",
                ["Misc.FreeProducts.Fields.DiscountId.Hint"] = "Choose a Discount by Name for Discount Rule",
                ["Misc.FreeProducts.Fields.From"] = "Enter From Date",
                ["Misc.FreeProducts.Fields.From.Hint"] = "Select From Date",
                ["Misc.FreeProducts.Fields.To"] = "Enter To Date",
                ["Misc.FreeProducts.Fields.To.Hint"] = "Select To Date",
                ["Misc.FreeProducts.Fields.Published"] = "Published",
                ["Misc.FreeProducts.Fields.Published.Hint"] = "choose to activate",
                ["Misc.FreeProducts.Fields.FreeQuantity"] = "Quantity",
                ["Misc.FreeProducts.Fields.Published.Hint"] = "Enter Quantities",

            });
                await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task UninstallAsync()
        {

            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(FreeProductDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(FreeProductDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }



            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Misc.FreeProducts");

            await base.UninstallAsync();
        }

        #endregion

    }
}
