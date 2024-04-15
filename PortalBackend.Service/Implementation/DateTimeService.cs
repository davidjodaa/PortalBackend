using PortalBackend.Service.Contract;
using System;

namespace PortalBackend.Service.Implementation
{
    public class DateTimeService : IDateTimeService
    {
        public DateTime NowUtc => DateTime.UtcNow;
    }
}