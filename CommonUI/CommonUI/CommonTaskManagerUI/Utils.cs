using System;
namespace com.ivp.secm
{

    public enum SMDateTimeOptions
    {
        DATE,
        TIME,
        DATETIME
    }

    internal static class SMUiUtils
    {
        public static string ConvertDateTimeFormatToCustomDatePickerFormat(string format, SMDateTimeOptions option)
        {
            if (format.IndexOf("yyyy") > -1)
                format = format.Replace("yyyy", "Y");
            if (format.IndexOf("YYYY") > -1)
                format = format.Replace("YYYY", "Y");
            if (format.IndexOf("yy") > -1)
                format = format.Replace("yy", "Y");
            if (format.IndexOf("YY") > -1)
                format = format.Replace("YY", "Y");
            if (format.IndexOf("mm") > -1)
                format = format.Replace("mm", "i");
            if (format.IndexOf("m") > -1)
                format = format.Replace("m", "i");
            if (format.IndexOf("MM") > -1)
                format = format.Replace("MM", "m");
            if (format.IndexOf("M") > -1)
                format = format.Replace("M", "m");
            if (format.IndexOf("ss") > -1)
                format = format.Replace("ss", "s");
            if (format.IndexOf("tt") > -1)
            {
                if (option.ToString().Equals("time", StringComparison.OrdinalIgnoreCase))
                    format = format.Replace("tt", "");
                else
                    format = format.Replace("tt", "A");
            }
            if (option.ToString().Equals("time", StringComparison.OrdinalIgnoreCase) && format.IndexOf("h") > -1)
                format = format.Replace("h", "H");
            if (format.IndexOf("hh") > -1)
                format = format.Replace("hh", "h");
            if (format.IndexOf("HH") > -1)
                format = format.Replace("HH", "H");
            if (format.IndexOf("dd") > -1)
                format = format.Replace("dd", "d");
            return format.Trim();
        }
    }
}