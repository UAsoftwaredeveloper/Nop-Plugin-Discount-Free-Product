using System.Threading.Tasks;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Core.Events;
using Nop.Plugin.Misc.FreeProduct.Services;
using Nop.Services.Configuration;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.FreeProduct.Infrastructure
{
    /// <summary>
    /// Discount requirement rule event consumer (used for removing unused settings)
    /// </summary>
    public partial class EventConsumer :IConsumer<EntityUpdatedEvent<ShoppingCartItem>>,IConsumer<EntityInsertedEvent<ShoppingCartItem>>,IConsumer<EntityDeletedEvent<ShoppingCartItem>>
    {
        #region Fields
        
        private readonly IFreeProductManager _freeproductManager;

        #endregion

        #region Ctor

        public EventConsumer(IFreeProductManager freeProductManager)
        {
            _freeproductManager = freeProductManager;
        }

        #endregion

        #region Methods

        public async Task HandleEventAsync(EntityUpdatedEvent<ShoppingCartItem> eventMessage)
        {
            await _freeproductManager.ApplyDiscountToCart(eventMessage.Entity);
        }
        public async Task HandleEventAsync(EntityInsertedEvent<ShoppingCartItem> eventMessage)
        {
            await _freeproductManager.ApplyDiscountToCart(eventMessage.Entity);
        }
        public async Task HandleEventAsync(EntityDeletedEvent<ShoppingCartItem> eventMessage)
        {
            await _freeproductManager.ApplyDeleteDiscountToCart(eventMessage.Entity);
        }

        #endregion
    }
}