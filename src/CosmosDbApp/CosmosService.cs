using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CosmosDbApp
{
    public partial class CosmosService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<CosmosService> logger;
        private readonly string endpoint;
        private readonly string authKey;
        private readonly string databaseName;
        private readonly string containerName;

        private const string StoredProcedureId = "spCreateReservations";

        public CosmosService(
            IConfiguration configuration,
            ILogger<CosmosService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            endpoint = this.configuration["CosmosDb:Endpoint"];
            authKey = this.configuration["CosmosDb:Key"];
            databaseName = this.configuration["CosmosDb:Database"];
            containerName = this.configuration["CosmosDb:Container"];
        }

        private CosmosClient CreateClient()
        {
            return new CosmosClient(endpoint, authKey);
        }
    }
}
