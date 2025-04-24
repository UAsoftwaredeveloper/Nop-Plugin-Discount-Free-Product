using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Messages;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Stores;
using Nop.Core.Events;
using Nop.Services.Common;
using Nop.Services.Events;
using Nop.Services.Messages;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Sms.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer :

        IConsumer<OrderPlacedEvent>,
        IConsumer<OrderCancelledEvent>,
        IConsumer<OrderRefundedEvent>,
        IConsumer<CustomerActivatedEvent>,
        IConsumer<ReturnRequest>

    {
        #region Fields

        private readonly SmsConfigurationManager _smsManager;
        private readonly IGenericAttributeService _genericAttributeService;

        #endregion

        #region Ctor

        public EventConsumer(SmsConfigurationManager smsManager,
            IGenericAttributeService genericAttributeService)
        {
            _genericAttributeService = genericAttributeService;
            _smsManager = smsManager;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle the order paid event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
        {
            //handle event
            await _smsManager.HandleOrderCancelledEventSMSAsync(eventMessage.Order);
        }
        public async Task HandleEventAsync(OrderRefundedEvent eventMessage)
        {
            //handle event
            await _smsManager.HandleOrderRefundedEventSMSAsync(eventMessage.Order);
        }
        public async Task HandleEventAsync(CustomerActivatedEvent customerRegisteredEvent)
        {
            //handle event
            await _smsManager.HandleCustomerRegisterationEventSMSAsync(customerRegisteredEvent.Customer);
        }

        /// <summary>
        /// Handle the order placed event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //handle event
            await _smsManager.HandleOrderPlacedEventSMSAsync(eventMessage.Order);
        }
        public async Task HandleEventAsync(ReturnRequest eventMessage)
        {
            //handle event
            await _smsManager.HandleReturnRequestEventSMSAsync(eventMessage);
        }


        #endregion
    }
}