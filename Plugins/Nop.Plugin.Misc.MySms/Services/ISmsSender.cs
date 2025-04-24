namespace Nop.Plugin.Misc.Sms.Services
{
    public interface ISmsSender
    {
        bool SendSms(string authorization, string content_type, string Message, string sender_id, string route, string numbers = "", string varibleValues = "");
    }
}