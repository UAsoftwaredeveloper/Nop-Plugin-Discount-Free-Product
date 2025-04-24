using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Misc.Sms.Models
{
    public record SmsConfigurationModel
    {
        [DataType(DataType.Url)]
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.ApiUrl")]
        public string ApiUrl { get; set; }
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.Authorization")]
        public string Authorization { get; set; }

        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.Sender")]
        public string senderId { get; set; }
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.Route")]
        public string route { get; set; }
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.Enable")]

        public bool Enable { get; set; }
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.ContentType")]
        public string ContentType { get; set; }
        [NopResourceDisplayName("Misc.Sms.Admin.SmsConfigurationModel.Fields.Method")]

        public string Method { get; set; }
    }
}
