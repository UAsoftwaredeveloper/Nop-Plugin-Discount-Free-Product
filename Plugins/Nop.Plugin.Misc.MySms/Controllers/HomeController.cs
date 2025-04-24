using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Sms.Models;
using Nop.Plugin.Misc.Sms.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Sms.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class HomeController : BasePluginController
    {
        #region Fields
        private readonly ISmsConfigurationManager _smsConfigurationManager;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        //private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        #endregion
        #region ctor
        public HomeController(ISmsConfigurationManager smsConfigurationManager,
            INotificationService notificationService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _storeContext = storeContext;
           // _storeService = storeService;
            _smsConfigurationManager = smsConfigurationManager;
            _notificationService = notificationService;
            _localizationService = localizationService;
            _settingService = settingService;
        }
        #endregion
        #region Utilities

        /// <summary>
        /// Prepare SendinblueModel
        /// </summary>
        /// <param name="model">Model</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        private async Task PrepareModelAsync(SmsConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var sendinblueSettings = await _settingService.LoadSettingAsync<MySmsSettings>(storeId);

            //whether plugin is configured
            if (string.IsNullOrEmpty(sendinblueSettings.ApiUrl))
                return;

            //prepare common properties
            model.ApiUrl = sendinblueSettings.ApiUrl;
            model.Authorization = sendinblueSettings.Authorization;
            model.ContentType = sendinblueSettings.ContentType;
            model.Enable = sendinblueSettings.Enable;
            model.Method = sendinblueSettings.Method;
            model.senderId = sendinblueSettings.sender;
            model.route = sendinblueSettings.route;



        }
        #endregion

        #region Methods
        // GET: SmsController
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {

            SmsConfigurationModel model=new SmsConfigurationModel();

            await PrepareModelAsync(model);
            CommonViewModel commonViewModel = new CommonViewModel();
            commonViewModel.SmsConfigurationModel = model;
                return View("~/Plugins/Misc.MySms/Views/Configure.cshtml", commonViewModel);

        }

        // POST: HomeController1/Create
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")][AuthorizeAdmin]
        
        public async Task<IActionResult> Create(SmsConfigurationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return await Configure();

               
                //set API key
                var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

                smsSettings.ApiUrl = model.ApiUrl;
                
                smsSettings.Authorization = model.Authorization;
                smsSettings.ContentType = model.ContentType;
                smsSettings.Enable = model.Enable;
                smsSettings.Method = model.Method;
                smsSettings.route = model.route;
                smsSettings.sender = model.senderId;
                await _settingService.SaveSettingAsync(smsSettings);
                await _settingService.ClearCacheAsync();

                _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

                return await Configure();
            }
            catch
            {
                return await Configure();
            }
        }
        #endregion

    }
}
