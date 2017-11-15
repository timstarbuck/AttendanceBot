using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Diagnostics;
using System.Text;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using AttendanceBot.Models;
using Newtonsoft.Json.Linq;

namespace AttendanceBot
{
    public class FaceServices
    {
        // TODO: put this somewhere else!!!
        const string subscriptionKey = "c385f8eb2b664487a658c435a6e240b8";

        // Replace or verify the region.
        //
        // You must use the same region in your REST API call as you used to obtain your subscription keys.
        // For example, if you obtained your subscription keys from the westus region, replace 
        // "westcentralus" in the URI below with "westus".
        //
        // NOTE: Free trial subscription keys are generated in the westcentralus region, so if you are using
        // a free trial subscription key, you should not need to change this region.
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/";


        public async Task<Boolean> EnsureGroupExistsAsync(string groupId)
        {
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/" + groupId.ToLower();

            HttpResponseMessage response;

            response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                // create group

                // Request body
                byte[] byteData = Encoding.UTF8.GetBytes("{name:'" + groupId + "',userData:''}");

                using (var content = new ByteArrayContent(byteData))
                {
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                    response = await client.PutAsync(uri, content);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Trace.TraceError(response.ReasonPhrase);
                    }
                }
            }
            else if (response.StatusCode == HttpStatusCode.OK)
            {
                Trace.TraceInformation("group exists");
            }
            else
            {
                Trace.TraceError(await response.Content.ReadAsStringAsync());
                throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
            }
            return true;
        }


        public async Task<string> AddPerson(string personName, string personGroup)
        {
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/" + personGroup.ToLower() + "/persons";

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{name:'" + personName + "',userData:''}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Trace.TraceInformation("Person {0} added!", personName);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Trace.TraceError(await response.Content.ReadAsStringAsync());
                    throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                }
            }

        }

        public async Task<string> AddPersonFace(string personId, string personGroup, byte[] image)
        {
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/" + personGroup.ToLower() + "/persons/" + personId + "/persistedFaces";

            HttpResponseMessage response;

            // Request body
            // byte[] byteData = Encoding.UTF8.GetBytes("{name:'" + personName + "',userData:''}");

            using (var content = new ByteArrayContent(image))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Trace.TraceInformation("PersonId {0} face added!", personId);
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    Trace.TraceError(await response.Content.ReadAsStringAsync());
                    throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
                }
            }

        }

        public async Task<List<Person>> ListPeople(string personGroup)
        {
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/" + personGroup.ToLower() + "/persons";

            HttpResponseMessage response;

            response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var data = await response.Content.ReadAsAsync<List<Person>>();
                return data;

            } else
            {
                throw new HttpException(await response.Content.ReadAsStringAsync());
            }

        }

        public async Task<string> FindPersonFromImage(string personGroup, byte[] image)
        {
            // detect
            // https://westus.dev.cognitive.microsoft.com/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395236
            string detectionResult = await MakeAnalysisRequest(new MemoryStream(image));

            JArray json = JArray.Parse(detectionResult);

            if (json.Count == 0)
            {
                return "Student not found.";
            }
            var faceId = json
                .FirstOrDefault(x => x.Value<int>("Id") == 0)
                .Value<string>("faceId");

            //identify
            // https://westus.dev.cognitive.microsoft.com/docs/services/563879b61984550e40cbbe8d/operations/563879b61984550f30395239

            string identificationResult = await MakeIdentificationRequest(personGroup, faceId);

            return identificationResult;
        }

        public async Task<bool> TrainGroup(string personGroup)
        {
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/" + personGroup.ToLower() + "/train";

            HttpResponseMessage response;

            response = await client.PostAsync(uri, new StringContent(""));

            if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Accepted)
            {
                return true;
            }
            else
            {
                throw new HttpException(await response.Content.ReadAsStringAsync());
            }

        }

        public async Task<List<PersonGroup>> ListGroups()
        {
            var groups = new List<PersonGroup>();
            HttpClient client = GetHttpClient();

            var uri = uriBase + "persongroups/";

            HttpResponseMessage response;

            response = await client.GetAsync(uri);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                groups = await response.Content.ReadAsAsync<List<PersonGroup>>();
                return groups;
            }
            else
            {
                throw new HttpException(await response.Content.ReadAsStringAsync());
            }

        }


        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return client;
        }

        private static async Task<String> MakeIdentificationRequest(String personGroupId, string faceId)
        {
            FaceDetectionRequest request = new FaceDetectionRequest();
            request.personGroupId = personGroupId.ToLower();
            request.maxNumOfCandidatesReturned = 1;
            request.confidenceThreshold = 0.5;
            request.faceIds = new List<string>();
            request.faceIds.Add(faceId);

            string requestJson = JsonConvert.SerializeObject(request);

            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            string uri = uriBase + "identify";

            HttpResponseMessage response;

            byte[] byteData = Encoding.UTF8.GetBytes(requestJson);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Trace.WriteLine("\nResponse:\n");
                Trace.WriteLine(contentString);
                return contentString;
            }
        }

        private static async Task<string> MakeAnalysisRequest(Stream image)
        {
            HttpClient client = new HttpClient();

            // Request headers.
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);

            // Request parameters. A third optional parameter is "details".
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=false&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

            // Assemble the URI for the REST API Call.
            string uri = uriBase + "detect" + "?" + requestParameters;

            HttpResponseMessage response;

            // Request body. Posts a locally stored JPEG image.
            byte[] byteData = ReadToEnd(image);

            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                // This example uses content type "application/octet-stream".
                // The other content types you can use are "application/json" and "multipart/form-data".
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                // Execute the REST API call.
                response = await client.PostAsync(uri, content);

                // Get the JSON response.
                string contentString = await response.Content.ReadAsStringAsync();

                // Display the JSON response.
                Trace.WriteLine("\nResponse:\n");
                Trace.WriteLine(contentString);
                return contentString;
            }

            // return "Test Id";
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