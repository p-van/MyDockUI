using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MyDockUI.Controllers
{
    [Controller]
    [Route("information")]
    public class InformationController : ControllerBase
    {
        private readonly ILogger<InformationController> _logger;
        private readonly ApiClientConfiguration _clientConfiguration;

        public InformationController(IOptions<ApiClientConfiguration> clientConfiguration, ILogger<InformationController> logger)
        {
            _logger = logger;
            _clientConfiguration = clientConfiguration.Value;
        }
        
        [HttpGet]
        public async Task<ActionResult> GetInformation()
        {
            var builder = new UriBuilder(_clientConfiguration.Scheme, _clientConfiguration.Host, _clientConfiguration.Port);
            var clientBaseAddress = builder.Uri;
            
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) =>
            {
                return true;
            };
            
            
            var client = new HttpClient(clientHandler);
            
            client.BaseAddress = clientBaseAddress;
            
            
            _logger.LogInformation($"Trying to connect to {clientBaseAddress.AbsoluteUri}");
            
            using (client)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, "api/information");
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return BadRequest("Error calling API");
                }

                var responseMessage = await response.Content.ReadAsStringAsync();
                return Ok($"Recieved information: {responseMessage}");
            }
        }
    }
    
    [ApiController]
    [Route("health")]
    public class HealthCheckController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult> Ping()
        {
            await Task.CompletedTask;
            var response = $"UI here";
            return Ok(response);
        }
    }
}