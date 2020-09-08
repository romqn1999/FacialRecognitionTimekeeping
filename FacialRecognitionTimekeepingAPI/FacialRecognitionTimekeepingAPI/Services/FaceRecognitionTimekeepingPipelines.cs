using FacialRecognitionTimekeepingAPI.Helper;
using FacialRecognitionTimekeepingAPI.Models;
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
using System.Web.Mvc;

namespace FacialRecognitionTimekeepingAPI.Services
{
    public class FaceRecognitionTimekeepingPipelines
    {
        private TimekeepingContext _timekeepingContext;

        public static ILogger Logger { get; set; }

        public FaceRecognitionTimekeepingPipelines(TimekeepingContext timekeepingContext)
        {
            _timekeepingContext = timekeepingContext;
        }

        public BlockingCollectionPipelineAwaitable<string, bool> TestPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<string, bool>((builder) =>
                //inputFirst.AddStep(builder, input => FindMostCommon(input))
                //    .AddStep(builder, input => input.Length)
                //    .AddStep(builder, input => input % 2 == 1)
                builder.AddStep<string, string>(input => FindMostCommon(input))
                    .AddStep<string, int>(input => input.Length)
                    .AddStep<int, bool>(input => input % 2 == 1)
            );

        public BlockingCollectionPipelineAwaitable<Models.RegisterPipelineModel, Task<RegisterPipelineModel>> RegisterPersonPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<Models.RegisterPipelineModel, Task<RegisterPipelineModel>>((builder) =>
                builder.AddStep<Models.RegisterPipelineModel, Task<RegisterPipelineModel>>(CreateCognitivePerson)
                    .AddStep<Task<RegisterPipelineModel>, Task<RegisterPipelineModel>>(DetectBiggestFace)
                    .AddStep<Task<RegisterPipelineModel>, Task<RegisterPipelineModel>>(AddFaceCognitivePerson)
                    .AddStep<Task<RegisterPipelineModel>, Task<RegisterPipelineModel>>(CreateTimekeepingPerson)
                    .AddStep<Task<RegisterPipelineModel>, Task<RegisterPipelineModel>>(TrainCognitivePersonGroup)
            );

        public BlockingCollectionPipelineAwaitable<RecognizeTimekeepingPipelineModels, Task<RecognizeTimekeepingPipelineModels>> TimekeepingPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<RecognizeTimekeepingPipelineModels, Task<RecognizeTimekeepingPipelineModels>>((builder) =>
                builder.AddStep<RecognizeTimekeepingPipelineModels, Task<RecognizeTimekeepingPipelineModels>>(DetectMultipleFaces)
                    .AddStep<Task<RecognizeTimekeepingPipelineModels>, Task<RecognizeTimekeepingPipelineModels>>(RecognizeFaces)
                    .AddStep<Task<RecognizeTimekeepingPipelineModels>, Task<RecognizeTimekeepingPipelineModels>>(Timekeeping)
            );

        public BlockingCollectionPipelineAwaitable<DeletePipelineModel, Task<DeletePipelineModel>> DeletePersonPipeline { get; private set; } =
            new BlockingCollectionPipelineAwaitable<DeletePipelineModel, Task<DeletePipelineModel>>((builder) =>
                builder.AddStep<DeletePipelineModel, Task<DeletePipelineModel>>(DeleteTimekeeping)
                    .AddStep<Task<DeletePipelineModel>, Task<DeletePipelineModel>>(DeleteCognitive)
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

        public static async Task<RegisterPipelineModel> DetectBiggestFace(Task<Models.RegisterPipelineModel> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.DetectFaces(await pipelineModel.FormFile?.GetBytesAsync());

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var listFaces = JsonSerializer.Deserialize<List<Face>>(responseStr);
                pipelineModel.BiggestFace = listFaces?.FirstOrDefault(
                        f => f.FaceRectangle.Width >= listFaces.Max(f => f.FaceRectangle.Width));
                if (pipelineModel.BiggestFace == default)
                {
                    pipelineModel.SetError("Face not found");
                }
            }
            else
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }

        public static async Task<RegisterPipelineModel> CreateCognitivePerson(RegisterPipelineModel input)
        {
            var pipelineModel = input;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            if (pipelineModel.FormFile == default)
            {
                pipelineModel.SetError("No file attached");
                return pipelineModel;
            }

            if (pipelineModel.TimekeepingContext.TimekeepingPeople.Find(pipelineModel.AliasId) != null)
            {
                pipelineModel.SetError("Alias id already exists");
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.CreateCognitivePerson(pipelineModel.AliasId);

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                pipelineModel.PersonInfo = JsonSerializer.Deserialize<PersonInfo>(responseStr);
            }
            else
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }

        public static async Task<RegisterPipelineModel> AddFaceCognitivePerson(Task<RegisterPipelineModel> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.AddFaceCognitivePerson(pipelineModel.PersonInfo.PersonId, await pipelineModel.FormFile?.GetBytesAsync(), pipelineModel.BiggestFace);

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                pipelineModel.BiggestFace.PersistedFaceId = JsonDocument.Parse(responseStr).RootElement.GetProperty("persistedFaceId").GetString();
            }
            else
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }

