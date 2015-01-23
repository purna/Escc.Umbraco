using System;
using System.Collections.Generic;
using Escc.Umbraco.Caching;
using NUnit.Framework;

namespace Escc.Umbraco.Tests
{
    [TestFixture]
    public class HttpCachingServiceTests
    {
        // 1pm UTC (2pm BST) on 1 Sep 2014
        private readonly DateTime _startDate = new DateTime(2014, 10, 1, 13, 0, 0, DateTimeKind.Utc);

        // Cache period 24 hours
        private readonly TimeSpan _defaultCachePeriod = new TimeSpan(1, 0, 0, 0);

        [Test]
        public void ExpiryDateOverridesDefaultCachePeriodIfSooner()
        {
            // Expires after 23 hours 
            var expiryDate = _startDate.AddHours(23);

            // Should pick the expiry date, not default timespan
            var service = new HttpCachingService();
            var freshness = service.WorkOutCacheFreshness(_startDate, _defaultCachePeriod, new List<DateTime>() { expiryDate });

            Assert.AreEqual(freshness.FreshFor, expiryDate - _startDate);
            Assert.AreEqual(freshness.FreshUntil, expiryDate);
        }

        [Test]
        public void EarliestExpiryDateWins()
        {
            var expiryDate1 = _startDate.AddHours(15);
            var expiryDate2 = _startDate.AddHours(7);
            var expiryDate3 = _startDate.AddHours(11);

            // Should pick second expiry date
            var service = new HttpCachingService();
            var freshness = service.WorkOutCacheFreshness(_startDate, _defaultCachePeriod, new List<DateTime>() { expiryDate1, expiryDate2, expiryDate3 });

            Assert.AreEqual(freshness.FreshFor, expiryDate2 - _startDate);
            Assert.AreEqual(freshness.FreshUntil, expiryDate2);
        }

        [Test]
        public void DefaultCachePeriodOverridesExpiryDateIfSooner()
        {
            // Expires after 30 hours 
            var expiryDate = _startDate.AddHours(30);

            // Should pick the default timespan, not expiry date
            var service = new HttpCachingService();
            var freshness = service.WorkOutCacheFreshness(_startDate, _defaultCachePeriod, new List<DateTime>() { expiryDate });

            Assert.AreEqual(freshness.FreshFor, _defaultCachePeriod);
            Assert.AreEqual(freshness.FreshUntil, _startDate.Add(_defaultCachePeriod));
        }

        [Test]
        public void ExpiryDateInPastIsIgnored()
        {
            // Expires in past
            var expiryDate = _startDate.AddHours(-5);

            // Expiry should be the default timespan
            // This is because the date likely relates to partial content, which is not part of what we'll be serving up anyway. 
            // If the whole content had expired, we would be asking to cache it in the first place.
            var service = new HttpCachingService();
            var freshness = service.WorkOutCacheFreshness(_startDate, _defaultCachePeriod, new List<DateTime>() { expiryDate });

            Assert.AreEqual(freshness.FreshFor, _defaultCachePeriod);
            Assert.AreEqual(freshness.FreshUntil, _startDate.Add(_defaultCachePeriod));
        }
    }
}
