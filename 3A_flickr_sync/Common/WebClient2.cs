using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.Common
{
    public class WebClient2 : WebClient
    {

        public int Timeout { get; set; }
        public string ContentType { get; set; }
        public long ContentLength { get; set; }
        //public WebClient2()
        //    : base()
        //{

        //}

        protected override WebRequest GetWebRequest(Uri address)
        {
            var objWebRequest = base.GetWebRequest(address);
            objWebRequest.Timeout = Timeout;
            objWebRequest.ContentType = ContentType;
            objWebRequest.ContentLength = ContentLength;
            return objWebRequest;
        }
    }
}
