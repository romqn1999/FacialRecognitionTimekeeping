using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class CognitiveServiceApiRequest
    {
        public static string CognitiveSubcriptionKey = "cebed4f82c1b48fda770abebd5f38ee7";
        public static string CognitiveEndpoint = "https://thinh.cognitiveservices.azure.com";
        public static string CognitivePersonGroupId = "timekeeping";
        public static ILogger Logger { get; set; }

        public static async Task<HttpResponseMessage> DetectFaces(byte[] byteDataImageFace)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            // Request parameters
            queryString["returnFaceId"] = "true";
            //queryString["returnFaceLandmarks"] = "true";
            //queryString["returnFaceAttributes"] = "{string}";
            queryString["recognitionModel"] = "recognition_03";
            queryString["returnRecognitionModel"] = "true";
            queryString["detectionModel"] = "detection_02";
            var uri = $"{CognitiveEndpoint}/face/v1.0/detect?" + queryString;

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(byteDataImageFace))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> CreateCognitivePerson(string name, string userData = null)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            var uri = $"{CognitiveEndpoint}/face/v1.0/persongroups/{CognitivePersonGroupId}/persons?" + queryString;

            HttpResponseMessage response;

            var bodyObj = new { name, userData };
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(bodyObj));

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> AddFaceCognitivePerson(string personId, byte[] byteDataImageFace, Models.Face face)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            // Request parameters
            //queryString["userData"] = "{string}";
            queryString["targetFace"] = $"{face.FaceRectangle.Left},{face.FaceRectangle.Top},{face.FaceRectangle.Width},{face.FaceRectangle.Height}";
            queryString["detectionModel"] = "detection_01";
            var uri = $"{CognitiveEndpoint}/face/v1.0/persongroups/{CognitivePersonGroupId}/persons/{personId}/persistedFaces?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = byteDataImageFace;

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> DeleteCognitivePerson(string personId)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            var uri = $"{CognitiveEndpoint}/face/v1.0/persongroups/{CognitivePersonGroupId}/persons/{personId}?" + queryString;

            HttpResponseMessage response = await client.DeleteAsync(uri);

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> TrainCognitivePersonGroup()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            var uri = $"{CognitiveEndpoint}/face/v1.0/persongroups/{CognitivePersonGroupId}/train?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> GetTrainingCognitivePersonGroupStatus()
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            var uri = $"{CognitiveEndpoint}/face/v1.0/persongroups/{CognitivePersonGroupId}/training?" + queryString;

            var response = await client.GetAsync(uri);

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

        public static async Task<HttpResponseMessage> IdentifyFaces(List<string> faceIds)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", CognitiveSubcriptionKey);

            var uri = $"{CognitiveEndpoint}/face/v1.0/identify?" + queryString;

            HttpResponseMessage response;

            Models.IdentifyPersonRequestBody identifyPersonGroupRequestBody = new Models.IdentifyPersonRequestBody { PersonGroupId = CognitivePersonGroupId, FaceIds = faceIds };
            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(identifyPersonGroupRequestBody));

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            string responseStr = await response.Content.ReadAsStringAsync();
            Logger.LogInformation(responseStr);

            return response;
        }

    }
}
