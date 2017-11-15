using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceBot.Models
{
    public class FaceDetectionRequest
    {
        public string personGroupId { get; set; }
        public List<string> faceIds { get; set; }
        public int maxNumOfCandidatesReturned { get; set; }
        public double confidenceThreshold { get; set; }
    }
}