using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CosmosDbApp
{
    public partial class CosmosService
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<CosmosService> logger;
        private readonly string databaseName;
        private readonly string containerName;

        private const string StoredProcedureId = "spCreateReservations";

        public CosmosService(
            IConfiguration configuration,
            ILogger<CosmosService> logger)
        {
            this.configuration = configuration;
            this.logger = logger;

            databaseName = this.configuration["CosmosDb:Database"];
            containerName = this.configuration["CosmosDb:Container"];
        }


        private static CosmosClient _cosmosClient;
        private static CosmosClient GetClient()
        {
            return _cosmosClient;
        }

        internal static CosmosClient SetClient(IConfiguration configuration)
        {
            var endpoint = configuration["CosmosDb:Endpoint"];
            var authKey = configuration["CosmosDb:Key"];
            if (_cosmosClient == null)
            {
                _cosmosClient = new CosmosClient(endpoint, authKey);
            }

            return _cosmosClient;
        }
    }
}
