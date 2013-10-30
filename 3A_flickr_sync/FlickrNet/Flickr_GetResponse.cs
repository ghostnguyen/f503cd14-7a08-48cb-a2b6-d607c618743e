using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _3A_flickr_sync.FlickrNet
{
    public partial class Flickr
    {

        private T GetResponseNoCache<T>(Dictionary<string, string> parameters) where T : IFlickrParsable, new()
        {
            return GetResponse<T>(parameters);
        }

        private T GetResponse<T>(Dictionary<string, string> parameters) where T : IFlickrParsable, new()
        {
            // Flow for GetResponse.
            // 2. Calculate Cache URL.
            // 3. Check Cache for URL.
            // 4. Get Response if not in cache.
            // 5. Write Cache.
            // 6. Parse Response.

            parameters["api_key"] = ApiKey;

            // If OAuth Token exists or no authentication required then use new OAuth
            if (!String.IsNullOrEmpty(OAuthAccessToken))
            {
                parameters.Remove("api_key");
                OAuthGetBasicParameters(parameters);
                if (!String.IsNullOrEmpty(OAuthAccessToken)) parameters["oauth_token"] = OAuthAccessToken;
            }

            var url = CalculateUri(parameters, !String.IsNullOrEmpty(sharedSecret));

            lastRequest = url.AbsoluteUri;

            string responseXml;



            var urlComplete = url.AbsoluteUri;

            responseXml = FlickrResponder.GetDataResponse(this, BaseUri.AbsoluteUri, parameters);

            lastResponse = responseXml;

            var reader = new XmlTextReader(new StringReader(responseXml))
            {
                WhitespaceHandling = WhitespaceHandling.None
            };

            if (!reader.ReadToDescendant("rsp"))
            {
                throw new XmlException("Unable to find response element 'rsp' in Flickr response");
            }
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "stat" && reader.Value == "fail")
                {
                    //TODO:
                    //throw ExceptionHandler.CreateResponseException(reader);
                }
            }

            reader.MoveToElement();
            reader.Read();

            var item = new T();
            item.Load(reader);

            return item;

        }
    }
}
