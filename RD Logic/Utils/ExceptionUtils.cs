using System;

namespace ManageInfo_Core
{
    public static class ExceptionUtils
    {
        public static string GetMessage(Exception exception)
        {
            string message = "";

            message += $"EXCEPTION!{Environment.NewLine}";
            message += $"Message: {exception.Message}{Environment.NewLine}";
            message += $"Method: {exception.TargetSite}{Environment.NewLine}";

            return message;
        }
    }
}