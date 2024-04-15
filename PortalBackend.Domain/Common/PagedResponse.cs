using Newtonsoft.Json;
using System;

namespace PortalBackend.Domain.Common
{
    public class PagedResponse<T> : Response<T>
    {
        [JsonProperty("pageIndex")]
        public int PageIndex { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("totalPages")]
        public int TotalPages { get; set; }
        [JsonProperty("total")]
        public int Total { get; set; }
        public PagedResponse(T data, int pageIndex, int pageSize, int total, string message)
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
            this.Total = total;
            this.TotalPages = (total + pageSize - 1) / pageSize;
            this.Succeeded = true;
            this.Code = 200;
            this.Message = message;
            this.Data = data;
            this.Errors = null;
        }
    }
}
