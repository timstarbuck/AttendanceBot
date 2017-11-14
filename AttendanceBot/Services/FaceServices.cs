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

        public async Task<string> AddPersonFace(string personId, string personGroup)
        {
            // TODO
            //HttpClient client = GetHttpClient();

            //var uri = uriBase + "persongroups/" + personGroup.ToLower() + "/persons";

            //HttpResponseMessage response;

            //// Request body
            //byte[] byteData = Encoding.UTF8.GetBytes("{name:'" + personName + "',userData:''}");

            //using (var content = new ByteArrayContent(byteData))
            //{
            //    content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            //    response = await client.PostAsync(uri, content);

            //    if (response.StatusCode == HttpStatusCode.OK)
            //    {
            //        Trace.TraceInformation("Person {0} added!", personName);
            //        return await response.Content.ReadAsStringAsync();
            //    }
            //    else
            //    {
            //        Trace.TraceError(await response.Content.ReadAsStringAsync());
            //        throw new HttpException((int)response.StatusCode, response.ReasonPhrase);
            //    }
            //}

        }

        private HttpClient GetHttpClient()
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            return client;
        }


    }
}