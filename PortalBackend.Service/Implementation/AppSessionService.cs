using PortalBackend.Domain.Entities;
using PortalBackend.Domain.Settings;
using PortalBackend.Persistence;
using PortalBackend.Service.Contract;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace PortalBackend.Service.Implementation
{
    public class AppSessionService : IAppSessionService
    {
        private readonly IApplicationDbContext _context;
        private readonly JWTSettings _jwtSettings;
        public AppSessionService(IApplicationDbContext context,
            IOptions<JWTSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
            _context = context;
        }

        public async Task<bool> CreateSession(string sessionId, string username)
        {
            AppSession userHasSession = _context.AppSession.FirstOrDefault(s => s.Username == username);
            if (userHasSession != null) _context.AppSession.Remove(userHasSession);
            await _context.AppSession.AddAsync(new AppSession { Session = sessionId, Username = username, DateCreated = DateTime.Now });

            return true;
        }

        public bool DeleteSession(string sessionId, string username)
        {
            AppSession session = _context.AppSession.FirstOrDefault(s => s.Session == sessionId && s.Username == username);

            if (session != null)
            {
                _context.AppSession.Remove(session);
            }

            return true;
        }

        public bool DeleteExistingSession(string username)
        {
            AppSession session = _context.AppSession.FirstOrDefault(s => s.Username == username);

            if (session != null)
            {
                _context.AppSession.Remove(session);
            }

            return true;
        }

        public bool ValidateSessionExists(string username)
        {
            var existingSession = _context.AppSession.FirstOrDefault(s => s.Username == username && s.DateCreated.AddMinutes(_jwtSettings.DurationInMinutes) >= DateTime.Now);

            if (existingSession != null)
            {
                return true;
            }
            return false;
        }

        public bool ValidateSession(string sessionId, string username)
        {
            var sessionExists = _context.AppSession.FirstOrDefault(s => s.Session == sessionId && s.Username == username) != null;

            if (sessionExists)
            {
                return true;
            }
            return false;
        }
    }
}
