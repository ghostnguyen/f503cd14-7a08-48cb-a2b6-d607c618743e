﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    /// <summary>
    /// Used by <see cref="Flickr.PlacesPlacesForUser()"/>.
    /// </summary>
    public enum PlaceType
    {
        /// <summary>
        /// No place type selected. Not used by the Flickr API.
        /// </summary>
        None = 0,
        /// <summary>
        /// Locality.
        /// </summary>
        Locality = 7,
        /// <summary>
        /// County.
        /// </summary>
        County = 9,
        /// <summary>
        /// Region.
        /// </summary>
        Region = 8,
        /// <summary>
        /// Country.
        /// </summary>
        Country = 12,
        /// <summary>
        /// Neighbourhood.
        /// </summary>
        Neighbourhood = 22,
        /// <summary>
        /// Continent.
        /// </summary>
        Continent = 29
    }
}