        public static async Task<RegisterPipelineModel> CreateTimekeepingPerson(Task<RegisterPipelineModel> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                //if (!string.IsNullOrEmpty(pipelineModel?.PersonInfo?.PersonId))
                //{
                //    await CognitiveServiceApiRequest.DeleteCognitivePerson(pipelineModel.PersonInfo.PersonId);
                //}
                return pipelineModel;
            }

            pipelineModel.TimekeepingPerson = new TimekeepingPerson { AliasId = pipelineModel.AliasId, CognitivePersonId = pipelineModel.PersonInfo.PersonId };

            try
            {
                pipelineModel.TimekeepingContext.TimekeepingPeople.Add(pipelineModel.TimekeepingPerson);
                await pipelineModel.TimekeepingContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                pipelineModel.SetError(e.Message);
            }

            return pipelineModel;
        }

        public static async Task<RegisterPipelineModel> TrainCognitivePersonGroup(Task<RegisterPipelineModel> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                if (!string.IsNullOrEmpty(pipelineModel?.PersonInfo?.PersonId))
                {
                    await CognitiveServiceApiRequest.DeleteCognitivePerson(pipelineModel.PersonInfo.PersonId);
                }
                return pipelineModel;
            }

            HttpResponseMessage response;
            string trainingStatus = null;

            response = await CognitiveServiceApiRequest.TrainCognitivePersonGroup();
            await Task.Run(async () =>
            {
                do
                {
                    await Task.Delay(1000);
                    response = await CognitiveServiceApiRequest.GetTrainingCognitivePersonGroupStatus();
                    if (response.IsSuccessStatusCode)
                    {
                        string responseStr = await response.Content.ReadAsStringAsync();
                        //trainingStatusResponse = JsonSerializer.Deserialize(responseStr, trainingStatusResponse.GetType());
                        trainingStatus = JsonDocument.Parse(responseStr).RootElement.GetProperty("status").GetString();
                    }
                    else
                    {
                        trainingStatus = "failed";
                    }
                }
                while (trainingStatus == "running");
            });

            return pipelineModel;
        }

        public static async Task<RecognizeTimekeepingPipelineModels> DetectMultipleFaces(RecognizeTimekeepingPipelineModels inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.DetectFaces(await pipelineModel.FormFile?.GetBytesAsync());

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                pipelineModel.Faces = JsonSerializer.Deserialize<List<Face>>(responseStr);
                if (pipelineModel.Faces?.Count <= 0)
                {
                    pipelineModel.SetError("No face detected");
                }
            }
            else
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }

        public static async Task<RecognizeTimekeepingPipelineModels> RecognizeFaces(Task<RecognizeTimekeepingPipelineModels> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.IdentifyFaces(pipelineModel.Faces.Select(f => f.FaceId).ToList());

            string responseStr = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                pipelineModel.IdentifyPeopleRespone = JsonSerializer.Deserialize<List<IdentifyPersonRespone>>(responseStr);
                pipelineModel.TimekeepingPeople = new List<TimekeepingPerson>();
                foreach (var identifiedPerson in pipelineModel.IdentifyPeopleRespone)
                {
                    string identifiedPersonId = identifiedPerson.Candidates?.FirstOrDefault()?.PersonId;
                    var timekeepingPerson = pipelineModel.TimekeepingContext.TimekeepingPeople.FirstOrDefault(p => (p.CognitivePersonId == identifiedPersonId));
                    if (timekeepingPerson != null)
                    {
                        pipelineModel.TimekeepingPeople.Add(timekeepingPerson);
                    }
                }
            }
            else
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }

        public static async Task<RecognizeTimekeepingPipelineModels> Timekeeping(Task<RecognizeTimekeepingPipelineModels> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            foreach (var timekeepingPerson in pipelineModel.TimekeepingPeople)
            {
                try
                {
                    pipelineModel.TimekeepingContext.TimekeepingRecords.Add(
                        new TimekeepingRecord
                        {
                            AliasId = timekeepingPerson.AliasId,
                            TimekeepingRecordUnixTimestampSeconds = DateTimeOffset.Now.ToUnixTimeSeconds()
                        });
                    await pipelineModel.TimekeepingContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    Logger.LogError(e, e.Message);
                    pipelineModel.SetError(e.Message);
                }
            }

            return pipelineModel;
        }

        public static async Task<DeletePipelineModel> DeleteTimekeeping(DeletePipelineModel inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            try
            {
                pipelineModel.TimekeepingPerson = await pipelineModel.TimekeepingContext.TimekeepingPeople.FindAsync(pipelineModel.AliasId);
                if (pipelineModel.TimekeepingPerson == null)
                {
                    pipelineModel.SetError("Not found");
                    return pipelineModel;
                }
                pipelineModel.TimekeepingContext.TimekeepingPeople.Remove(pipelineModel.TimekeepingPerson);
                await pipelineModel.TimekeepingContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
                pipelineModel.SetError(e.Message);
            }

            return pipelineModel;
        }

        public static async Task<DeletePipelineModel> DeleteCognitive(Task<DeletePipelineModel> inputPipelineModel)
        {
            var pipelineModel = inputPipelineModel.Result;
            if (pipelineModel.HasError)
            {
                return pipelineModel;
            }

            var response = await CognitiveServiceApiRequest.DeleteCognitivePerson(pipelineModel.TimekeepingPerson.CognitivePersonId);

            string responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                pipelineModel.SetError(responseStr);
            }

            return pipelineModel;
        }
    }
}
