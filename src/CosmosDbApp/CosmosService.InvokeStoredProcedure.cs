using CosmosDbApp.Models.Write;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CosmosDbApp
{
    public partial class CosmosService
    {
        public async Task InvokeStoredProcedure()
        {
            using (CosmosClient client = CreateClient())
            {
                var container = client.GetContainer(databaseName, containerName);
                await CreateNewItems(container);
            }
        }

        private async Task CreateNewItems(Container container)
        {
            var (reservationId, items) = GetNewItemsToCreate();
            try
            {
                var result = await container.Scripts.ExecuteStoredProcedureAsync<string>(
                    StoredProcedureId,
                    new PartitionKey(reservationId),
                    new dynamic[] { items },
                    new StoredProcedureRequestOptions { EnableScriptLogging = true });

                this.logger.LogInformation(result.ScriptLog);
                this.logger.LogInformation("The request charge for this operation = {0} RU/s", result.RequestCharge);
                // While testing the method 3 times, got the following output (differed lightly across executions):
                // The request charge for this operation = 45.02 RU/s
                this.logger.LogInformation("Response from Cosmos DB = {0}", result.Resource);
            }
            catch (CosmosException cosmosException)
            {
                this.logger.LogError("Reservation {0} is not stored.", reservationId);
                this.logger.LogError(cosmosException.ResponseBody);
                this.logger.LogError(cosmosException.SubStatusCode.ToString());
                this.logger.LogError(cosmosException.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex.Message);
            }
        }

        private static (int, dynamic[]) GetNewItemsToCreate()
        {
            var reservationId = new Random().Next(1, 500);
            var items = new object[]
            {
                new
                {
                    ReservationId = reservationId,
                    Name = "Prague",
                    From = DateTime.Today.AddDays(7),
                    To = DateTime.Today.AddDays(8),
                    State = 0,
                    Type = nameof(Reservation),
                    id = Guid.NewGuid().ToString("D")
                },
                new
                {
                    ReservationId = reservationId,
                    Name = "Kiev",
                    From = DateTime.Today.AddDays(21),
                    To = DateTime.Today.AddDays(26),
                    State = 0,
                    Type = nameof(Reservation),
                    id = Guid.NewGuid().ToString("D")
                },
                new
                {
                    ReservationId = reservationId,
                    Name = "Jan",
                    PassportNumber = "NW23191WD",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new
                {
                    ReservationId = reservationId,
                    Name = "Angelique",
                    PassportNumber = "TW228391YC",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new
                {
                    ReservationId = reservationId,
                    Name = "Sven",
                    PassportNumber = "YR726387IU",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new
                {
                    ReservationId = reservationId,
                    Name = "Fleur",
                    PassportNumber = "YR726388IU",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                }
            };
            return (reservationId, items);
        }
    }
}
