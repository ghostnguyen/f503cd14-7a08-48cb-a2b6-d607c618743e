using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace _3A_flickr_sync.FlickrNet
{
    public interface IFlickrParsable
    {
        /// <summary>
        /// Allows each class that implements this interface to be loaded via an <see cref="XmlReader"/>.
        /// </summary>
        /// <param name="reader"></param>
        void Load(XmlReader reader);
    }
}
