using System;

namespace SmartLaundry.Controllers.Helpers
{
    public static class LaundryTimeHelper
    {
        public static DateTime GetClosestStartTime(DateTime searchDateTime, TimeSpan startTime, TimeSpan shiftTime, int shiftCount)
        {
            var date = searchDateTime.Date;
            var time = searchDateTime.Subtract(date);

            for (var shift = shiftCount - 1; shift >= 0; shift--)
            {
                var shiftStartTime = startTime + shiftTime * shift;
                var shiftEndTime = startTime + shiftTime * (shift + 1);
                if (time >= shiftStartTime && time < shiftEndTime)
                {
                    return date.Add(shiftStartTime);
                }
            }

            var minShiftStartTime = startTime;
            var maxShiftEndTime = startTime + shiftTime * shiftCount;

            return time < minShiftStartTime ? date.Add(minShiftStartTime) : date.AddDays(1).Add(minShiftStartTime);
        }
    }
}
