namespace Nop.Plugin.Misc.Sms.Models
{
    public class CommonViewModel
    {
        public CommonViewModel()
        {
            SmsConfigurationModel = new SmsConfigurationModel();

        }
        public SmsConfigurationModel SmsConfigurationModel { get; set; }
    }
}
