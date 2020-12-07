using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbApp.Models.Write;
using Newtonsoft.Json;

namespace CosmosDbApp
{
    public partial class CosmosService
    {
        public async Task InvokePlainCommand()
        {
            using (CosmosClient client = GetClient())
            {
                var container = client.GetContainer(databaseName, containerName);
                await AddNewItems(container);
            }
        }

        private async Task AddNewItems(Container container)
        {
            var (reservationId, items) = GetNewTypedItemsToCreate();
            try
            {
                double total = 0;
                total += await CreateItems<Reservation>(container, items, reservationId, total);
                total += await CreateItems<Traveller>(container, items, reservationId, total);
                this.logger.LogInformation("The total request charge for these oparations are {0} RU/s", total);
                // While testing the method 3 times, got the following output (differed lightly across executions):
                // The total request charge for these oparations are 59.43999999999999 RU/s
            }
            catch (CosmosException cosmosException)
            {
                this.logger.LogError(cosmosException.ResponseBody);
                this.logger.LogError(cosmosException.SubStatusCode.ToString());
                this.logger.LogError(cosmosException.Message);
            }
            catch (Exception ex)
            {
                this.logger.LogCritical(ex.Message);
            }
        }

        private async Task<double> CreateItems<T>(Container container, object[] items, int reservationId, double subtotal)
            where T : class
        {
            foreach (var item in items.Where(i => i.GetType() == typeof(T)))
            {
                ItemResponse<T> result = await container.CreateItemAsync((T)item, new PartitionKey(reservationId));
                subtotal += result.RequestCharge;
                this.logger.LogInformation("The request charge for this operation = {0} RU/s", result.RequestCharge);
                string serializedResource = JsonConvert.SerializeObject(result.Resource);
                this.logger.LogInformation("Response from Cosmos DB = {0}", serializedResource);
            }

            return subtotal;
        }

        private static (int, object[]) GetNewTypedItemsToCreate()
        {
            var reservationId = new Random().Next(1, 500);
            var items = new object[]
            {
                new Reservation
                {
                    ReservationId = reservationId,
                    Name = "Prague",
                    From = DateTime.Today.AddDays(7),
                    To = DateTime.Today.AddDays(8),
                    State = 0,
                    Type = nameof(Reservation),
                    id = Guid.NewGuid().ToString("D")
                },
                new Reservation
                {
                    ReservationId = reservationId,
                    Name = "Kiev",
                    From = DateTime.Today.AddDays(21),
                    To = DateTime.Today.AddDays(26),
                    State = 0,
                    Type = nameof(Reservation),
                    id = Guid.NewGuid().ToString("D")
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Jan",
                    PassportNumber = "NW23191WD",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Angelique",
                    PassportNumber = "TW228391YC",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Sven",
                    PassportNumber = "YR726387IU",
                    Type = nameof(Traveller),
                    id = Guid.NewGuid().ToString("D")
                },
                new Traveller
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
