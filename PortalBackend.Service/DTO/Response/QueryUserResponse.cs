using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PortalBackend.Service.DTO.Response
{
    public class QueryUserResponse
    {
        public string response_code { get; set; }
        public string response_message { get; set; }
        public string response_time { get; set; }
        public QueryUserResponseData response_data { get; set; }
    }

    public class QueryUserResponseData
    {
        public string user_id { get; set; }
        public string customer_id { get; set; }
        public string user_name { get; set; }
        public string user_role { get; set; }
        public string user_status { get; set; }
        public string mobile { get; set; }
        public string email { get; set; }
    }
}
