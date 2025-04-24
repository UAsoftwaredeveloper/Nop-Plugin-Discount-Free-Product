using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.FreeProduct.Models
{
    public partial class FreeProductDiscount:BaseEntity
    {
        public int FreeQuantity { get; set;  }
        public int ProductId { get; set; }
        public int DiscountId { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool Published { get; set; }
    }
}
