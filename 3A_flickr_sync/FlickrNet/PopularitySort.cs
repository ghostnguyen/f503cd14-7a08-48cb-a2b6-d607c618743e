﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    /// <summary>
    /// Sorting used for <see cref="Flickr.StatsGetPopularPhotos(DateTime, PopularitySort, int, int)"/>
    /// </summary>
    public enum PopularitySort
    {
        /// <summary>
        /// No sorting performed.
        /// </summary>
        None,
        /// <summary>
        /// Sort by number of views.
        /// </summary>
        Views,
        /// <summary>
        /// Sort by number of comments.
        /// </summary>
        Comments,
        /// <summary>
        /// Sort by number of favorites.
        /// </summary>
        Favorites
    }
}
