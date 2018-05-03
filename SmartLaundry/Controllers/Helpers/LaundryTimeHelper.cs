using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Controllers.Helpers
{
    public static class LaundryTimeHelper
    {
        public static DateTime GetLastEndTime(DateTime time)
        {
            if (time.Hour < 15)
            {
                return new DateTime(time.Year, time.Month, time.Day, 14, 59, 59, 999);
            }
            else if (time.Hour < 17)
            {
                return new DateTime(time.Year, time.Month, time.Day, 16, 59, 59, 999);
            }
            else if (time.Hour < 19)
            {
                return new DateTime(time.Year, time.Month, time.Day, 18, 59, 59, 999);
            }
            else if (time.Hour < 21)
            {
                return new DateTime(time.Year, time.Month, time.Day, 20, 59, 59, 999);
            }
            else
            {
                return new DateTime(time.Year, time.Month, time.Day, 22, 59, 59, 999);
            }
        }

        public static DateTime GetClosestStartTime(DateTime time)
        {
            DateTime dataTime;
            if (time.Hour >= 23)
            {
                dataTime = new DateTime(time.Year, time.Month, time.Day, 15, 0, 0, 0);
                dataTime.Add(new TimeSpan(24, 0, 0));
            }
            else if (time.Hour >= 21)
            {
                dataTime = new DateTime(time.Year, time.Month, time.Day, 21, 0, 0, 0);
            }
            else if (time.Hour >= 19)
            {
                dataTime = new DateTime(time.Year, time.Month, time.Day, 19, 0, 0, 0);
            }
            else if (time.Hour >= 17)
            {
                dataTime = new DateTime(time.Year, time.Month, time.Day, 17, 0, 0, 0);
            }
            else
            {
                dataTime = new DateTime(time.Year, time.Month, time.Day, 15, 0, 0, 0);
            }
            return dataTime;
        }
    }
}
