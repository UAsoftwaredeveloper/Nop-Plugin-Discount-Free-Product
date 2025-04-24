

namespace Nop.Plugin.Misc.Sms.Models
{
    public class CommonViewModel
    {
        public CommonViewModel()
        {
            SmsConfigurationModel = new SmsConfigurationModel();
            OrderSearchModel = new OrderSearchModel();

        }
        public SmsConfigurationModel SmsConfigurationModel { get; set; }
        public OrderSearchModel OrderSearchModel { get; set; }
    }
}
