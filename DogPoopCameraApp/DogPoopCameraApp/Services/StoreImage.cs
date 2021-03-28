using Azure.Storage.Blobs;
using DogPoopDetectionSystem.Configurations;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace DogPoopCameraApp.Services
{
    public class StoreImage
    {
        private BlobContainerClient _containerClient;

        public StoreImage()
        {
        }

        public async Task InitializeAsync()
        {
            var blobServiceClient = new BlobServiceClient(BlobConfiguration.ConnectionString);
            CancellationToken cancellationToken = CancellationToken.None;
            var containers = blobServiceClient.GetBlobContainers();

            if (!containers.Any(container => container.Name == BlobConfiguration.ContainerName))
            {
                _containerClient = await blobServiceClient.CreateBlobContainerAsync(BlobConfiguration.ContainerName);
            }
            else
            {
                _containerClient = blobServiceClient.GetBlobContainerClient(BlobConfiguration.ContainerName);
            }
        }

        public async Task<bool> ToBlobStorage(ImageSource image)
        {
            string blobFileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".jpg";
            var blobClient = _containerClient.GetBlobClient(blobFileName);

            var streamImageSource = (StreamImageSource)image;
            CancellationToken cancellationToken = CancellationToken.None;
            var imageStream = await streamImageSource.Stream(cancellationToken);
            await blobClient.UploadAsync(imageStream);

            return true;
        }
    }
}