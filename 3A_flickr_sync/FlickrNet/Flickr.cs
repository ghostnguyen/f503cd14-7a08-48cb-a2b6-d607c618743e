using System;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Collections.Generic;
using _3A_flickr_sync.Common;
using _3A_flickr_sync.Logic;

namespace _3A_flickr_sync.FlickrNet
{
    /// <summary>
    /// The main Flickr class.
    /// </summary>
    /// <remarks>
    /// Create an instance of this class and then call its methods to perform methods on Flickr.
    /// </remarks>
    /// <example>
    /// <code>
    /// FlickrNet.Flickr flickr = new FlickrNet.Flickr();
    /// User user = flickr.PeopleFindByEmail("cal@iamcal.com");
    /// Console.WriteLine("User Id is " + u.UserId);
    /// </code>
    /// </example>
    // [System.Net.WebPermission(System.Security.Permissions.SecurityAction.Demand, ConnectPattern="http://www.flickr.com/.*")]
    public partial class Flickr
    {
        /// <summary>
        /// 
        /// </summary>
        /// <summary>
        /// UploadProgressHandler is fired during a synchronous upload process to signify that 
        /// a segment of uploading has been completed. This is approximately 50 bytes. The total
        /// uploaded is recorded in the <see cref="UploadProgressEventArgs"/> class.
        /// </summary>
        public event EventHandler<UploadProgressEventArgs> OnUploadProgress;

        private static SupportedService defaultService = SupportedService.Flickr;

        /// <summary>
        /// The base URL for all Flickr REST method calls.
        /// </summary>
        public Uri BaseUri
        {
            get { return baseUri; }
        }

        //private readonly Uri baseUri = new Uri("http://api.flickr.com/services/rest/");
        private readonly Uri baseUri = new Uri("https://secure.flickr.com/services/rest/");

        private string UploadUrl
        {
            get { return uploadUrl; }
        }

        //private static string uploadUrl = "http://api.flickr.com/services/upload/";
        private static string uploadUrl = "https://secure.flickr.com/services/upload/";

        private string ReplaceUrl
        {
            get { return replaceUrl; }
        }
        //private static string replaceUrl = "http://api.flickr.com/services/replace/";
        private static string replaceUrl = "https://secure.flickr.com/services/replace/";

        private string AuthUrl
        {
            get { return authUrl; }
        }
        //private static string authUrl = "http://www.flickr.com/services/auth/";
        private static string authUrl = "https://www.flickr.com/services/auth/";

        public readonly static string ApiKey = "5022cacde5b7dee7f9419711f35223b8";

        public readonly static string ApiSecret = "b33e1a569c1f8d2a";

        private int timeout = 100000;

        /// <summary>
        /// User Agent string sent web calls to Flickr.
        /// </summary>
        //public const string UserAgent = "Mozilla/4.0 FlickrNet API (compatible; MSIE 6.0; Windows NT 5.1)";

        /// <summary>
        /// OAuth Access Token. Needed for authenticated access using OAuth to Flickr.
        /// </summary>
        public static string OAuthAccessToken { get; set; }

        /// <summary>
        /// OAuth Access Token Secret. Needed for authenticated access using OAuth to Flickr.
        /// </summary>
        public static string OAuthAccessTokenSecret { get; set; }

        //public static string UserId { get; set; }
        public static FoundUser User { get; set; }
        /// <summary>
        /// The default service to use for new Flickr instances
        /// </summary>
        public static SupportedService DefaultService
        {
            get
            {
                return defaultService;
            }
        }

        /// <summary>
        /// Internal timeout for all web requests in milliseconds. Defaults to 30 seconds.
        /// </summary>
        public int HttpTimeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Returns the raw XML returned from the last response.
        /// Only set it the response was not returned from cache.
        /// </summary>
        //public string LastResponse
        //{
        //    get { return lastResponse; }
        //}

        /// <summary>
        /// Returns the last URL requested. Includes API signing.
        /// </summary>
        //public string LastRequest
        //{
        //    get { return lastRequest; }
        //}

        /// <summary>
        /// Constructor loads configuration settings from app.config or web.config file if they exist.
        /// </summary>
        public Flickr()
        {
            
        }

