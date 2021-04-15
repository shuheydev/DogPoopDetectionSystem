using DogPoopDetectionSystem.Configurations;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DogPoopDetectionSystem.DogPoopDetectionFunctions
{
    public class DogPoopDetectionFunctions
    {
        private readonly double probablityThreshold = 0.7;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public DogPoopDetectionFunctions(HttpClient httpClient, IConfiguration config)
        {
            this._httpClient = httpClient;
            this._config = config;
        }

        [FunctionName("PoopImageAddedTrigger")]
        public async Task Run([BlobTrigger("poopimages/{name}", Connection = "")] Stream myBlob, string name, ILogger log)
        {
            log.LogInformation($"C# Blob trigger function Processed blob\n Name:{name} \n Size: {myBlob.Length} Bytes");

            //Blobの設定情報を読み込み
            var blobConfiguration = _config.GetSection("BlobConfiguration").Get<BlobConfiguration2>();
            log.LogDebug($"blob settings.Container name = {blobConfiguration.ContainerName}");

            //CustomVisionの設定情報を読み込み
            var customVisionSettings = _config.GetSection("CustomVisionConfiguration").Get<CustomVisionConfiguration2>();
            log.LogDebug($"custom vision settings.endpoint = {customVisionSettings.Endpoint}");

            //TeamsにWebhookで投稿する設定情報を読み込み
            var teamsWebhookConfiguration = _config.GetSection("TeamsWebhookConfiguration").Get<TeamsWebhookConfiguration>();

            //CustomVisionで処理する画像のURL
            string imageUrl = $"{blobConfiguration.ContainerUrl}/poopimages/{name}";

            #region Use Custom Vision SDK

            //SDK使おうとしたけれど、わからなくて挫折。→できた。
            CustomVisionPredictionClient predictionClient = AuthenticatePrediction(customVisionSettings.Endpoint, customVisionSettings.PredictionKey);
            var projectIdGuid = Guid.Parse(customVisionSettings.ProjectID);
            string publishedName = customVisionSettings.PublishedName;

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
            //Teamsに
            var teamsWebhookContentObject = new Hashtable
            {
                {"Text","💩しちゃった。片付けお願いだワン🐶" }
            };
            var teamsWebhookContent = new StringContent(JsonSerializer.Serialize(teamsWebhookContentObject));
            await _httpClient.PostAsync(teamsWebhookConfiguration.WebhookUrl, teamsWebhookContent);

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