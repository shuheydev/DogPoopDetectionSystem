namespace DogPoopDetectionSystem.Configurations
{
    public static class BlobConfiguration
    {
        public const string ContainerName = "{Blob Container Name}";
        public static readonly string ConnectionString = "{Blob Connection String}";
        public const string BlobUrl = "{Blob Container Url}";
    }

    public class BlobConfiguration2
    {
        public string ContainerName { get; set; }
        public string ConnectionString { get; set; }
        public string ContainerUrl { get; set; }
    }
}
