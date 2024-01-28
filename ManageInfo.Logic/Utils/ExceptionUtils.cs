using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageInfo_Logic
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
