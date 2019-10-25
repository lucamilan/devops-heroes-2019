using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.ML;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace mlnet_func.Extensibility
{
    public class BlobModelLoader : ModelLoader, IDisposable
    {
        public const string ContainerName = "ml-models";
        private readonly IConfiguration configuration;
        private readonly MLContext context;
        private readonly ILogger<BlobModelLoader> logger;
        private ITransformer model; 
        private ModelReloadToken reloadToken;
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1,1);

        public BlobModelLoader(IOptions<MLOptions> contextOptions, IConfiguration configuration,
            ILogger<BlobModelLoader> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
            reloadToken = new ModelReloadToken();
            context = contextOptions.Value?.MLContext;
        }

        private CloudBlobClient BlobClient => CloudStorageAccount.Parse(configuration["AzureWebJobsStorage"]).CreateCloudBlobClient();

        public string LastModelLoaded { get; private set; } = "";

        internal virtual void Refresh(Stream stream , string name)
        {
            logger.LogWarning($"Start Refresh");

            semaphoreSlim.Wait();

            try
            {
                var token = Interlocked.Exchange(ref reloadToken, new ModelReloadToken());
                
                LastModelLoaded = name;
                
                model = context.Model.Load(stream, out _);
                
                token.OnReload();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, nameof(Refresh));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        internal virtual async Task Load()
        {
            logger.LogWarning($"Start Load");
            
            var blob = (await BlobClient.ListBlobs(ContainerName).ConfigureAwait(false)).FirstOrDefault();
            
            if (blob is null) throw new ModelNotFoundExceptionException();
            
            await semaphoreSlim.WaitAsync().ConfigureAwait(false);
            
            try
            {
                LastModelLoaded = blob.Name;

                logger.LogWarning($"Reading model '{blob.Name}' modified {blob.Properties.LastModified}");

                using (var stream = await blob.OpenReadAsync())
                {
                    model = context.Model.Load(stream, out _);
                }
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        public override IChangeToken GetReloadToken()
        {
            return reloadToken;
        }

        public override ITransformer GetModel()
        {
            if (model is null)
                throw new ModelNotFoundExceptionException(
                    "Load must be called on a ModelLoader before it can be used.");

            return model;
        }

        public void Dispose()
        {
        }
    }
}