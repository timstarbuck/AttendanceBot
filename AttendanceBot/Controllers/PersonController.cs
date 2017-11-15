using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace AttendanceBot
{
    public class PersonController : ApiController
    {
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
            //var imageBytes = Convert.FromBase64String(image);
            //var imageBytes = await Request.Content.ReadAsByteArrayAsync();
            //if (imageBytes.Length == 0)
            //{
            //    var stream = await Request.Content.ReadAsStreamAsync();
            //    imageBytes = ReadToEnd(stream);
            //}
            var imageBytes = System.IO.File.ReadAllBytes(imagePath);
            var face = new FaceServices();
            string faceId = await face.AddPersonFace(personId, groupName, imageBytes);
            return Request.CreateResponse(HttpStatusCode.OK, faceId);

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
