using System;
using System.Linq;
using NUnit.Framework;
using FlickrNet;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for FlickrPhotoSetGetPhotos
    /// </summary>
    [TestFixture]
    public class PhotosetsGetPhotosTests : BaseTest
    {
        [Test]
        public void PhotosetsGetPhotosBasicTest()
        {
            PhotosetPhotoCollection set = Instance.PhotosetsGetPhotos("72157618515066456", PhotoSearchExtras.All, PrivacyFilter.None, 1, 10);

            Assert.AreEqual(8, set.Total, "NumberOfPhotos should be 8.");
            Assert.AreEqual(8, set.Count, "Should be 8 photos returned.");
        }

        [Test]
        public void PhotosetsGetPhotosMachineTagsTest()
        {
            var set = Instance.PhotosetsGetPhotos("72157594218885767", PhotoSearchExtras.MachineTags, PrivacyFilter.None, 1, 10);

            var machineTagsFound = set.Any(p => !String.IsNullOrEmpty(p.MachineTags));

            Assert.IsTrue(machineTagsFound, "No machine tags were found in the photoset");
        }

        [Test]
        public void PhotosetsGetPhotosFilterMediaTest()
        {
            // http://www.flickr.com/photos/sgoralnick/sets/72157600283870192/
            // Set contains videos and photos
            var theset = Instance.PhotosetsGetPhotos("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Videos);

            foreach (var p in theset)
            {
                Assert.AreEqual("video", p.Media, "Should be video.");
            }

            var theset2 = Instance.PhotosetsGetPhotos("72157600283870192", PhotoSearchExtras.Media, PrivacyFilter.None, 1, 100, MediaType.Photos);
            foreach (var p in theset2)
            {
                Assert.AreEqual("photo", p.Media, "Should be photo.");
            }

        }

        [Test]
        public void PhotosetsGetPhotosWebUrlTest()
        {
            var theset = Instance.PhotosetsGetPhotos("72157618515066456");

            foreach(var p in theset)
            {
                Assert.IsNotNull(p.UserId, "UserId should not be null.");
                Assert.AreNotEqual(String.Empty, p.UserId, "UserId should not be an empty string.");
                var url = "http://www.flickr.com/photos/" + p.UserId + "/" + p.PhotoId + "/";
                Assert.AreEqual(url, p.WebUrl);
            }
        }

        [Test]
        public void PhotosetsGetPhotosPrimaryPhotoTest()
        {
            var theset = Instance.PhotosetsGetPhotos("72157618515066456", 1, 100);

            Assert.IsNotNull(theset.PrimaryPhotoId, "PrimaryPhotoId should not be null.");

            if (theset.Total >= theset.PerPage) return;

            var primary = theset.FirstOrDefault(p => p.PhotoId == theset.PrimaryPhotoId);

            Assert.IsNotNull(primary, "Primary photo should have been found.");
        }

        [Test]
        [Category("AccessTokenRequired")]
        public void PhotosetsGetPhotosOrignalTest()
        {
            var photos = AuthInstance.PhotosetsGetPhotos("72157623027759445", PhotoSearchExtras.AllUrls);

            foreach (var photo in photos)
            {
                Assert.IsNotNull(photo.OriginalUrl, "Original URL should not be null.");
            }
        }
    }
}
