using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using mlnet_func.Extensibility;

namespace mlnet_func.Functions
{
    public class TriggerModelRefresh
    {
        private readonly BlobModelLoader modelLoader;

        public TriggerModelRefresh(BlobModelLoader modelLoader)
        {
            this.modelLoader = modelLoader;
        }

        [FunctionName(nameof(TriggerModelRefresh))]
        public void Run([BlobTrigger("ml-models/{name}", Connection = "AzureWebJobsStorage")]
            Stream model, string name, ILogger log)
        {
            log.LogInformation($"Refresh model Name:{name} \t Size: {model.Length} Bytes");

            modelLoader.Refresh(model,name);
        }
    }
}