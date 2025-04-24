using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Misc.FreeProduct.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreeProduct.Services
{
    public interface IFreeProductManager
    {
        Task ApplyDeleteDiscountToCart(ShoppingCartItem item);
        Task ApplyDiscountToCart(ShoppingCartItem item);
        Task DeleteAsync(FreeProductDiscount freeProductRequiremeneModel);
        Task<IList<Discount>> DiscountList();
        Task<IList<FreeProductRequirementModel>> GetAllFreeProductDiscountList();
        Task InsertAsync(FreeProductDiscount freeProductRequiremeneModel);
        Task InsertUpdateFreeProductDiscount(FreeProductRequirementModel freeProductRequiremeneModel);
        Task<IList<Product>> ProducrList();
    }
}