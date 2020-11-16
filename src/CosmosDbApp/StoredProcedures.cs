using System;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using CosmosDbApp.Models;
using CosmosDbApp.Models.Write;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Scripts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CosmosDbApp
{
    public class StoredProcedures
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<StoredProcedures> logger;

        private const string StoredProcedureId = "spCreateReservations";

        public StoredProcedures(
            IConfiguration configuration,
            ILogger<StoredProcedures> logger)
        {
            this.configuration = configuration;
            this.logger = logger;
        }

        [FunctionName(nameof(AddStoredProcedures))]
        public async Task<IActionResult> AddStoredProcedures(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)]
            HttpRequest req)
        {
            var endpoint = this.configuration["CosmosDb:Endpoint"];
            var authKey = this.configuration["CosmosDb:Key"];
            var databaseName = this.configuration["CosmosDb:Database"];
            var containerName = this.configuration["CosmosDb:Container"];
            using (CosmosClient client = new CosmosClient(endpoint, authKey))
            {
                var container = client.GetContainer(databaseName, containerName);
                await RegisterStoredProcedure(container);
            }
            return new OkObjectResult(default);
        }

        [FunctionName(nameof(InvokeStoredProcedure))]
        public async Task<IActionResult> InvokeStoredProcedure(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] 
            HttpRequest req)
        {
            var endpoint = this.configuration["CosmosDb:Endpoint"];
            var authKey = this.configuration["CosmosDb:Key"];
            var databaseName = this.configuration["CosmosDb:Database"];
            var containerName = this.configuration["CosmosDb:Container"];
            using (CosmosClient client = new CosmosClient(endpoint, authKey))
            {
                var container = client.GetContainer(databaseName, containerName);
                await CreateNewItems(container);
            }
            return new OkObjectResult(default);
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

        private async Task CreateNewItems(Container container)
        {
            var reservationId = new Random().Next(1, 500);
            dynamic[] reservations = {
                new Reservation
                {
                    Id = reservationId,
                    Name = "Prague",
                    From = DateTime.Today.AddDays(7),
                    To = DateTime.Today.AddDays(8),
                    State = 0
                },
                new Reservation 
                {
                    Id = reservationId,
                    Name = "Kiev",
                    From = DateTime.Today.AddDays(21),
                    To = DateTime.Today.AddDays(26),
                    State = 0,
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Jan",
                    PassportNumber = "NW23191WD"
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Angelique",
                    PassportNumber = "TW228391YC"
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Sven",
                    PassportNumber = "YR726387IU"
                },
                new Traveller
                {
                    ReservationId = reservationId,
                    Name = "Fleur",
                    PassportNumber = "YR726388IU"
                }
            };
            try
            {
                var result = await container.Scripts.ExecuteStoredProcedureAsync<string>(
                    StoredProcedureId,
                    new PartitionKey(reservationId),
                    reservations);

                var serializedResult = JsonConvert.SerializeObject(result);
                this.logger.LogInformation(serializedResult);
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

    }
}
