using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nop.Web.Areas.Admin.Models.ShippingPartner
{
    public  class PikkrrResponse
    {
        public  bool Success { get; set; }
        public  string tracking_id { get; set; }
        public  string order_id { get; set; }
        public  int order_pk { get; set; }
        public  string manifest_link { get; set; }
        public  string routing_code { get; set; }
        public  string client_order_id { get; set; }
        public  string courier { get; set; }
        public  string dispatch_mode { get; set; }
        public string err { get; set; }
        public string udaan_parent_shipment_id { get; set; }
        public  object child_waybill_list { get; set; }
    }
}
