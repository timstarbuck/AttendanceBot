using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceBot
{
    public class IdentificationResponse
    {
        public string FaceId { get; set; }

        public List<Candidate> Candidates { get; set; }
    }

    public class Candidate
    {
        public string PersonId { get; set; }

        public double Confidence { get; set; }
    }
}