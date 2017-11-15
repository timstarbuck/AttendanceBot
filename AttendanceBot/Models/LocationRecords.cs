using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceBot
{
    public class LocationRecord
    {
        public string PersonId { get; set; }

        public string Location { get; set; }

        public DateTime Timestamp { get; set; }

        public double Confidence { get; set; }
    }

    public static class LocationRecordLogs
    {
        static LocationRecordLogs()
        {
            LocationRecordLogs.Logs = new Dictionary<string, List<LocationRecord>>();
        }

        public static Dictionary<string, List<LocationRecord>> Logs { get; set; }
    }
}