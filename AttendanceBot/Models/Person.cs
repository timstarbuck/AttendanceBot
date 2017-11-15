using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceBot
{
    public class Person
    {
        public string PersonId { get; set; }

        public string Name { get; set; }

        public string Arrived { get; set; }

        public string Departed { get; set; }

        public string Bus { get; set; }

        public string UserData { get; set; }

        public List<string> PersistedFaceIds { get; set; }

        public override string ToString()
        {
            return string.Format("PersonId: {0}, Name: {1}", this.PersonId, this.Name);
        }
    }
}