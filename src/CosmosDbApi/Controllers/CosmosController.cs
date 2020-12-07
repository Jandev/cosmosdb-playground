using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbApp;

namespace CosmosDbApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CosmosController : ControllerBase
    {
        private readonly CosmosService cosmosService;
        private readonly ILogger<CosmosController> _logger;

        public CosmosController(
            CosmosService cosmosService,
            ILogger<CosmosController> logger)
        {
            this.cosmosService = cosmosService;
            _logger = logger;
        }

        [HttpGet]
        public async Task Get()
        {
            await cosmosService.InvokePlainCommand();
        }
    }
}
