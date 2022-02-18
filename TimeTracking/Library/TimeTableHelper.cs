using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTracking.Library
{
    public class TimeTableHelper
    {
        public static string GetTimeInHours(int time)
        {
            StringBuilder sb = new StringBuilder();

            int mod = time % 60;
            int hours = (time - mod) / 60;

            sb.Append(hours.ToString());
            sb.Append("h ");
            sb.Append(mod.ToString());
            sb.Append("m");

            return sb.ToString();
        }
    }
}
