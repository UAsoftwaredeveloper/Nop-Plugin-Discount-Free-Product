using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Plugin.Misc.Sms;
using Nop.Plugin.Misc.Sms.Models;
using Nop.Plugin.Misc.Sms.Services;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Task = System.Threading.Tasks.Task;

namespace Nop.Plugin.Misc.MySms
{
    /// <summary>
    /// Represents the Sendinblue plugin
    /// </summary>
    public class MySmsPlugin : BasePlugin, IMiscPlugin
    {
        #region Fields



        private readonly ILocalizationService _localizationService;


        private readonly ISettingService _settingService;

        private readonly IWebHelper _webHelper;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public MySmsPlugin(
            ILocalizationService localizationService,
            WidgetSettings widgetSettings, IWebHelper webHelper, ISettingService settingService)
        {
            _settingService = settingService;
            _webHelper = webHelper;
            _localizationService = localizationService;

            _widgetSettings = widgetSettings;
        }

        #endregion

        #region Methods
       
        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Home/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public override async Task InstallAsync()
        {
            //settings         
            await _settingService.SaveSettingAsync(new MySmsSettings(),0);
            await _settingService.SaveSettingAsync(new MySmsSettings
            {
                auth_token= "Ente Auth Token",
                Enable =false,
               
            }); ;
            //await _settingService.SaveSettingAsync(new SmsConfigurationManager());
            _widgetSettings.ActiveWidgetSystemNames.Add(PickrrDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);


            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.ApiUrl"] = "Account info",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.ApiUrl.Hint"] = "Display account information.",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Authorization"] = "AuthKey",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Authorization.Hint"] = "Authorization Key",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Sender"] = "Sender",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Sender.Hint"] = "Sender Id",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Route"] = "Route",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Route.Hint"] = "Ex. DLT",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Enable"] = "Enable",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Enable.Hint"] = "Activate",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Method"] = "Method",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.Method.Hint"] = "Method Type Get or Post.",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.ContentType"] = "Content type",
                ["Misc.MySms.Admin.SmsConfigurationModel.Fields.ContentType.Hint"] = "format JSON/form-encode",

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
            await _settingService.DeleteSettingAsync<MySmsSettings>();
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(PickrrDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(PickrrDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }



            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Misc.Sms");

            await base.UninstallAsync();
        }

        #endregion

    }
}
