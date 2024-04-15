using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.Contract
{
    public interface IAppSessionService
    {
        Task<bool> CreateSession(string sessionId, string username);
        bool DeleteSession(string sessionId, string username);
        bool DeleteExistingSession(string username);
        bool ValidateSessionExists(string username);
        bool ValidateSession(string sessionId, string username);
    }
}
