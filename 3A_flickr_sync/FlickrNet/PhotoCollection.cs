using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using _3A_flickr_sync.Common;

namespace _3A_flickr_sync.FlickrNet
{
    /// <remarks/>
    public sealed class PhotoCollection : PagedPhotoCollection, IFlickrParsable
    {
        void IFlickrParsable.Load(XmlReader reader)
        {
            if (reader.LocalName != "photos")
                Helper.CheckParsingException(reader);

            while (reader.MoveToNextAttribute())
            {
                switch (reader.LocalName)
                {
                    case "total":
                        Total = String.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "perpage":
                    case "per_page":
                        PerPage = String.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "page":
                        Page = String.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    case "pages":
                        Pages = String.IsNullOrEmpty(reader.Value) ? 0 : int.Parse(reader.Value, System.Globalization.CultureInfo.InvariantCulture);
                        break;
                    default:
                        Helper.CheckParsingException(reader);
                        break;

                }
            }

            reader.Read();

            while (reader.LocalName == "photo")
            {
                Photo p = new Photo();
                ((IFlickrParsable)p).Load(reader);
                if (!String.IsNullOrEmpty(p.PhotoId)) Add(p);
            }

            // Skip to next element (if any)
            reader.Skip();

        }

    }
}
