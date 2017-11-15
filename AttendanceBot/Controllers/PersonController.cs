using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using System.Diagnostics;

namespace AttendanceBot
{
    public class PersonController : ApiController
    {
        private readonly double _confidenceMatch = .5;

        public async Task<HttpResponseMessage> AddGroup(string groupName)
        {
            var face = new FaceServices();
            await face.EnsureGroupExistsAsync(groupName);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public async Task<HttpResponseMessage> AddNewPerson (string personName, string groupName)
        {
            var face = new FaceServices();
            string personId = await face.AddPerson(personName, groupName);
            return Request.CreateResponse(HttpStatusCode.OK, personId);
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ListPeople(string groupName)
        {
            var face = new FaceServices();
            var people = await face.ListPeople(groupName);
            return Request.CreateResponse(HttpStatusCode.OK, people);
        }

        public async Task<HttpResponseMessage> AddPersonFace(string personId, string groupName, string imagePath)
        {
            byte[] imageBytes;
            if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                using (var webClient = new WebClient())
                {
                    imageBytes = webClient.DownloadData(imagePath);
                }
            }
            else
            {
                imageBytes = System.IO.File.ReadAllBytes(imagePath);
            }
            var face = new FaceServices();
            string faceId = await face.AddPersonFace(personId, groupName, imageBytes);
            return Request.CreateResponse(HttpStatusCode.OK, faceId);

        }

        public async Task<HttpResponseMessage> TrainGroup(string groupName)
        {
            var face = new FaceServices();
            await face.TrainGroup(groupName);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        public async Task<HttpResponseMessage> ListGroups()
        {
            var face = new FaceServices();
            var groups = await face.ListGroups();
            return Request.CreateResponse(HttpStatusCode.OK, groups);
        }

        public async Task<HttpResponseMessage> LogLocation(string groupName, string location, string imagePath)
        {
            byte[] imageBytes;
            if (Uri.IsWellFormedUriString(imagePath, UriKind.Absolute))
            {
                using (var webClient = new WebClient())
                {
                    imageBytes = webClient.DownloadData(imagePath);
                }
            } else
            {
                imageBytes = System.IO.File.ReadAllBytes(imagePath);
            }
            
            var face = new FaceServices();
            string data = await face.FindPersonFromImage(groupName, imageBytes);
            var idResponse = JsonConvert.DeserializeObject <List<IdentificationResponse>>(data);
            if (idResponse[0].Candidates.Count == 1 && idResponse[0].Candidates[0].Confidence > _confidenceMatch )
            {
                var log = new LocationRecord()
                {
                    PersonId = idResponse[0].Candidates[0].PersonId,
                    Location = location,
                    Timestamp = DateTime.UtcNow,
                    Confidence = idResponse[0].Candidates[0].Confidence
                };

                Trace.TraceInformation("Matched {0} at {1} with confidence {2}", log.PersonId, log.Location, idResponse[0].Candidates[0].Confidence);
                
                // log something!
                if (!LocationRecordLogs.Logs.ContainsKey(log.PersonId))
                {
                    LocationRecordLogs.Logs.Add(log.PersonId, new List<LocationRecord>());
                }
                LocationRecordLogs.Logs[log.PersonId].Add(log);
                return Request.CreateResponse(HttpStatusCode.OK, log.PersonId);
            }
            return Request.CreateResponse(HttpStatusCode.NotFound);

        }

        [HttpGet]
        public HttpResponseMessage ListRecords()
        {
            return Request.CreateResponse(HttpStatusCode.OK, LocationRecordLogs.Logs);
        }

        private static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }

    }
}
