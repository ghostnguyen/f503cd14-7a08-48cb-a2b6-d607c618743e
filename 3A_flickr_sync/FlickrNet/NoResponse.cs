using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    public sealed class NoResponse : IFlickrParsable
    {
        void IFlickrParsable.Load(System.Xml.XmlReader reader)
        {
        }
    }
}
