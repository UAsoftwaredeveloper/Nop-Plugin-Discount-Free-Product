using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.Sms.Models;
using Nop.Services.Affiliates;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Payments;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Sms.Services
{
    public class SmsConfigurationManager : ISmsConfigurationManager
    {
        #region Fields

        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerService _customerService;
        private readonly ISmsSender _smsSender;
        private readonly IStoreContext _storeContext;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IAffiliateService _affiliateService;
        private readonly IAddressService _addressService;
        private string authkey = string.Empty;
        private string senderId = string.Empty;
        private string messageid = string.Empty;
        private string content_type = string.Empty;
        private string variableValues = string.Empty;
        private string Mobiles = string.Empty;
        private string msgtostorenumber = string.Empty;
        private string storemessageId = string.Empty;
        private string storemessagevariables = string.Empty;
        private string AffiliateNumber = string.Empty;
        private readonly ISettingService _settingService;
        #endregion

        #region Ctor
        public SmsConfigurationManager(ICustomerService customerService,
            IGenericAttributeService genericAttributeService,
            ISmsSender smsSender,
            IStoreContext storeContext,
            IAffiliateService affiliateService,
            IAddressService addressService, ISettingService settingService, IPaymentPluginManager paymentPluginManager)
        {
            _settingService = settingService;
            _customerService = customerService;
            _genericAttributeService = genericAttributeService;
            _storeContext = storeContext;
            _affiliateService = affiliateService;
            _addressService = addressService;
            _smsSender = smsSender;
            _paymentPluginManager = paymentPluginManager;

        }

        #endregion
        #region Methods
        public async Task HandleOrderPlacedEventSMSAsync(Order order)
        {
            //whether marketing automation is enabled
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };

                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, NopCustomerDefaults.PhoneAttribute);
                var customerName = await _genericAttributeService.GetAttributeAsync<string>(
                    customer, NopCustomerDefaults.FirstNameAttribute);
                string storeNumber = _storeContext.GetCurrentStore().CompanyPhoneNumber.ToString();
                Mobiles = customerphone;
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    //variableValues= await _genericAttributeService.GetAttributeAsync<string>(
                    //customer, NopCustomerDefaults.PhoneAttribute);
                    variableValues += customerName + "|" + order.Id;

                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        messageid = "131773";
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }
                    storemessagevariables += order.Id;
                    storemessageId = "123598";
                    isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: storemessageId, sender_id: senderId, route: "dlt", storeNumber, storemessagevariables);


                }

            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: Order payment SMS error: {exception.Message}.", exception, customer);
            }
        }

        /// <summary>
        /// Handle order Cancelled event
        /// </summary>
        /// <param name="order">Order Cancelled</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderCancelledEventSMSAsync(Order order)
        {
            //whether marketing automation is enabled
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();
            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                // Check online payment completed
                var pm = await _paymentPluginManager.LoadPluginBySystemNameAsync(order.PaymentMethodSystemName);
                var isOrderValid = true;
                if (pm != null && pm.PluginDescriptor.FriendlyName.Contains("Netbanking, Credit/Debit card"))
                {
                    if (order.PaymentStatus != Core.Domain.Payments.PaymentStatus.Pending)
                    {
                        isOrderValid = true;
                    }
                    else
                    {
                        isOrderValid = false;
                    }

                }
                if (isOrderValid)
                {
                    //Loading Sms Api Setting
                    SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                    {
                        AuthKey = smsSettings.ApiUrl,
                        Authorization = smsSettings.Authorization,
                        ContentType = smsSettings.ContentType,
                        Enable = smsSettings.Enable,
                        route = smsSettings.route,
                        Method = smsSettings.Method,
                        senderId = smsSettings.sender

                    };
                    //loading sms template details
                    bool isSmsSent = false;
                    var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                    Mobiles += customerphone;
                    if (smsConfigurationModel != null)
                    {
                        authkey = smsConfigurationModel.Authorization;
                        senderId = smsConfigurationModel.senderId;
                        variableValues = await _genericAttributeService.GetAttributeAsync<string>(
                        customer, NopCustomerDefaults.FirstNameAttribute);
                        variableValues += "|" + order.Id;
                        messageid = "131775";

                    }
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }
                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: Order Cancelled SMS error: {exception.Message}.", exception, customer);
            }
        }

        /// <summary>
        /// Handle order Completed event
        /// </summary>
        /// <param name="order">Order Completed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderCompletedEventSMSAsync(Order order)
        {
            //whether marketing automation is enabled
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    //variableValues = await _genericAttributeService.GetAttributeAsync<string>(
                    //customer, NopCustomerDefaults.FirstNameAttribute);
                    variableValues += order.Id + "| is now completed";
                    messageid = "123598";

                }
                if (!string.IsNullOrEmpty(Mobiles))
                {
                    isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: Order Completed SMS error: {exception.Message}.", exception, customer);
            }
        }
        /// <summary>
        /// Handle order Refunded event
        /// </summary>
        /// <param name="order">Order Refunded</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderRefundedEventSMSAsync(Order order)
        {
            //whether marketing automation is enabled
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

            if (order is null)
                throw new ArgumentNullException(nameof(order));

            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                //check affiliates
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    variableValues += order.Id + "|" + order.CreatedOnUtc;
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        messageid = "130899";

                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }

                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: Order Refunded SMS error: {exception.Message}.", exception, customer);
            }
        }
        /// <summary>
        /// Handle order status changed event
        /// </summary>
        /// <param name="order">Order status changed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleOrderStatusEventSMSAsync(Order order)
        {
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

            //whether marketing automation is enabled

            if (order is null)
                throw new ArgumentNullException(nameof(order));
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                //check affiliates
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    variableValues += order.Id + "|" + order.CreatedOnUtc;
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        if (order.OrderStatus == OrderStatus.Processing)
                            messageid = "130900";
                        else if (order.OrderStatus == OrderStatus.Complete)
                            messageid = "130894";
                        else if (order.OrderStatus == OrderStatus.Pending)
                            messageid = "130901";
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }

                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: OrderStatusEvent SMS error: {exception.Message}.", exception, customer);
            }
        }
        /// <summary>
        /// Handle Return Request event
        /// </summary>
        /// <param name="ReturnRequestModel">Return Request changed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleReturnRequestEventSMSAsync(ReturnRequest order)
        {
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

            //whether marketing automation is enabled

            if (order is null)
                throw new ArgumentNullException(nameof(order));
            var customer = await _customerService.GetCustomerByIdAsync(order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                //check affiliates
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    variableValues += await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute) + "|" + order.Id;
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        messageid = "131774";
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }

                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: OrderStatusEvent SMS error: {exception.Message}.", exception, customer);
            }
        }
        /// <summary>
        /// Handle Return Request event
        /// </summary>
        /// <param name="ReturnRequestModel">Return Request changed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleReturnRequestEventSMSAsync(OrderRefundedEvent order)
        {
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();

            //whether marketing automation is enabled

            if (order is null)
                throw new ArgumentNullException(nameof(order));
            var customer = await _customerService.GetCustomerByIdAsync(order.Order.CustomerId);

            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                //check affiliates
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    variableValues += await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute) + "|" + order.Order.Id;
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        messageid = "131774";
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }

                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: OrderStatusEvent SMS error: {exception.Message}.", exception, customer);
            }
        }
        /// <summary>
        /// Handle order status changed event
        /// </summary>
        /// <param name="customer">Order status changed</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleCustomerRegisterationEventSMSAsync(Customer customer)
        {
            var smsSettings = await _settingService.LoadSettingAsync<MySmsSettings>();
            //whether marketing automation is enabled
            try
            {
                //Loading Sms Api Setting
                SmsConfigurationModel smsConfigurationModel = new SmsConfigurationModel
                {
                    AuthKey = smsSettings.ApiUrl,
                    Authorization = smsSettings.Authorization,
                    ContentType = smsSettings.ContentType,
                    Enable = smsSettings.Enable,
                    route = smsSettings.route,
                    Method = smsSettings.Method,
                    senderId = smsSettings.sender

                };
                //loading sms template details
                bool isSmsSent = false;
                var customerphone = await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute);
                Mobiles += customerphone;
                //check affiliates
                if (smsConfigurationModel != null)
                {
                    authkey = smsConfigurationModel.Authorization;
                    senderId = smsConfigurationModel.senderId;
                    variableValues += await _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute);
                    if (!string.IsNullOrEmpty(Mobiles))
                    {
                        messageid = "131777";
                        isSmsSent = _smsSender.SendSms(authorization: authkey, content_type: content_type, Message: messageid, sender_id: senderId, route: "dlt", Mobiles, variableValues);
                    }

                }
            }
            catch (Exception exception)
            {
                //log full error
                throw new NopException($"SMSApi: CustomerRegistration Event SMS error: {exception.Message}.", exception, customer);
            }
        }
        public decimal GetSmswallet()
        {
            try
            {
                var client = new RestClient("https://www.fast2sms.com/dev/wallet");
                var request = new RestRequest(Method.GET);
                request.AddParameter("authorization", "Bg0MmxKWdRz286OChVvy4pcJaNwj713otkqGneT5YDiHuUZQbFBMWPgbel9kXTKZ1yju50QtcEdL836f");
                IRestResponse response = client.Execute(request);
                var customerDto = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content);


                var wallet = customerDto.Where(x => x.Key == "wallet").FirstOrDefault().Value;
                if (!string.IsNullOrEmpty(wallet))
                    return Convert.ToDecimal(wallet);
                else
                    return 0;

            }
            catch (Exception ex)
            {
                throw new NopException($"Fast2Sms wallet api Exception", ex);
            }

        }

        #endregion

    }

}
