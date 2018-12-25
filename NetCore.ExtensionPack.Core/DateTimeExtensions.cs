using System;
using System.Globalization;

namespace ExtensionsPack.Core
{
    public static class DateTimeExtensions
    {
        private static readonly Lazy<GregorianCalendar> GregorianCalendarLazy = new Lazy<GregorianCalendar>();

        public static DateTime ToDateTime(this long unixUtcTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixUtcTimeStamp);
        }

        public static long ToUnixUtcTimeStamp(this DateTime dateTime)
        {
            return Convert.ToInt64(dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds);
        }
    }
}
