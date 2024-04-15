using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Domain.Extension
{
    public static class ExtensionMethods
    {
        public static bool IsValidDate(this string str)
        {
            DateTime validDate;

            return DateTime.TryParse(str, out validDate);
        }
    }
}
