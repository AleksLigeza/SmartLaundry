using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartLaundry.Controllers.Helpers
{
    public static class LaundryTimeHelper
    {
        public static DateTime GetClosestStartTime(DateTime searchDateTime, TimeSpan startTime, TimeSpan shiftTime, int shiftCount)
        {
            DateTime date = searchDateTime.Date;
            TimeSpan time = searchDateTime.Subtract(date);

            for (int shift = shiftCount - 1; shift >= 0; shift--)
            {
                TimeSpan shiftStartTime = startTime + shiftTime * shift;
                TimeSpan shiftEndTime = startTime + shiftTime * (shift + 1);
                if (time >= shiftStartTime && time < shiftEndTime)
                {
                    return date.Add(shiftStartTime);
                }
            }

            TimeSpan minShiftStartTime = startTime;
            TimeSpan maxShiftEndTime = startTime + shiftTime * shiftCount;

            if (time < minShiftStartTime)
            {
                return date.Add(minShiftStartTime);
            }

            return date.AddDays(1).Add(minShiftStartTime);
        }
    }
}
