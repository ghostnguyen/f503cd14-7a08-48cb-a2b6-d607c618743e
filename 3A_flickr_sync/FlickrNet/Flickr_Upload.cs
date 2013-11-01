﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using _3A_flickr_sync.Common;

namespace _3A_flickr_sync.FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// UploadPicture method that does all the uploading work.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> object containing the pphoto to be uploaded.</param>
        /// <param name="fileName">The filename of the file to upload. Used as the title if title is null.</param>
        /// <param name="title">The title of the photo (optional).</param>
        /// <param name="description">The description of the photograph (optional).</param>
        /// <param name="tags">The tags for the photograph (optional).</param>
        /// <param name="isPublic">false for private, true for public.</param>
        /// <param name="isFamily">true if visible to family.</param>
        /// <param name="isFriend">true if visible to friends only.</param>
        /// <param name="contentType">The content type of the photo, i.e. Photo, Screenshot or Other.</param>
        /// <param name="safetyLevel">The safety level of the photo, i.e. Safe, Moderate or Restricted.</param>
        /// <param name="hiddenFromSearch">Is the photo hidden from public searches.</param>
        /// <returns>The id of the photograph after successful uploading.</returns>
        public string UploadPicture(string path, string title = "", string description = "", string tags = "", bool isPublic = false, bool isFamily = true, bool isFriend = false, ContentType contentType = ContentType.None, SafetyLevel safetyLevel = SafetyLevel.Restricted, HiddenFromSearch hiddenFromSearch = HiddenFromSearch.Hidden)
        {
            CheckRequiresAuthentication();

            string fileName = Path.GetFileName(path);
            using (Stream stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Uri uploadUri = new Uri(UploadUrl);

                Dictionary<string, string> parameters = new Dictionary<string, string>();

                if (string.IsNullOrEmpty(title))
                { }
                else
                { parameters.Add("title", title); }

                if (string.IsNullOrEmpty(description))
                { }
                else
                { parameters.Add("description", description); }


                if (string.IsNullOrEmpty(tags))
                { }
                else { parameters.Add("tags", tags); }

                parameters.Add("is_public", isPublic ? "1" : "0");
                parameters.Add("is_friend", isFriend ? "1" : "0");
                parameters.Add("is_family", isFamily ? "1" : "0");

                if (safetyLevel != SafetyLevel.None)
                {
                    parameters.Add("safety_level", safetyLevel.ToString("D"));
                }
                if (contentType != ContentType.None)
                {
                    parameters.Add("content_type", contentType.ToString("D"));
                }
                if (hiddenFromSearch != HiddenFromSearch.None)
                {
                    parameters.Add("hidden", hiddenFromSearch.ToString("D"));
                }

                OAuthGetBasicParameters(parameters);

                string sig = OAuthCalculateSignature("POST", uploadUri.AbsoluteUri, parameters, OAuthAccessTokenSecret);
                parameters.Add("oauth_signature", sig);

                string responseXml = UploadData(stream, fileName, uploadUri, parameters);

                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                XmlReader reader = XmlReader.Create(new StringReader(responseXml), settings);

                if (!reader.ReadToDescendant("rsp"))
                {
                    throw new XmlException("Unable to find response element 'rsp' in Flickr response");
                }
                while (reader.MoveToNextAttribute())
                {
                    if (reader.LocalName == "stat" && reader.Value == "fail")
                        throw new Exception();
                    //TODO:
                    //throw ExceptionHandler.CreateResponseException(reader);
                    continue;
                }

                reader.MoveToElement();
                reader.Read();

                UnknownResponse t = new UnknownResponse();
                ((IFlickrParsable)t).Load(reader);

                stream.Close();
                return t.GetElementValue("photoid");
            }
        }

        private string UploadData(Stream imageStream, string fileName, Uri uploadUri, Dictionary<string, string> parameters)
        {
            //string boundary = "FLICKR_MIME_" + DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            //string authHeader = FlickrResponder.OAuthCalculateAuthHeader(parameters);
            //byte[] dataBuffer = CreateUploadData(imageStream, fileName, parameters, boundary);

            //HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(uploadUri);
            //req.Method = "POST";

            //req.Timeout = HttpTimeout;
            //req.ContentType = "multipart/form-data; boundary=" + boundary;
            ////req.Expect = String.Empty;
            //if (!String.IsNullOrEmpty(authHeader))
            //{
            //    req.Headers["Authorization"] = authHeader;
            //}

            //req.ContentLength = dataBuffer.Length;

            //using (Stream reqStream = req.GetRequestStream())
            //{
            //    int bufferSize = 32 * 1024;
            //    if (dataBuffer.Length / 100 > bufferSize) bufferSize = bufferSize * 2;

            //    int uploadedSoFar = 0;

            //    while (uploadedSoFar < dataBuffer.Length)
            //    {
            //        reqStream.Write(dataBuffer, uploadedSoFar, Math.Min(bufferSize, dataBuffer.Length - uploadedSoFar));
            //        uploadedSoFar += bufferSize;

            //        if (OnUploadProgress != null)
            //        {
            //            UploadProgressEventArgs args = new UploadProgressEventArgs(uploadedSoFar, dataBuffer.Length);
            //            OnUploadProgress(this, args);
            //        }
            //    }
            //    reqStream.Close();
            //}

            //HttpWebResponse res = (HttpWebResponse)req.GetResponse();

            //StreamReader sr = new StreamReader(res.GetResponseStream());
            //string s = sr.ReadToEnd();
            //sr.Close();
            //return s;




            string boundary = "FLICKR_MIME_" + DateTime.Now.ToString("yyyyMMddhhmmss", System.Globalization.DateTimeFormatInfo.InvariantInfo);

            string authHeader = FlickrResponder.OAuthCalculateAuthHeader(parameters);
            byte[] dataBuffer = CreateUploadData(imageStream, fileName, parameters, boundary);

            WebClient2 webClient = new WebClient2();

            webClient.Timeout = HttpTimeout;
            webClient.ContentType = "multipart/form-data; boundary=" + boundary;

            if (!String.IsNullOrEmpty(authHeader))
            {
                webClient.Headers["Authorization"] = authHeader;
            }

            webClient.ContentLength = dataBuffer.Length;

            var responseArray = webClient.UploadData(uploadUri, dataBuffer);
            string s = System.Text.Encoding.UTF8.GetString(responseArray);

            return s;
        }

        /// <summary>
        /// Replace an existing photo on Flickr.
        /// </summary>
        /// <param name="fullFileName">The full filename of the photo to upload.</param>
        /// <param name="photoId">The ID of the photo to replace.</param>
        /// <returns>The id of the photograph after successful uploading.</returns>
        public string ReplacePicture(string fullFileName, string photoId)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(fullFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                return ReplacePicture(stream, fullFileName, photoId);
            }
            finally
            {
                if (stream != null) stream.Close();
            }

        }

        /// <summary>
        /// Replace an existing photo on Flickr.
        /// </summary>
        /// <param name="stream">The <see cref="Stream"/> object containing the photo to be uploaded.</param>
        /// <param name="fileName">The filename of the file to replace the existing item with.</param>
        /// <param name="photoId">The ID of the photo to replace.</param>
        /// <returns>The id of the photograph after successful uploading.</returns>
        public string ReplacePicture(Stream stream, string fileName, string photoId)
        {

            Uri replaceUri = new Uri(ReplaceUrl);

            Dictionary<string, string> parameters = new Dictionary<string, string>();

            parameters.Add("photo_id", photoId);

            OAuthGetBasicParameters(parameters);
            parameters.Add("oauth_token", OAuthAccessToken);

            string sig = OAuthCalculateSignature("POST", replaceUri.AbsoluteUri, parameters, OAuthAccessTokenSecret);
            parameters.Add("oauth_signature", sig);

            string responseXml = UploadData(stream, fileName, replaceUri, parameters);

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            XmlReader reader = XmlReader.Create(new StringReader(responseXml), settings);

            if (!reader.ReadToDescendant("rsp"))
            {
                throw new XmlException("Unable to find response element 'rsp' in Flickr response");
            }
            while (reader.MoveToNextAttribute())
            {
                if (reader.LocalName == "stat" && reader.Value == "fail")
                    throw new Exception();
                //TODO:
                //throw ExceptionHandler.CreateResponseException(reader);
                continue;
            }

            reader.MoveToElement();
            reader.Read();

            UnknownResponse t = new UnknownResponse();
            ((IFlickrParsable)t).Load(reader);
            return t.GetElementValue("photoid");
        }
    }
}