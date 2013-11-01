using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3A_flickr_sync.FlickrNet
{
    public partial class Flickr
    {
        /// <summary>
        /// Add a photo to a photoset.
        /// </summary>
        /// <param name="photosetId">The ID of the photoset to add the photo to.</param>
        /// <param name="photoId">The ID of the photo to add.</param>
        public void PhotosetsAddPhoto(string photosetId, string photoId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.addPhoto");
            parameters.Add("photoset_id", photosetId);
            parameters.Add("photo_id", photoId);

            GetResponse<NoResponse>(parameters);
        }

        /// <summary>
        /// Creates a blank photoset, with a title and a primary photo (minimum requirements).
        /// </summary>
        /// <param name="title">The title of the photoset.</param>
        /// <param name="primaryPhotoId">The ID of the photo which will be the primary photo for the photoset. This photo will also be added to the set.</param>
        /// <returns>The <see cref="Photoset"/> that is created.</returns>
        public Photoset PhotosetsCreate(string title, string primaryPhotoId)
        {
            return PhotosetsCreate(title, null, primaryPhotoId);
        }

        /// <summary>
        /// Creates a blank photoset, with a title, description and a primary photo.
        /// </summary>
        /// <param name="title">The title of the photoset.</param>
        /// <param name="description">THe description of the photoset.</param>
        /// <param name="primaryPhotoId">The ID of the photo which will be the primary photo for the photoset. This photo will also be added to the set.</param>
        /// <returns>The <see cref="Photoset"/> that is created.</returns>
        public Photoset PhotosetsCreate(string title, string description, string primaryPhotoId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.create");
            parameters.Add("primary_photo_id", primaryPhotoId);
            if (!String.IsNullOrEmpty(title)) parameters.Add("title", title);
            if (!String.IsNullOrEmpty(description)) parameters.Add("description", description);

            return GetResponse<Photoset>(parameters);
        }

        /// <summary>
        /// Deletes the specified photoset.
        /// </summary>
        /// <param name="photosetId">The ID of the photoset to delete.</param>
        public void PhotosetsDelete(string photosetId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.delete");
            parameters.Add("photoset_id", photosetId);

            GetResponse<NoResponse>(parameters);
        }

        /// <summary>
        /// Updates the title and description for a photoset.
        /// </summary>
        /// <param name="photosetId">The ID of the photoset to update.</param>
        /// <param name="title">The new title for the photoset.</param>
        /// <param name="description">The new description for the photoset.</param>
        public void PhotosetsEditMeta(string photosetId, string title, string description)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.editMeta");
            parameters.Add("photoset_id", photosetId);
            parameters.Add("title", title);
            parameters.Add("description", description);

            GetResponse<NoResponse>(parameters);
        }

        /// <summary>
        /// Gets the information about a photoset.
        /// </summary>
        /// <param name="photosetId">The ID of the photoset to return information for.</param>
        /// <returns>A <see cref="Photoset"/> instance.</returns>
        public Photoset PhotosetsGetInfo(string photosetId)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.getInfo");
            parameters.Add("photoset_id", photosetId);

            return GetResponse<Photoset>(parameters);
        }

        /// <summary>
        /// Gets a list of the currently authenticated users photosets.
        /// </summary>
        /// <returns>A <see cref="PhotosetCollection"/> instance containing a collection of photosets.</returns>
        public PhotosetCollection PhotosetsGetList()
        {
            CheckRequiresAuthentication();

            return PhotosetsGetList(null, 0, 0);
        }

        /// <summary>
        /// Gets a list of the currently authenticated users photosets.
        /// </summary>
        /// <param name="page">The page of the results to return. Defaults to page 1.</param>
        /// <param name="perPage">The number of photosets to return per page. Defaults to 500.</param>
        /// <returns>A <see cref="PhotosetCollection"/> instance containing a collection of photosets.</returns>
        public PhotosetCollection PhotosetsGetList(int page, int perPage)
        {
            CheckRequiresAuthentication();

            return PhotosetsGetList(null, page, perPage);
        }

        /// <summary>
        /// Gets a list of the specified users photosets.
        /// </summary>
        /// <param name="userId">The ID of the user to return the photosets of.</param>
        /// <returns>A <see cref="PhotosetCollection"/> instance containing a collection of photosets.</returns>
        public PhotosetCollection PhotosetsGetList(string userId)
        {
            return PhotosetsGetList(userId, 0, 0);
        }

        /// <summary>
        /// Gets a list of the specified users photosets.
        /// </summary>
        /// <param name="userId">The ID of the user to return the photosets of.</param>
        /// <param name="page">The page of the results to return. Defaults to page 1.</param>
        /// <param name="perPage">The number of photosets to return per page. Defaults to 500.</param>
        /// <returns>A <see cref="PhotosetCollection"/> instance containing a collection of photosets.</returns>
        public PhotosetCollection PhotosetsGetList(string userId, int page, int perPage)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photosets.getList");
            if (userId != null) parameters.Add("user_id", userId);
            if (page > 0) parameters.Add("page", page.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));
            if (perPage > 0) parameters.Add("per_page", perPage.ToString(System.Globalization.NumberFormatInfo.InvariantInfo));

            PhotosetCollection photosets = GetResponse<PhotosetCollection>(parameters);
            //foreach (Photoset photoset in photosets)
            //{
            //    photoset.OwnerId = userId;
            //}
            return photosets;
        }
    }
}
