using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async static Task<List<LocationRecord>> GetByPerson(string personName, string groupName)
        {
            var records = new List<LocationRecord>();
            var face = new FaceServices();
            var people = await face.ListPeople(groupName);

            var person = people.FirstOrDefault((p) => p.Name.Equals(personName, StringComparison.InvariantCultureIgnoreCase));
            if (person != null)
            {
                if (LocationRecordLogs.Logs.ContainsKey(person.PersonId))
                {
                    records = LocationRecordLogs.Logs[person.PersonId];
                }
            }

            return records;
        }
    }
}