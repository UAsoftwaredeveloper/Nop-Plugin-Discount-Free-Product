using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.Sms.Models;
using Nop.Plugin.Misc.Sms.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Areas.Admin.Factories;
using Nop.Web.Areas.Admin.Models.Orders;
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
        private readonly IPermissionService _permissionService;
        private readonly IOrderModelFactory _orderModelFactory;

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
            if (string.IsNullOrEmpty(sendinblueSettings.auth_token))
                return;

            //prepare common properties
            model.AuthKey = sendinblueSettings.auth_token;
            
            model.Enable = sendinblueSettings.Enable;

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
                smsSettings.auth_token = model.AuthKey;
                smsSettings.Enable = model.Enable;                
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
        public virtual async Task<IActionResult> List(List<int> orderStatuses = null, List<int> paymentStatuses = null, List<int> shippingStatuses = null)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return AccessDeniedView();

            //prepare model
            var model = await _orderModelFactory.PrepareOrderSearchModelAsync(new OrderSearchModel
            {
                OrderStatusIds = orderStatuses,
                PaymentStatusIds = paymentStatuses,
                ShippingStatusIds = shippingStatuses
            });

            return View(model);
        }

        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public virtual async Task<IActionResult> OrderList(OrderSearchModel searchModel)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageOrders))
                return await AccessDeniedDataTablesJson();

            //prepare model
            var model = await _orderModelFactory.PrepareOrderListModelAsync(searchModel);

            return Json(model);
        }

        #endregion

    }
}
