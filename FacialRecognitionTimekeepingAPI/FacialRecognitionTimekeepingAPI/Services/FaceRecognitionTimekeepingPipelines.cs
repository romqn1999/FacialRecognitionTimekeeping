using FacialRecognitionTimekeepingAPI.Helper;
using FacialRecognitionTimekeepingAPI.Models;
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
    public class FaceRecognitionTimekeepingPipelines
    {
        public BlockingCollectionPipelineAwaitable<string, bool> TestPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<string, bool>((builder) =>
                //inputFirst.AddStep(builder, input => FindMostCommon(input))
                //    .AddStep(builder, input => input.Length)
                //    .AddStep(builder, input => input % 2 == 1)
                builder.AddStep<string, string>(input => FindMostCommon(input))
                    .AddStep<string, int>(input => input.Length)
                    .AddStep<int, bool>(input => input % 2 == 1)
            );

        public BlockingCollectionPipelineAwaitable<Models.RegisterInputModel, Task<string>> RegisterPersonPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<Models.RegisterInputModel, Task<string>>((builder) =>
                builder.AddStep<Models.RegisterInputModel, Task<string>>(DetectFace)
                    .AddStep<Task<string>, Task<string>>(RegisterFace)
            );

        public BlockingCollectionPipelineAwaitable<Models.TimekeepingInputModel, string> TimekeepingPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<Models.TimekeepingInputModel, string>((builder) =>
                builder.AddStep<Models.TimekeepingInputModel, Task<string>>(RecognizeFace)
                    .AddStep<Task<string>, string>(Timekeeping)
            );

        public BlockingCollectionPipelineAwaitable<string, Task<string>> DeletePersonPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<string, Task<string>>((builder) =>
                builder.AddStep<string, Task<string>>(DeleteFace)
            );

        private static string FindMostCommon(string input)
        {
            Console.WriteLine(nameof(FindMostCommon));
            Console.WriteLine(input);
            return input.Split(' ')
                .GroupBy(word => word)
                .OrderBy(group => group.Count())
                .Last()
                .Key;
        }

        public static async Task<string> DetectFace(Models.RegisterInputModel registerInputModel)
        {
            if (registerInputModel.FormFile == default)
            {
                return "No file attached";
            }
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "cebed4f82c1b48fda770abebd5f38ee7");

            // Request parameters
            queryString["returnFaceId"] = "true";
            //queryString["returnFaceLandmarks"] = "true";
            //queryString["returnFaceAttributes"] = "{string}";
            queryString["recognitionModel"] = "recognition_03";
            queryString["returnRecognitionModel"] = "true";
            queryString["detectionModel"] = "detection_02";
            var uri = "https://thinh.cognitiveservices.azure.com/face/v1.0/detect?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = await registerInputModel.FormFile?.GetBytesAsync();

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }
            string responseStr = await response.Content.ReadAsStringAsync();
            var listFaces = JsonSerializer.Deserialize<List<Face>>(responseStr);
            return JsonSerializer.Serialize(
                listFaces.FirstOrDefault(
                    f => f.FaceRectangle.Width >= listFaces.Max(f => f.FaceRectangle.Width)));
        }

        public static async Task<string> RegisterFace(Task<string> input)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/{personGroupId}/persons?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }
            return input.Result + Environment.NewLine + await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> RecognizeFace(Models.TimekeepingInputModel input)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/identify?" + queryString;

            HttpResponseMessage response;

            // Request body
            byte[] byteData = Encoding.UTF8.GetBytes("{body}");

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                response = await client.PostAsync(uri, content);
            }

            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> DeleteFace(string input)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "{subscription key}");

            var uri = "https://westus.api.cognitive.microsoft.com/face/v1.0/persongroups/{personGroupId}/persons/{personId}?" + queryString;

            var response = await client.DeleteAsync(uri);
            return await response.Content.ReadAsStringAsync();
        }

        public static string Timekeeping(Task<string> input)
        {
            return "dacrom";
        }
    }
}
