using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Escc.Umbraco.Caching
{
    public class HttpCachingService
    {
        public static void SetHttpCacheHeadersFromUmbracoContent(IPublishedContent content, bool isPreview, HttpCachePolicyBase cachePolicy)
        {
            // Default to 24 hours, but allow specific pages to override this
            var defaultCachePeriod = new TimeSpan(1, 0, 0, 0);
            var pageCachePeriod = ParseTimeSpan(content.GetPropertyValue<string>("cache"));
            var cachePeriod = (pageCachePeriod == TimeSpan.Zero) ? defaultCachePeriod : pageCachePeriod;

            DateTime? contentExpiry = content.GetPropertyValue<DateTime>("unpublishAt");
            if (contentExpiry == DateTime.MinValue) contentExpiry = null;

            DateTime? latestExpiry = content.GetPropertyValue<DateTime>("latestUnpublishDate");
            if (latestExpiry == DateTime.MinValue) latestExpiry = null;

            SetHttpCacheHeaders(DateTime.UtcNow,
                cachePeriod,
                new List<DateTime?>() { contentExpiry, latestExpiry },
                UmbracoContext.Current.InPreviewMode, cachePolicy);
        }

        /// <summary>
        /// Supports HTTP caching based on upload times.
        /// </summary>
        private static void SetHttpCacheHeaders(DateTime relativeToDate, TimeSpan defaultCachePeriod, IList<DateTime?> contentExpiryDates, bool isPreview, HttpCachePolicyBase cachePolicy)
        {
            // Only do this if it's enabled in web.config
            if (!IsHttpCachingEnabled()) return;

            // Never use HTTP caching for anyone who can edit the site
            if (isPreview)
            {
                cachePolicy.SetCacheability(HttpCacheability.NoCache);
                cachePolicy.SetMaxAge(new TimeSpan(0));
                cachePolicy.AppendCacheExtension("must-revalidate, proxy-revalidate");
            }
            else
            {
                // Get cache period. 
                var freshness = WorkOutCacheFreshness(relativeToDate, defaultCachePeriod, contentExpiryDates);

                // Cache the page
                cachePolicy.SetCacheability(HttpCacheability.Public);
                cachePolicy.SetExpires(freshness.FreshUntil.ToUniversalTime());
                cachePolicy.SetMaxAge(freshness.FreshFor);
            }
        }

        public static CacheFreshness WorkOutCacheFreshness(DateTime relativeToDate, TimeSpan defaultCachePeriod, IList<DateTime?> contentExpiryDates)
        {
            // Convert date to UTC so that it can be compared on an equal basis
            relativeToDate = relativeToDate.ToUniversalTime();

            // How long is this page fresh for?
            var freshness = new CacheFreshness()
            {
                FreshFor = defaultCachePeriod,
                FreshUntil = relativeToDate.Add(defaultCachePeriod)
            };

            for (var i = 0; i < contentExpiryDates.Count; i++)
            {
                // Convert date to UTC so that it can be compared on an equal basis
                if (contentExpiryDates[i].HasValue) contentExpiryDates[i] = contentExpiryDates[i].Value.ToUniversalTime();

                // Check if some or all of the content expires sooner than the default cache period?
                OverrideDueToContentExpiry(freshness, relativeToDate, contentExpiryDates[i]);
            }

            return freshness;

        }

        private static void OverrideDueToContentExpiry(CacheFreshness freshness, DateTime relativeToDate, DateTime? expiryDate)
        {
            if (expiryDate.HasValue && expiryDate > relativeToDate && expiryDate < freshness.FreshUntil)
            {
                freshness.FreshFor = expiryDate.Value.Subtract(relativeToDate);
                freshness.FreshUntil = relativeToDate.Add(freshness.FreshFor);
            }
        }


        private static TimeSpan ParseTimeSpan(string text)
        {
            switch (text)
            {
                case "5 minutes":
                    return new TimeSpan(0, 5, 0);
                case "10 minutes":
                    return new TimeSpan(0, 10, 0);
                case "30 minutes":
                    return new TimeSpan(0, 30, 0);
                case "1 hour":
                    return new TimeSpan(1, 0, 0);
            }
            return TimeSpan.Zero;
        }

        private static bool IsHttpCachingEnabled()
        {
            return true;
        }


    }
}