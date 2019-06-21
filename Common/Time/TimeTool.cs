using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMEWebCAD.Common.Time
{
    public static class TimeTool
    {
        public static string DateDiff(DateTime DateTime1, DateTime DateTime2)
        {
            string dateDiff = null;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                string hours = ts.Hours.ToString(), minutes = ts.Minutes.ToString(), seconds = ts.Seconds.ToString();
                if (ts.Hours < 10)
                {
                    hours = "0" + ts.Hours.ToString();
                }
                if (ts.Minutes < 10)
                {
                    minutes = "0" + ts.Minutes.ToString();
                }
                if (ts.Seconds < 10)
                {
                    seconds = "0" + ts.Seconds.ToString();
                }
                dateDiff = hours + ":" + minutes + ":" + seconds;
            }
            catch
            {
            }
            return dateDiff;
        }
        public static int DateDiffBySeconds(DateTime DateTime1, DateTime DateTime2)
        {
            int dateDiff = -1;
            try
            {
                TimeSpan ts1 = new TimeSpan(DateTime1.Ticks);
                TimeSpan ts2 = new TimeSpan(DateTime2.Ticks);
                TimeSpan ts = ts1.Subtract(ts2).Duration();
                int hours = ts.Hours, minutes = ts.Minutes, seconds = ts.Seconds;
                dateDiff = 60 * 60 * hours + 60 * minutes + seconds;
            }
            catch
            {
            }
            return dateDiff;
        }
    }
}
