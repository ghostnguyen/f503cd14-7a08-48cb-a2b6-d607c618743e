using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using _3A_flickr_sync.FlickrNet;

namespace _3A_flickr_sync.Common
{
    public class Helper
    {

        private static readonly DateTime unixStartDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Generates an MD5 Hash of the passed in string.
        /// </summary>
        /// <param name="data">The unhashed string.</param>
        /// <returns>The MD5 hash string.</returns>
        public static string MD5Hash(string data)
        {
            byte[] hashedBytes;

            using (System.Security.Cryptography.MD5CryptoServiceProvider csp = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(data);
                hashedBytes = csp.ComputeHash(bytes, 0, bytes.Length);
            }

            return BitConverter.ToString(hashedBytes).Replace("-", String.Empty).ToLower(System.Globalization.CultureInfo.InvariantCulture);
        }

        

        /// <summary>
        /// Converts <see cref="AuthLevel"/> to a string.
        /// </summary>
        /// <param name="level">The level to convert.</param>
        /// <returns></returns>
        public static string AuthLevelToString(AuthLevel level)
        {
            switch (level)
            {
                case AuthLevel.Delete:
                    return "delete";
                case AuthLevel.Read:
                    return "read";
                case AuthLevel.Write:
                    return "write";
                case AuthLevel.None:
                    return "none";
                default:
                    return String.Empty;

            }
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> object into a unix timestamp number.
        /// </summary>
        /// <param name="date">The date to convert.</param>
        /// <returns>A long for the number of seconds since 1st January 1970, as per unix specification.</returns>
        public static string DateToUnixTimestamp(DateTime date)
        {
            TimeSpan ts = date - unixStartDate;
            return ts.TotalSeconds.ToString("0", System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Converts a URL parameter encoded string into a dictionary.
        /// </summary>
        /// <remarks>
        /// e.g. ab=cd&amp;ef=gh will return a dictionary of { "ab" => "cd", "ef" => "gh" }.</remarks>
        /// <param name="response"></param>
        /// <returns></returns>
        public static Dictionary<string, string> StringToDictionary(string response)
        {
            var dic = new Dictionary<string, string>();

            if (String.IsNullOrEmpty(response)) return dic;

            var parts = response.Split('&');

            foreach (var part in parts)
            {

                var bits = part.Split(new[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);

                dic.Add(bits[0], bits.Length == 1 ? "" : Uri.UnescapeDataString(bits[1]));
            }

            return dic;
        }

        /// <summary>
        /// If an unknown element is found and the DLL is a debug DLL then a <see cref="ParsingException"/> is thrown.
        /// </summary>
        /// <param name="reader">The <see cref="XmlReader"/> containing the unknown xml node.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void CheckParsingException(XmlReader reader)
        {
            if (reader.NodeType == XmlNodeType.Attribute)
            {
                //TODO:
                //throw new ParsingException("Unknown attribute: " + reader.Name + "=" + reader.Value);
            }
            if (!String.IsNullOrEmpty(reader.Value))
            { }
            //TODO:
            //throw new ParsingException("Unknown " + reader.NodeType.ToString() + ": " + reader.Name + "=" + reader.Value);
            else
            { }
              //TODO:  
            //throw new ParsingException("Unknown element: " + reader.Name);

        }
    }

    public static class dotNetExtension
    {
        public static SortedList<string, string> ToSortedList(this Dictionary<string, string> parameters)
        {
            SortedList<string, string> sorted = new SortedList<string, string>();
            foreach (KeyValuePair<string, string> pair in parameters) { sorted.Add(pair.Key, pair.Value); }
            return sorted;
        }

        /// <summary>
        /// Escapes a string for use with OAuth.
        /// </summary>
        /// <remarks>The only valid characters are Alphanumerics and "-", "_", "." and "~". Everything else is hex encoded.</remarks>
        /// <param name="text">The text to escape.</param>
        /// <returns>The escaped string.</returns>
        public static string ToEscapeOAuth(this string text)
        {
            string value = text;

            value = Uri.EscapeDataString(value).Replace("+", "%20");

            // UrlEncode escapes with lowercase characters (e.g. %2f) but oAuth needs %2F
            value = Regex.Replace(value, "(%[0-9a-f][0-9a-f])", c => c.Value.ToUpper());

            // these characters are not escaped by UrlEncode() but needed to be escaped
            value = value.Replace("(", "%28").Replace(")", "%29").Replace("$", "%24").Replace("!", "%21").Replace(
                "*", "%2A").Replace("'", "%27");

            // these characters are escaped by UrlEncode() but will fail if unescaped!
            value = value.Replace("%7E", "~");

            return value;
        }
    }
}
