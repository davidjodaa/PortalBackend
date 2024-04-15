using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface ITemplateService
    {
        string GenerateHtmlStringFromViewsAsync<T>(string viewName, T model);
    }
}
