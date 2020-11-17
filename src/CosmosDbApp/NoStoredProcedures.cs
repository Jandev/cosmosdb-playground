using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    public class NoStoredProcedures
    {
        private readonly CosmosService cosmosService;

        public NoStoredProcedures(CosmosService cosmosService)
        {
            this.cosmosService = cosmosService;
        }
        [FunctionName("NoStoredProcedures")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            await cosmosService.InvokePlainCommand();
            return new OkObjectResult(default);
        }
    }
}
