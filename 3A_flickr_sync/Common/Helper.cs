using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        /// Converts a string, representing a unix timestamp number into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="timestamp">The timestamp, as a string.</param>
        /// <returns>The <see cref="DateTime"/> object the time represents.</returns>
        public static DateTime UnixTimestampToDate(string timestamp)
        {
            if (String.IsNullOrEmpty(timestamp)) return DateTime.MinValue;
            try
            {
                return UnixTimestampToDate(Int64.Parse(timestamp, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo));
            }
            catch (FormatException)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Converts a <see cref="long"/>, representing a unix timestamp number into a <see cref="DateTime"/> object.
        /// </summary>
        /// <param name="timestamp">The unix timestamp.</param>
        /// <returns>The <see cref="DateTime"/> object the time represents.</returns>
        public static DateTime UnixTimestampToDate(long timestamp)
        {
            return unixStartDate.AddSeconds(timestamp);
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

        public static string HashFile(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(path))
                {
                    return md5.ComputeHash(stream).ToReadableString();
                }
            }
        }

        public static string HashPhotoNoExif(string path)
        {
            using (var md5 = MD5.Create())
            {
                using (var image = Image.FromFile(path))
                using (var output = new MemoryStream())
                {
                    image.Save(output, ImageFormat.Bmp);

                    return md5.ComputeHash(output).ToReadableString();
                }
            }
        }

        private const string PhotoUrlFormat = "http://farm{0}.staticflickr.com/{1}/{2}_{3}{4}.{5}";

        internal static string UrlFormat(Photo p, string size, string extension)
        {
            if (size == "_o" || size == "original")
                return UrlFormat(p.Farm, p.Server, p.PhotoId, p.OriginalSecret, size, extension);
            else
                return UrlFormat(p.Farm, p.Server, p.PhotoId, p.Secret, size, extension);
        }

        internal static string UrlFormat(PhotoInfo p, string size, string extension)
        {
            if (size == "_o" || size == "original")
                return UrlFormat(p.Farm, p.Server, p.PhotoId, p.OriginalSecret, size, extension);
            else
                return UrlFormat(p.Farm, p.Server, p.PhotoId, p.Secret, size, extension);
        }

        internal static string UrlFormat(Photoset p, string size, string extension)
        {
            return UrlFormat(p.Farm, p.Server, p.PrimaryPhotoId, p.Secret, size, extension);
        }

        internal static string UrlFormat(string farm, string server, string photoid, string secret, string size, string extension)
        {
            switch (size)
            {
                case "square":
                    size = "_s";
                    break;
                case "thumbnail":
                    size = "_t";
                    break;
                case "small":
                    size = "_m";
                    break;
                case "medium":
                    size = String.Empty;
                    break;
                case "large":
                    size = "_b";
                    break;
                case "original":
                    size = "_o";
                    break;
            }

            return UrlFormat(PhotoUrlFormat, farm, server, photoid, secret, size, extension);
        }

        private static string UrlFormat(string format, params object[] parameters)
        {
            return String.Format(System.Globalization.CultureInfo.InvariantCulture, format, parameters);
        }
        /// <summary>
        /// Parses a date which may contain only a vald year component.
        /// </summary>
        /// <param name="date">The date, as a string, to be parsed.</param>
        /// <returns>The parsed <see cref="DateTime"/>.</returns>
        public static DateTime ParseDateWithGranularity(string date)
        {
            DateTime output = DateTime.MinValue;

            if (String.IsNullOrEmpty(date)) return output;
            if (date == "0000-00-00 00:00:00") return output;
            if (date.EndsWith("-00-01 00:00:00"))
            {
                output = new DateTime(int.Parse(date.Substring(0, 4), System.Globalization.NumberFormatInfo.InvariantInfo), 1, 1);
                return output;
            }

            string format = "yyyy-MM-dd HH:mm:ss";
            try
            {
                output = DateTime.ParseExact(date, format, System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.None);
            }
            catch (FormatException)
            {
#if DEBUG
                throw;
#endif
            }
            return output;
        }

        /// <summary>
        /// Returns the buddy icon for a given set of server, farm and userid. If no server is present then returns the standard buddy icon.
        /// </summary>
        /// <param name="server"></param>
        /// <param name="farm"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static string BuddyIcon(string server, string farm, string userId)
        {
            if (String.IsNullOrEmpty(server) || server == "0")
                return "http://www.flickr.com/images/buddyicon.jpg";
            else
                return String.Format(System.Globalization.CultureInfo.InvariantCulture, "http://farm{0}.staticflickr.com/{1}/buddyicons/{2}.jpg", farm, server, userId);
        }

        /// <summary>
        /// Utility method to convert the <see cref="PhotoSearchExtras"/> enum to a string.
        /// </summary>
        /// <example>
        /// <code>
        ///     PhotoSearchExtras extras = PhotoSearchExtras.DateTaken &amp; PhotoSearchExtras.IconServer;
        ///     string val = Utils.ExtrasToString(extras);
        ///     Console.WriteLine(val);
        /// </code>
        /// outputs: "date_taken,icon_server";
        /// </example>
        /// <param name="extras"></param>
        /// <returns></returns>
        public static string ExtrasToString(PhotoSearchExtras extras)
        {
            List<string> extraList = new List<string>();

            if ((extras & PhotoSearchExtras.DateTaken) == PhotoSearchExtras.DateTaken) extraList.Add("date_taken");
            if ((extras & PhotoSearchExtras.DateUploaded) == PhotoSearchExtras.DateUploaded) extraList.Add("date_upload");
            if ((extras & PhotoSearchExtras.IconServer) == PhotoSearchExtras.IconServer) extraList.Add("icon_server");
            if ((extras & PhotoSearchExtras.License) == PhotoSearchExtras.License) extraList.Add("license");
            if ((extras & PhotoSearchExtras.OwnerName) == PhotoSearchExtras.OwnerName) extraList.Add("owner_name");
            if ((extras & PhotoSearchExtras.OriginalFormat) == PhotoSearchExtras.OriginalFormat) extraList.Add("original_format");
            if ((extras & PhotoSearchExtras.LastUpdated) == PhotoSearchExtras.LastUpdated) extraList.Add("last_update");
            if ((extras & PhotoSearchExtras.Tags) == PhotoSearchExtras.Tags) extraList.Add("tags");
            if ((extras & PhotoSearchExtras.Geo) == PhotoSearchExtras.Geo) extraList.Add("geo");
            if ((extras & PhotoSearchExtras.MachineTags) == PhotoSearchExtras.MachineTags) extraList.Add("machine_tags");
            if ((extras & PhotoSearchExtras.OriginalDimensions) == PhotoSearchExtras.OriginalDimensions) extraList.Add("o_dims");
            if ((extras & PhotoSearchExtras.Views) == PhotoSearchExtras.Views) extraList.Add("views");
            if ((extras & PhotoSearchExtras.Media) == PhotoSearchExtras.Media) extraList.Add("media");
            if ((extras & PhotoSearchExtras.PathAlias) == PhotoSearchExtras.PathAlias) extraList.Add("path_alias");
            if ((extras & PhotoSearchExtras.SquareUrl) == PhotoSearchExtras.SquareUrl) extraList.Add("url_sq");
            if ((extras & PhotoSearchExtras.ThumbnailUrl) == PhotoSearchExtras.ThumbnailUrl) extraList.Add("url_t");
            if ((extras & PhotoSearchExtras.SmallUrl) == PhotoSearchExtras.SmallUrl) extraList.Add("url_s");
            if ((extras & PhotoSearchExtras.MediumUrl) == PhotoSearchExtras.MediumUrl) extraList.Add("url_m");
            if ((extras & PhotoSearchExtras.Medium640Url) == PhotoSearchExtras.Medium640Url) extraList.Add("url_z");
            if ((extras & PhotoSearchExtras.LargeSquareUrl) == PhotoSearchExtras.LargeSquareUrl) extraList.Add("url_q");
            if ((extras & PhotoSearchExtras.Small320Url) == PhotoSearchExtras.Small320Url) extraList.Add("url_n");
            if ((extras & PhotoSearchExtras.LargeUrl) == PhotoSearchExtras.LargeUrl) extraList.Add("url_l");
            if ((extras & PhotoSearchExtras.OriginalUrl) == PhotoSearchExtras.OriginalUrl) extraList.Add("url_o");
            if ((extras & PhotoSearchExtras.Description) == PhotoSearchExtras.Description) extraList.Add("description");
            if ((extras & PhotoSearchExtras.Usage) == PhotoSearchExtras.Usage) extraList.Add("usage");
            if ((extras & PhotoSearchExtras.Visibility) == PhotoSearchExtras.Visibility) extraList.Add("visibility");
            if ((extras & PhotoSearchExtras.Rotation) == PhotoSearchExtras.Rotation) extraList.Add("rotation");
            if ((extras & PhotoSearchExtras.Large1600Url) == PhotoSearchExtras.Large1600Url) extraList.Add("url_h");
            if ((extras & PhotoSearchExtras.Large2048Url) == PhotoSearchExtras.Large2048Url) extraList.Add("url_k");
            if ((extras & PhotoSearchExtras.Medium800Url) == PhotoSearchExtras.Medium800Url) extraList.Add("url_c");

            return String.Join(",", extraList.ToArray());
        }


        /// <summary>
        /// Converts a <see cref="PhotoSearchSortOrder"/> into a string for use by the Flickr API.
        /// </summary>
        /// <param name="order">The sort order to convert.</param>
        /// <returns>The string representative for the sort order.</returns>
        public static string SortOrderToString(PhotoSearchSortOrder order)
        {
            switch (order)
            {
                case PhotoSearchSortOrder.DatePostedAscending:
                    return "date-posted-asc";
                case PhotoSearchSortOrder.DatePostedDescending:
                    return "date-posted-desc";
                case PhotoSearchSortOrder.DateTakenAscending:
                    return "date-taken-asc";
                case PhotoSearchSortOrder.DateTakenDescending:
                    return "date-taken-desc";
                case PhotoSearchSortOrder.InterestingnessAscending:
                    return "interestingness-asc";
                case PhotoSearchSortOrder.InterestingnessDescending:
                    return "interestingness-desc";
                case PhotoSearchSortOrder.Relevance:
                    return "relevance";
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Converts a <see cref="PopularitySort"/> enum to a string.
        /// </summary>
        /// <param name="sortOrder">The value to convert.</param>
        /// <returns></returns>
        public static string SortOrderToString(PopularitySort sortOrder)
        {
            switch (sortOrder)
            {
                case PopularitySort.Comments:
                    return "comments";
                case PopularitySort.Favorites:
                    return "favorites";
                case PopularitySort.Views:
                    return "views";
                default:
                    return String.Empty;
            }
        }

        internal static string DateToMySql(DateTime date)
        {
            return date.ToString("yyyy-MM-dd HH:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo);
        }

        /// <summary>
        /// Convert a <see cref="MachineTagMode"/> to a string used when passing to Flickr.
        /// </summary>
        /// <param name="machineTagMode">The machine tag mode to convert.</param>
        /// <returns>The string to pass to Flickr.</returns>
        public static string MachineTagModeToString(MachineTagMode machineTagMode)
        {
            switch (machineTagMode)
            {
                case MachineTagMode.None:
                    return String.Empty;
                case MachineTagMode.AllTags:
                    return "all";
                case MachineTagMode.AnyTag:
                    return "any";
                default:
                    return String.Empty;
            }

        }

        /// <summary>
        /// Convert a <see cref="TagMode"/> to a string used when passing to Flickr.
        /// </summary>
        /// <param name="tagMode">The tag mode to convert.</param>
        /// <returns>The string to pass to Flickr.</returns>
        public static string TagModeToString(TagMode tagMode)
        {
            switch (tagMode)
            {
                case TagMode.None:
                    return String.Empty;
                case TagMode.AllTags:
                    return "all";
                case TagMode.AnyTag:
                    return "any";
                case TagMode.Boolean:
                    return "bool";
                default:
                    return String.Empty;
            }
        }

        /// <summary>
        /// Converts a <see cref="MediaType"/> enumeration into a string used by Flickr.
        /// </summary>
        /// <param name="mediaType">The <see cref="MediaType"/> value to convert.</param>
        /// <returns></returns>
        public static string MediaTypeToString(MediaType mediaType)
        {
            switch (mediaType)
            {
                case MediaType.All:
                    return "all";
                case MediaType.Photos:
                    return "photos";
                case MediaType.Videos:
                    return "videos";
                default:
                    return String.Empty;
            }
        }
    }

    
}
