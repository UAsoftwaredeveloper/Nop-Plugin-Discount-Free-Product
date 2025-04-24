using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Orders;
using Nop.Data;
using Nop.Plugin.Misc.FreeProduct.Models;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Orders;

namespace Nop.Plugin.Misc.FreeProduct.Services
{
    public class FreeProductManager : IFreeProductManager
    {
        private readonly IDiscountService _discountServices;
        private readonly IProductService _productService;        
        private readonly ICustomerService _customerService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly IRepository<FreeProductDiscount> _freeProductDiscountService;
        private readonly IRepository<Product> _product;
        private readonly IRepository<ShoppingCartItem> _sciRepository;

        #region Ctor
        public FreeProductManager(IDiscountService discountService,
            IRepository<FreeProductDiscount> freeProductDiscountRepository,
            IProductService productService,
            ICustomerService customerService, IShoppingCartService shoppingCartService,
            IRepository<Product> product, IRepository<ShoppingCartItem> sciRepository)
            
        {
           
            _discountServices = discountService;
            _freeProductDiscountService = freeProductDiscountRepository;
            _sciRepository = sciRepository;
            _productService = productService;
            _customerService = customerService;
            _product = product;
            _shoppingCartService = shoppingCartService;
           
        }

        #endregion
        #region Utility
       
        public async Task<IList<Discount>> DiscountList()
        {
            var discounts = await _discountServices.GetAllDiscountsAsync();

            return discounts;
        }
        public async Task<IList<Product>> ProducrList()
        {
            var products = await _product.Table.Where(x=>x.Price<100).ToListAsync();
            return products;
        }

