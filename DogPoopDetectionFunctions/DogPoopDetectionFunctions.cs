using DogPoopDetectionSystem.Configurations;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace DogPoopDetectionSystem.DogPoopDetectionFunctions
{
    public class DogPoopDetectionFunctions
    {
        public DogPoopDetectionFunctions(HttpClient httpClient)
        {
        }

        private double probablityThreshold = 0.7;

        [FunctionName("PoopImageAddedTrigger")]
        public async Task Run([BlobTrigger("poopimages/{name}", Connection = "")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            string imageUrl = $"{BlobConfiguration.ContainerUrl}/poopimages/{name}";

            #region Use Custom Vision SDK

            //SDK使おうとしたけれど、わからなくて挫折。→できた。
            CustomVisionPredictionClient predictionClient = AuthenticatePrediction(CustomVisionConfiguration.Endpoint, CustomVisionConfiguration.PredictionKey);
            var projectIdGuid = Guid.Parse(CustomVisionConfiguration.ProjectID);
            string publishedName = CustomVisionConfiguration.PublishedName;

            //結果は70%以上に絞る
            var resultBySDK = (await predictionClient.DetectImageUrlAsync(projectIdGuid, publishedName, new ImageUrl(imageUrl))).Predictions
                .Where(prediction => prediction.Probability > probablityThreshold);

            //何も無ければ終了
            if (!resultBySDK.Any())
            {
                log.LogInformation($"No poop, Cage is clean.");
                return;
            }

            log.LogInformation($"Top probability is {resultBySDK.First().Probability}. Threshold: {probablityThreshold}");
            log.LogInformation($"Your dog probably pooped.");

            //通知を出す。
            //とりあえずメールかな。
            //SendGridか。

            #endregion Use Custom Vision SDK
        }

        private CustomVisionPredictionClient AuthenticatePrediction(string endpoint, string predictionKey)
        {
            // Create a prediction endpoint, passing in the obtained prediction key
            CustomVisionPredictionClient predictionApi = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
            {
                Endpoint = endpoint
            };
            return predictionApi;
        }
    }
}