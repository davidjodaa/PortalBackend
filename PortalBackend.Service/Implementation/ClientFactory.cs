using PortalBackend.Service.Contract;
using PortalBackend.Service.DTO.Response;
using PortalBackend.Service.Exceptions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class ClientFactory : IClientFactory
    {
        private readonly ILogger<ClientFactory> _logger;

        public ClientFactory(ILogger<ClientFactory> logger)
        {
            _logger = logger;
        }

        public async Task<SampleResponse> PostDataAsync<SampleResponse, SampleRequest>(string endPoint, SampleRequest dto, bool isSensitive = false)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var client = new HttpClient(handler))
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string requestString = await content.ReadAsStringAsync();
                if (!isSensitive)
                {
                    _logger.LogWarning($"JSON Request for {endPoint}:: {requestString}");
                }

                Stopwatch timer = new Stopwatch();
                timer.Start();

                HttpResponseMessage httpResponse = await client.PostAsync(endPoint, content);
                timer.Stop();
                _logger.LogWarning($"HTTP Response Code for {endPoint}:: {httpResponse.StatusCode} Response Time:: {timer.ElapsedMilliseconds}ms");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogWarning($"JSON Response for {endPoint}:: {jsonString}");
                var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

                return data;
            }
        }

        public async Task<SampleResponse> PostDataAsync<SampleResponse, SampleRequest>(string endPoint, SampleRequest dto, string authorizationHeader, bool isSensitive = false)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var client = new HttpClient(handler))
            {
                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authorizationHeader);

                string requestString = await content.ReadAsStringAsync();
                if (!isSensitive)
                {
                    _logger.LogWarning($"JSON Request for {endPoint}:: {requestString}");
                }

                Stopwatch timer = new Stopwatch();
                timer.Start();

                HttpResponseMessage httpResponse = await client.PostAsync(endPoint, content);
                timer.Stop();
                _logger.LogWarning($"HTTP Response Code for {endPoint}:: {httpResponse.StatusCode} Response Time:: {timer.ElapsedMilliseconds}ms");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogWarning($"JSON Response for {endPoint}:: {jsonString}");
                var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

                return data;
            }
        }

        public async Task<SampleResponse> PostDataWithCsrfAsync<SampleResponse, SampleRequest>(string endPoint, string tokenEndpoint, SampleRequest dto, bool isSensitive = false)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var csrfRequestMessage = new HttpRequestMessage(HttpMethod.Get, tokenEndpoint);
                var csrfResponseMessage = await client.SendAsync(csrfRequestMessage);

                csrfResponseMessage.EnsureSuccessStatusCode();

                string cookie = csrfResponseMessage.Headers.GetValues("Set-Cookie")?.FirstOrDefault()?.Split(';')[0]?.Trim();
                string CsrfTokenJsonString = await csrfResponseMessage.Content.ReadAsStringAsync();
                var CsrfTokenResponse = JsonConvert.DeserializeObject<FetchTokenResponse>(CsrfTokenJsonString);

                HttpContent content = new StringContent(JsonConvert.SerializeObject(dto), Encoding.UTF8, "application/json");
                content.Headers.Add("X-CSRF-Token", CsrfTokenResponse.token);
                content.Headers.Add("Cookie", cookie);

                string requestString = await content.ReadAsStringAsync();
                if (!isSensitive)
                {
                    _logger.LogWarning($"JSON Request for {endPoint}:: {requestString}");
                }

                Stopwatch timer = new Stopwatch();
                timer.Start();

                HttpResponseMessage httpResponse = await client.PostAsync(endPoint, content);
                timer.Stop();
                _logger.LogWarning($"HTTP Response Code for {endPoint}:: {httpResponse.StatusCode} Response Time:: {timer.ElapsedMilliseconds}ms");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogWarning($"JSON Response for {endPoint}:: {jsonString}");
                var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

                return data;
            }
        }

        public async Task<SampleResponse> GetDataAsync<SampleResponse>(string endPoint)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            using (var client = new HttpClient(handler))
            {

                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Stopwatch timer = new Stopwatch();
                timer.Start();

                HttpResponseMessage httpResponse = await client.GetAsync(endPoint);
                timer.Stop();
                _logger.LogWarning($"HTTP Response Code for {endPoint}:: {httpResponse.StatusCode} Response Time:: {timer.ElapsedMilliseconds}ms");

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                _logger.LogWarning($"JSON Response for {endPoint}:: {jsonString}");
                var data = JsonConvert.DeserializeObject<SampleResponse>(jsonString);

                return data;
            }
        }

    }
}