        public static void ResetOAuth()
        {
            FUserLogic l = new FUserLogic();
            var v = l.GetFirst();
            if (v != null)
            {
                OAuthAccessToken = v.OAuthAccessToken;
                OAuthAccessTokenSecret = v.OAuthAccessTokenSecret;

                SetLogic setL = new SetLogic();
                setL.DownloadPhotsets();

                Flickr flickr = new Flickr();
                User = flickr.TestLogin();
            }
        }

        private void CheckRequiresAuthentication()
        {
            if (String.IsNullOrEmpty(OAuthAccessToken) || String.IsNullOrEmpty(OAuthAccessTokenSecret))
            {
                ResetOAuth();
                if (String.IsNullOrEmpty(OAuthAccessToken) || String.IsNullOrEmpty(OAuthAccessTokenSecret))
                {
                    throw new Exception(ErrMess.Err20);
                }
            }
             

            if (!String.IsNullOrEmpty(OAuthAccessToken) && !String.IsNullOrEmpty(OAuthAccessTokenSecret))
                return;

            if (String.IsNullOrEmpty(ApiSecret))
                throw new Exception(ErrMess.Err20);            
        }

        /// <summary>
        /// Calculates the Flickr method cal URL based on the passed in parameters, and also generates the signature if required.
        /// </summary>
        /// <param name="parameters">A Dictionary containing a list of parameters to add to the method call.</param>
        /// <param name="includeSignature">Boolean use to decide whether to generate the api call signature as well.</param>
        /// <returns>The <see cref="Uri"/> for the method call.</returns>
        public Uri CalculateUri(Dictionary<string, string> parameters)
        {
            StringBuilder url = new StringBuilder();
            url.Append("?");
            foreach (KeyValuePair<string, string> pair in parameters)
            {
                url.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "{0}={1}&", pair.Key, Uri.EscapeDataString(pair.Value ?? ""));
            }

            return new Uri(BaseUri, new Uri(url.ToString(), UriKind.Relative));
        }

        private byte[] ConvertNonSeekableStreamToByteArray(Stream nonSeekableStream)
        {
            MemoryStream ms = new MemoryStream();
            byte[] buffer = new byte[1024];
            int bytes;
            while ((bytes = nonSeekableStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, bytes);
            }
            byte[] output = ms.ToArray();
            return output;
        }

        private byte[] CreateUploadData(Stream imageStream, string fileName, Dictionary<string, string> parameters, string boundary)
        {
            bool oAuth = parameters.ContainsKey("oauth_consumer_key");

            string[] keys = new string[parameters.Keys.Count];
            parameters.Keys.CopyTo(keys, 0);
            Array.Sort(keys);

            StringBuilder hashStringBuilder = new StringBuilder(ApiSecret, 2 * 1024);
            StringBuilder contentStringBuilder = new StringBuilder();

            foreach (string key in keys)
            {
                if (key.StartsWith("oauth")) continue;

                hashStringBuilder.Append(key);
                hashStringBuilder.Append(parameters[key]);
                contentStringBuilder.Append("--" + boundary + "\r\n");
                contentStringBuilder.Append("Content-Disposition: form-data; name=\"" + key + "\"\r\n");
                contentStringBuilder.Append("\r\n");
                contentStringBuilder.Append(parameters[key] + "\r\n");
            }

            // Photo
            contentStringBuilder.Append("--" + boundary + "\r\n");
            contentStringBuilder.Append("Content-Disposition: form-data; name=\"photo\"; filename=\"" + Path.GetFileName(fileName) + "\"\r\n");
            contentStringBuilder.Append("Content-Type: image/jpeg\r\n");
            contentStringBuilder.Append("\r\n");

            UTF8Encoding encoding = new UTF8Encoding();

            byte[] postContents = encoding.GetBytes(contentStringBuilder.ToString());

            byte[] photoContents = ConvertNonSeekableStreamToByteArray(imageStream);

            byte[] postFooter = encoding.GetBytes("\r\n--" + boundary + "--\r\n");

            byte[] dataBuffer = new byte[postContents.Length + photoContents.Length + postFooter.Length];

            Buffer.BlockCopy(postContents, 0, dataBuffer, 0, postContents.Length);
            Buffer.BlockCopy(photoContents, 0, dataBuffer, postContents.Length, photoContents.Length);
            Buffer.BlockCopy(postFooter, 0, dataBuffer, postContents.Length + photoContents.Length, postFooter.Length);

            return dataBuffer;
        }
    }
}
