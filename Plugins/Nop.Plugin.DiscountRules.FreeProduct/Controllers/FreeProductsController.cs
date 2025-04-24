using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Plugin.Misc.FreeProduct.Models;
using Nop.Plugin.Misc.FreeProduct.Services;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.FreeProduct.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class FreeProductsController : BasePluginController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly INotificationService _notificationService;
        private readonly IFreeProductManager _freeProductManager;

        #endregion

        #region Ctor

        public FreeProductsController(ICustomerService customerService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService,
            IFreeProductManager freeProductManager,
            INotificationService notificationService)
        {
            _customerService = customerService;
            _discountService = discountService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
            _freeProductManager = freeProductManager;
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var modelList = await _freeProductManager.GetAllFreeProductDiscountList();
            return View("~/Plugins/Misc.FreeProducts/Views/Index.cshtml", modelList);            

        }
        public async Task<IActionResult> Index()
        {
            var modelList = await _freeProductManager.GetAllFreeProductDiscountList();
            return View("~/Plugins/Misc.FreeProducts/Views/Index.cshtml", modelList);
        }

        public IActionResult Add()
        {

            var model = new FreeProductRequirementModel();
            if (model.From == null || model.To == null)
            {
                model.From = System.DateTime.Now;
                model.To = System.DateTime.MaxValue;
            }
            return View("~/Plugins/Misc.FreeProducts/Views/AddEdit.cshtml", model);
        }
        public IActionResult Edit(int id)
        {
            var model = new FreeProductRequirementModel();
            model = _freeProductManager.GetAllFreeProductDiscountList().Result.Where(x => x.Id == id).FirstOrDefault();
            return View("~/Plugins/Misc.FreeProducts/Views/AddEdit.cshtml", model);
        }
        [HttpPost]
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> save(FreeProductRequirementModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");
            
            if (ModelState.IsValid)
            {
                //load the discount
                await _freeProductManager.InsertUpdateFreeProductDiscount(model);

                _notificationService.SuccessNotification("Saved Successfully");
                return View("~/Plugins/Misc.FreeProducts/Views/AddEdit.cshtml", model);
            }
            else
            {
                _notificationService.ErrorNotification("Please Completed the Form");
                return View("~/Plugins/Misc.FreeProducts/Views/AddEdit.cshtml", model);
            }
        }
        
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<IActionResult> Delete(int id)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");
            
              //load the discount
                var data = await _freeProductManager.GetAllFreeProductDiscountList();
                var item = data.Where(x => x.Id==id).FirstOrDefault();
                var mode = new FreeProductDiscount
                {
                    Id = item.Id,
                    Published = item.Published,
                    DiscountId = item.DiscountId,
                    FreeQuantity = item.FreeQuantity,
                    From = item.From.Value,
                    ProductId = item.ProductId,
                    To = item.To.Value
                };
                await _freeProductManager.DeleteAsync(mode);

                _notificationService.SuccessNotification("Delete Successfully");
                return await Configure();
        }

        #endregion

        #region Utilities

        private IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }

        #endregion
    }
}