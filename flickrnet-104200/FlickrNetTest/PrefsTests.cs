﻿using System;
using System.Text;
using System.Collections.Generic;

using NUnit.Framework;
using FlickrNet;

namespace FlickrNetTest
{
    /// <summary>
    /// Summary description for PrefsTest
    /// </summary>
    [TestFixture]
    [Category("AccessTokenRequired")]
    public class PrefsTests
    {
        public PrefsTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Test]
        public void PrefsGetContentTypeTest()
        {
            var s = TestData.GetAuthInstance().PrefsGetContentType();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(ContentType.None, s);
        }

        [Test]
        public void PrefsGetGeoPermsTest()
        {
            var p = TestData.GetAuthInstance().PrefsGetGeoPerms();

            Assert.IsNotNull(p);
            Assert.IsTrue(p.ImportGeoExif);
            Assert.AreEqual(GeoPermissionType.Public, p.GeoPermissions);
        }

        [Test]
        public void PrefsGetHiddenTest()
        {
            var s = TestData.GetAuthInstance().PrefsGetHidden();

            Assert.IsNotNull(s);
            Assert.AreNotEqual(HiddenFromSearch.None, s);
        }

        [Test]
        public void PrefsGetPrivacyTest()
        {
            var p = TestData.GetAuthInstance().PrefsGetPrivacy();

            Assert.IsNotNull(p);
            Assert.AreEqual(PrivacyFilter.PublicPhotos, p);
        }

        [Test]
        public void PrefsGetSafetyLevelTest()
        {
            var s = TestData.GetAuthInstance().PrefsGetSafetyLevel();

            Assert.IsNotNull(s);
            Assert.AreEqual(SafetyLevel.Safe, s);
        }


    }
}
