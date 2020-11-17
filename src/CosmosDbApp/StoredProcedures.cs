using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    public class StoredProcedures
    {
        private readonly CosmosService cosmosService;

        public StoredProcedures(
            CosmosService cosmosService)
        {
            this.cosmosService = cosmosService;
        }

        [FunctionName(nameof(AddStoredProcedures))]
        public async Task<IActionResult> AddStoredProcedures(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req)
        {
            await cosmosService.RegisterStoredProcedure();
            return new OkObjectResult(default);
        }

        [FunctionName(nameof(InvokeStoredProcedure))]
        public async Task<IActionResult> InvokeStoredProcedure(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] 
            HttpRequest req)
        {
            await cosmosService.InvokeStoredProcedure();
            return new OkObjectResult(default);
        }
    }
}
