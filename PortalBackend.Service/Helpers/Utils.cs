using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.Helpers
{
    public static class Utils
    {
        public static string GenerateRandomCharacters(int size)
        {
            Random random = new Random();

            const string chars = "QWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm0123456789";
            string builder = new string(Enumerable.Repeat(chars, size)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return builder;
        }

        public static string ConvertToBase64DataUrl(byte[] request, string extension)
        {
            if (extension == ".pdf" && request != null)
            {
                return $"data:application/{extension.Replace(".", "").Trim()};base64,{Convert.ToBase64String(request)}";
            }
            else if ((extension == ".jpg" || extension == ".png") && request != null)
            {
                return $"data:image/{extension.Replace(".", "").Trim()};base64,{Convert.ToBase64String(request)}";
            }
            else
            {
                return default;
            }
        }
#pragma warning disable CA1416

        public static string GetAdPropertyValue(this SearchResult sr, string propertyName)
        {
            string ret = string.Empty;

            if (sr.Properties[propertyName].Count > 0)
                ret = sr.Properties[propertyName][0].ToString();

            return ret;
        }

        public static List<string> GetAdPropertyArray(this SearchResult sr, string propertyName)
        {
            List<string> response = new List<string>();

            if (sr.Properties[propertyName].Count > 0)
            {
                foreach (var property in sr.Properties[propertyName])
                {
                    response.Add(property.ToString());
                }
            }

            return response;
        }
    }
}
