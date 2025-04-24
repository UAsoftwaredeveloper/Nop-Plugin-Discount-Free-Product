using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.FreeProduct.Models
{
    public record FreeProductRequirementModel
    {
        [Key]
        public int Id { get; set; }
        [NopResourceDisplayName("Misc.FreeProducts.Fields.ProductId")]
        public int ProductId { get; set; }
        [NopResourceDisplayName("Misc.FreeProducts.Fields.FreeQuantity")]

        public int FreeQuantity { get; set; }
        [NopResourceDisplayName("Misc.FreeProducts.Fields.DiscountId")]
        public int DiscountId { get; set; }
        [NopResourceDisplayName("Misc.FreeProducts.Fields.From")]
        [UIHint("DateTimeNullable")]
        public Nullable<DateTime> From { get; set; }
        [NopResourceDisplayName("Misc.FreeProducts.Fields.To")]
        [UIHint("DateTimeNullable")]
        public Nullable<DateTime> To { get; set; }

        [NopResourceDisplayName("Misc.FreeProducts.Fields.Published")]
        public bool Published { get; set; }
    
    }
}