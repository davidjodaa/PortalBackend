using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IClientFactory
    {
        Task<SampleResponse> PostDataAsync<SampleResponse, SampleRequest>(string endPoint, SampleRequest dto, bool isSensitive = false);
        Task<SampleResponse> PostDataAsync<SampleResponse, SampleRequest>(string endPoint, SampleRequest dto, string authorizationHeader, bool isSensitive = false);
        Task<SampleResponse> PostDataWithCsrfAsync<SampleResponse, SampleRequest>(string endPoint, string tokenEndpoint, SampleRequest dto, bool isSensitive = false);
        Task<SampleResponse> GetDataAsync<SampleResponse>(string endPoint);
    }
}