        #endregion
        #region Methods
        public async Task InsertUpdateFreeProductDiscount(FreeProductRequirementModel freeProductRequiremeneModel)
        {
            try
            {
                FreeProductDiscount freeProductDiscount = new FreeProductDiscount
                {
                    Id = freeProductRequiremeneModel.Id,
                    DiscountId = freeProductRequiremeneModel.DiscountId,
                    From = freeProductRequiremeneModel.From!=null?freeProductRequiremeneModel.From.Value:DateTime.Now,
                    ProductId = freeProductRequiremeneModel.ProductId,
                    Published = freeProductRequiremeneModel.Published,
                    To = freeProductRequiremeneModel.To!=null?freeProductRequiremeneModel.To.Value:DateTime.MaxValue
                };
                if (freeProductRequiremeneModel.Id > 0)
                    await _freeProductDiscountService.UpdateAsync(freeProductDiscount, false);
                else
                    await _freeProductDiscountService.InsertAsync(freeProductDiscount, false);

            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public async Task InsertAsync(FreeProductDiscount freeProductRequiremeneModel)
        {
            try
            {
                FreeProductDiscount freeProductDiscount = new FreeProductDiscount
                {
                    Id = freeProductRequiremeneModel.Id,
                    DiscountId = freeProductRequiremeneModel.DiscountId,
                    From = freeProductRequiremeneModel.From,
                    ProductId = freeProductRequiremeneModel.ProductId,
                    Published = freeProductRequiremeneModel.Published,
                    To = freeProductRequiremeneModel.To
                };
                if (freeProductRequiremeneModel.Id > 0)
                    await _freeProductDiscountService.InsertAsync(freeProductDiscount, false);
                else
                    await _freeProductDiscountService.UpdateAsync(freeProductDiscount, false);


            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public async Task DeleteAsync(FreeProductDiscount freeProductRequiremeneModel)
        {
            try
            {
                FreeProductDiscount freeProductDiscount = new FreeProductDiscount
                {
                    Id = freeProductRequiremeneModel.Id,
                    DiscountId = freeProductRequiremeneModel.DiscountId,
                    From = freeProductRequiremeneModel.From,
                    ProductId = freeProductRequiremeneModel.ProductId,
                    Published = freeProductRequiremeneModel.Published,
                    To = freeProductRequiremeneModel.To
                };
                if (freeProductRequiremeneModel.Id > 0)
                    await _freeProductDiscountService.DeleteAsync(freeProductDiscount, false);


            }
            catch (Exception ex)
            {
                throw ex.InnerException;
            }
        }
        public async Task<IList<FreeProductRequirementModel>> GetAllFreeProductDiscountList()
        {
            List<FreeProductRequirementModel> freeProductRequiremeneModels = new List<FreeProductRequirementModel>();
            var rez = await _freeProductDiscountService.Table.ToListAsync();
            foreach (var item in rez)
            {
                var product = await _productService.GetProductByIdAsync(item.ProductId);
                freeProductRequiremeneModels.Add(new FreeProductRequirementModel
                {
                    ProductId = item.ProductId,
                    DiscountId = item.DiscountId,
                    From = item.From,
                    Id = item.Id,
                    Published = item.Published,
                    To = item.To
                });
            }
            return freeProductRequiremeneModels;
        }
        public async Task ApplyDiscountToCart(ShoppingCartItem item)
        {
            var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
            var shoppincartListItems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, 0);
            var shoppingCartProductIds = shoppincartListItems.Select(x => x.ProductId).ToArray();
            if (shoppincartListItems.Count > 0)
            {
                List<Discount> discounts = new List<Discount>();
                decimal uniPrice = 0;
                decimal discountAmount = 0;
                (uniPrice, discountAmount, discounts) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                var freeProductDiscounts = _freeProductDiscountService.GetAll();
                int[] discountIds = discounts.Select(x => x.Id).ToArray();
                var freeProductDiscount = freeProductDiscounts.Where(x => x.Published == true && x.From < DateTime.Now && x.To >= DateTime.Now && discountIds.Contains(x.DiscountId)).FirstOrDefault();
                if (discounts.Count > 0 && discounts.Any(x => x.Id == freeProductDiscount.DiscountId))
                {
                    var product = await _productService.GetProductByIdAsync(freeProductDiscount.ProductId);
                    var customerCarts = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart,1);
                    var productsIds = customerCarts.Select(x => x.ProductId).ToArray();
                    if (!productsIds.Contains(product.Id))
                    {
                        var now = DateTime.UtcNow;
                        ShoppingCartItem shoppingCartItem = new ShoppingCartItem
                        {
                            ShoppingCartType = ShoppingCartType.ShoppingCart,
                            StoreId = 1,
                            ProductId = product.Id,
                            CustomerEnteredPrice = 0,
                            Quantity = freeProductDiscount.FreeQuantity>0?freeProductDiscount.FreeQuantity:1,
                            RentalStartDateUtc = null,
                            RentalEndDateUtc = null,
                            CreatedOnUtc = now,
                            UpdatedOnUtc = now,
                            CustomerId = customer.Id,

                        };
                        await _sciRepository.InsertAsync(shoppingCartItem,false);
                    }
                }
            }
        }
        public async Task ApplyDeleteDiscountToCart(ShoppingCartItem item)
        {
            var customer = await _customerService.GetCustomerByIdAsync(item.CustomerId);
            var shoppincartListItems = await _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart, 0);
            var shoppingCartProductIds = shoppincartListItems.Select(x => x.ProductId).ToArray();
            if (shoppincartListItems.Count > 0)
            {
                List<Discount> discounts = new List<Discount>();
                decimal uniPrice = 0;
                decimal discountAmount = 0;
                (uniPrice, discountAmount, discounts) = await _shoppingCartService.GetUnitPriceAsync(item, true);
                var freeProductDiscounts = _freeProductDiscountService.GetAll();
                int[] discountIds = discounts.Select(x => x.Id).ToArray();
                var freeProductDiscount = freeProductDiscounts.Where(x => x.Published == true && x.From < DateTime.Now && x.To >= DateTime.Now && discountIds.Contains(x.DiscountId)).FirstOrDefault();
                if (discounts.Count > 0 && !discounts.Any(x => x.Id == freeProductDiscount.DiscountId))
                {
                    var product = await _productService.GetProductByIdAsync(freeProductDiscount.ProductId);
                    product.Price = decimal.Zero;
                    var cartitems =  _shoppingCartService.GetShoppingCartAsync(customer, ShoppingCartType.ShoppingCart).Result.Where(x => x.ProductId == freeProductDiscount.ProductId).FirstOrDefault();
                    await _sciRepository.DeleteAsync(cartitems,false);
                }
            }
        }
        #endregion

    }
}
