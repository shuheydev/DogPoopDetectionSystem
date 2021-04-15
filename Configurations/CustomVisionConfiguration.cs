using System;
using System.Collections.Generic;
using System.Text;

namespace DogPoopDetectionSystem.Configurations
{
    public static class CustomVisionConfiguration
    {
        public static readonly string Endpoint = "{Custom Vision Endpoint}";
        public static readonly string PredictionKey = "{Prediction Key}";
        public static readonly string ProjectID = "{Project ID}";
        public static readonly string PublishedName = "{Published Name}";
    }

    public class CustomVisionConfiguration2
    {
        public string Endpoint { get; set; }
        public string PredictionKey { get; set; }
        public string ProjectID { get; set; }
        public string PublishedName { get; set; }
    }
}
