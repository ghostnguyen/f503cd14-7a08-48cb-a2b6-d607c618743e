using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _3A_flickr_sync.Common
{
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

        public static string ToReadableString(this byte[] hashBytes)
        {
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public static IObservable<EventPattern<NotifyCollectionChangedEventArgs>> CollectionChangedAsObservable<T>(this ObservableCollection<T> col)
        {
            return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>
                (h => col.CollectionChanged += h,
                h => col.CollectionChanged -= h)
                ;
        }
    }
}
