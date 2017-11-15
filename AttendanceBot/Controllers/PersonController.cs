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

        public async Task<HttpResponseMessage> AddPersonFace(string personId, string groupName, [FromBody]byte[] image)
        {
            var face = new FaceServices();
            string faceId = await face.AddPersonFace(personId, groupName, image);
            return Request.CreateResponse(HttpStatusCode.OK, faceId);

        }
    }
}
