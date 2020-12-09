using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ContactDonor.Web.Helpers
{
    public class Response
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public static class StatusTypes
    {
        public const string Success = "SUCCESS";
        public const string Failed = "FAILED";
    }
}
