using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    public partial class CosmosService
    {
        public async Task RegisterStoredProcedure()
        {
            using (CosmosClient client = GetClient())
            {
                var container = client.GetContainer(databaseName, containerName);
                await RegisterStoredProcedure(container);
            }
        }
        private async Task RegisterStoredProcedure(Container container)
        {
            var storedProcedureProperties = new StoredProcedureProperties
            {
                Id = StoredProcedureId,
                Body = await File.ReadAllTextAsync($@".\js\{StoredProcedureId}.js")
            };
            try
            {
                var storedProcedureResponse = await container
                    .Scripts
                    .CreateStoredProcedureAsync(storedProcedureProperties);
                var serializedResponse = JsonConvert.SerializeObject(storedProcedureResponse);
                this.logger.LogInformation(serializedResponse);
            }
            catch (CosmosException cosmosException) when (cosmosException.StatusCode == HttpStatusCode.Conflict)
            {
                var storedProcedureResponse = await container.Scripts.ReplaceStoredProcedureAsync(storedProcedureProperties);
                var serializedResponse = JsonConvert.SerializeObject(storedProcedureResponse);
                this.logger.LogInformation(serializedResponse);
            }
            catch (CosmosException cosmosException)
            {
                this.logger.LogError(cosmosException.ResponseBody);
                this.logger.LogError(cosmosException.SubStatusCode.ToString());
                this.logger.LogError(cosmosException.Message);
            }
            catch (Exception e)
            {
                this.logger.LogCritical(e.Message);
            }
        }

    }
}
