using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Data;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Web.Areas.Admin.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.ShippingPartner
{
    public class OrderDetails
    {
        string token = "cc1399b4d32ed5cc8e106492713faef3129306";
        public string get_dataPickerForDispatch(OrderModel order, IGenericAttributeService _genericAttributeService, Customer customer, IProductService _productService,IAddressService addressService)
        {
            DateTime today = DateTime.Now;
            DateTime Answer = today.AddDays(5);
            try
            { // call payment //allready set COd as C and prepaid is p
                var jsonObject = new JObject();
                dynamic Customers = new JObject();
                //  dynamic consignments = new JArray() as dynamic;
                dynamic consignments = new JObject();
                Customers = new JObject();
                string productname = string.Empty;
                int count = 0;
                decimal totalWeightOfItems = 0;
                decimal totalHeightOfItems = 0;
                decimal totalLenghtOfItems = 0;
                decimal totalWidthtOfItems = 0;
                foreach (var iten in order.Items)
                {
                    count++;
                    if (count > 1)
                        productname += "," + iten.ProductName;
                    else
                        productname += iten.ProductName;

                }
                consignments = new JObject();
                consignments.auth_token = token;
                consignments.item_name = productname;
                dynamic origin_details1 = new JObject();
                origin_details1 = new JObject();
                // consignments.destination_details = destination_details;
                consignments.item_list = new JArray() as dynamic;
                dynamic item_list = new JObject();
                //consignments.pieces_detail = item_list;
                item_list = new JObject();
                count = 0;
                int warehouseId = 0;
                foreach (var iten in order.Items)
                {
                    count++;
                    if (count > 1)
                        productname += "," + iten.ProductName;
                    else
                        productname += iten.ProductName;
                    var productDetails = _productService.GetProductByIdAsync(iten.ProductId).Result;
                    warehouseId = productDetails.WarehouseId;
                    totalWeightOfItems += productDetails.Weight;
                    totalLenghtOfItems += productDetails.Length;
                    totalWidthtOfItems += productDetails.Width;
                    totalHeightOfItems += productDetails.Height;

                    item_list.price = Convert.ToInt32(iten.UnitPriceInclTaxValue);
                    item_list.item_name = iten.ProductName;
                    item_list.quantity = iten.Quantity;
                    item_list.sku = "-";
                    item_list.item_tax_percentage = order.TaxRates.FirstOrDefault().Rate;
                    consignments.item_list.Add(item_list);
                }
                string paymenttype = order.PaymentStatus == PaymentStatus.Pending.ToString() ? order.OrderTotalValue.ToString() : "0";

                var warehouse = _productService.GetWarehousesByIdAsync(warehouseId).Result;
                var WareHouseAddress = addressService.GetAddressByIdAsync(warehouse.AddressId).Result;
                consignments.from_name = WareHouseAddress.Company;
                
                consignments.from_phone_number = WareHouseAddress.PhoneNumber;
                consignments.from_address = WareHouseAddress.Address1;
                consignments.from_pincode = WareHouseAddress.ZipPostalCode ;
                consignments.pickup_gstin = order.VatNumber;

                consignments.to_name = _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.FirstNameAttribute, 0).Result;
                consignments.to_email = order.CustomerEmail;
                consignments.to_phone_number = _genericAttributeService.GetAttributeAsync<string>(customer, NopCustomerDefaults.PhoneAttribute, 0).Result + "," + order.ShippingAddress.PhoneNumber;
                consignments.to_pincode = order.ShippingAddress.ZipPostalCode;
                consignments.to_address = order.ShippingAddress.Address1;

                consignments.quantity = 1;
                consignments.invoice_value = Convert.ToInt32(order.OrderTotalValue);
                consignments.cod_amount = paymenttype;
                consignments.client_order_id = order.Id;
                /*Default value*/
                consignments.item_breadth = totalWidthtOfItems > 0 ? totalWidthtOfItems : 1;
                consignments.item_length = totalLenghtOfItems > 0 ? totalLenghtOfItems : 1;
                consignments.item_height = totalHeightOfItems > 0 ? totalHeightOfItems : 1;
                consignments.item_weight = totalWeightOfItems > 0 ? totalWeightOfItems.ToString() : "0.7";
                consignments.is_reverse = false;
                consignments.invoice_number = order.Id;
                /*Total transaction*/
                consignments.total_discount = order.OrderTotalDiscountValue;
                consignments.shipping_charge = 0;
                consignments.transaction_charge = 0;
                consignments.giftwrap_charge = 0;
                string json = JsonConvert.SerializeObject(consignments, Newtonsoft.Json.Formatting.Indented);

                return json.ToString();
            }


            catch (Exception Ex)
            {
                throw Ex.GetBaseException();
            }

        }
        /// <summary>
        /// Prepare Order data for Order Cancelation
        /// </summary>
        /// <param name="trackingId"></param>
        /// <returns></returns>
        public string PrepareOrderCancellation(string trackingId)
        {
            try
            {
                var jsonObject = new JObject();
                //  dynamic consignments = new JArray() as dynamic;
                dynamic consignments = new JObject();
                consignments = new JObject();
                consignments.auth_token = token;
                consignments.tracking_id = trackingId;
                string json = JsonConvert.SerializeObject(consignments, Newtonsoft.Json.Formatting.Indented);
                return json.ToString();
            }


            catch (Exception Ex)
            {
                throw Ex.GetBaseException();
            }

        }
    }
}
