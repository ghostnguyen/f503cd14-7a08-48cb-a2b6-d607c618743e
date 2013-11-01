using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    public static partial class FlickrResponder
    {
        /// <summary>
        /// Gets a data response for the given base url and parameters, 
        /// either using OAuth or not depending on which parameters were passed in.
        /// </summary>
        /// <param name="flickr">The current instance of the <see cref="Flickr"/> class.</param>
        /// <param name="baseUrl">The base url to be called.</param>
        /// <param name="parameters">A dictionary of parameters.</param>
        /// <returns></returns>
        public static string GetDataResponse(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
        {
            bool oAuth = parameters.ContainsKey("oauth_consumer_key");

            if (oAuth)
                return GetDataResponseOAuth(flickr, baseUrl, parameters);
            else
                return GetDataResponseNormal(flickr, baseUrl, parameters);
        }

        private static string GetDataResponseNormal(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
        {
            string method = "POST";

            string data = String.Empty;

            foreach (var k in parameters)
            {
                data += k.Key + "=" + Uri.EscapeDataString(k.Value) + "&";
            }

            if (method == "GET" && data.Length > 2000) method = "POST";

            if (method == "GET")
                return DownloadData(method, baseUrl + "?" + data, null, null, null);
            else
                return DownloadData(method, baseUrl, data, PostContentType, null);
        }

        private static string GetDataResponseOAuth(Flickr flickr, string baseUrl, Dictionary<string, string> parameters)
        {
            string method = "POST";

            // Remove api key if it exists.
            if (parameters.ContainsKey("api_key")) parameters.Remove("api_key");
            if (parameters.ContainsKey("api_sig")) parameters.Remove("api_sig");

            if (!String.IsNullOrEmpty(Flickr.OAuthAccessTokenSecret) && !parameters.ContainsKey("oauth_signature"))
            {
                string sig = flickr.OAuthCalculateSignature(method, baseUrl, parameters, Flickr.OAuthAccessTokenSecret);
                parameters.Add("oauth_signature", sig);
            }

            // Calculate post data, content header and auth header
            string data = OAuthCalculatePostData(parameters);
            string authHeader = OAuthCalculateAuthHeader(parameters);

            // Download data.
            try
            {
                return DownloadData(method, baseUrl, data, PostContentType, authHeader);
            }
            catch (WebException ex)
            {
                if (ex.Status != WebExceptionStatus.ProtocolError) throw;

                var response = ex.Response as HttpWebResponse;
                if (response == null) throw;

                string responseData = null;

                using (var stream = response.GetResponseStream())
                {
                    if (stream != null)
                        using (var responseReader = new StreamReader(stream))
                        {
                            responseData = responseReader.ReadToEnd();
                            responseReader.Close();
                        }
                }
                if (response.StatusCode == HttpStatusCode.BadRequest ||
                    response.StatusCode == HttpStatusCode.Unauthorized)
                {

                    throw new OAuthException(responseData, ex);
                }

                if (String.IsNullOrEmpty(responseData)) throw;
                throw new WebException("WebException occurred with the following body content: " + responseData, ex, ex.Status, ex.Response);
            }
        }



        private static string DownloadData(string method, string baseUrl, string data, string contentType, string authHeader)
        {
            using (WebClient client = new WebClient())
            {
                client.Encoding = System.Text.Encoding.UTF8;
                if (!String.IsNullOrEmpty(contentType)) client.Headers.Add("Content-Type", contentType);
                if (!String.IsNullOrEmpty(authHeader)) client.Headers.Add("Authorization", authHeader);

                if (method == "POST")
                    return client.UploadString(baseUrl, data);
                else
                    return client.DownloadString(baseUrl);
            }
        }
    }
}
