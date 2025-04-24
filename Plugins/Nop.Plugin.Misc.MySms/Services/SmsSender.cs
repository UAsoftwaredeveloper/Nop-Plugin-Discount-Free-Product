using Nop.Core;
using RestSharp;
using System;
using System.Threading.Tasks;

namespace Nop.Plugin.Misc.Sms.Services
{
    public class SmsSender : ISmsSender
    {
        Uri ApiUrl = new Uri("https://www.fast2sms.com/dev/bulkV2");
        public SmsSender(string ApiUri = "")
        {
            if (!string.IsNullOrEmpty(ApiUri))
            {
                ApiUrl = new Uri(ApiUri);
            }

        }
        public bool SendSms(string authorization, string content_type, string Message, string sender_id, string route, string numbers = "", string varibleValues = "")
        {
            try
            {
                var client = new RestClient("https://www.fast2sms.com/dev/bulkV2");
                var request = new RestRequest(Method.POST);
                request.AddHeader("content-type", content_type);
                request.AddHeader("authorization", authorization);
                request.AddParameter("sender_id", sender_id);
                request.AddParameter("message", Message);
                request.AddParameter("variables_values", varibleValues);
                request.AddParameter("route", route);
                request.AddParameter("numbers", numbers);
                IRestResponse response = client.Execute(request);
                if (response.IsSuccessful)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw new NopException($"Fast2Sms Exception", ex);
            }

        }
    
    }
}
