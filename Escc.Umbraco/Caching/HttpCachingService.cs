using System;
using System.Collections.Generic;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace Escc.Umbraco.Caching
{
    /// <summary>
    /// Manage HTTP caching for Umbraco content
    /// </summary>
    public class HttpCachingService
    {
        /// <summary>
        /// Sets the content of the HTTP cache headers from well-known Umbraco properties.
        /// </summary>
        /// <param name="content">The Umbraco published content node.</param>
        /// <param name="expiryDateFieldAliases">Aliases of any fields containing expiry dates. Expiry of the page itself is taken care of by default.</param>
        /// <param name="isPreview">if set to <c>true</c> Umbraco is in preview mode.</param>
        /// <param name="cachePolicy">The cache policy.</param>
        public static void SetHttpCacheHeadersFromUmbracoContent(IPublishedContent content, IList<string> expiryDateFieldAliases, bool isPreview, HttpCachePolicyBase cachePolicy)
        {
            // Default to 24 hours, but allow specific pages to override this
            var defaultCachePeriod = new TimeSpan(1, 0, 0, 0);
            var pageCachePeriod = ParseTimeSpan(content.GetPropertyValue<string>("cache"));
            var cachePeriod = (pageCachePeriod == TimeSpan.Zero) ? defaultCachePeriod : pageCachePeriod;

            // Use well-known unpublishAt property alias set by ExpiryDateEventHandler, but allow other expiry fields to be specified too
            if (expiryDateFieldAliases == null) expiryDateFieldAliases = new List<string>();
            if (!expiryDateFieldAliases.Contains("unpublishAt")) expiryDateFieldAliases.Add("unpublishAt");
            var expiryDates = new List<DateTime>();

            foreach (var fieldAlias in expiryDateFieldAliases)
            {
                var expiryDate = content.GetPropertyValue<DateTime>(fieldAlias);
                if (expiryDate != DateTime.MinValue) expiryDates.Add(expiryDate);
            }

            SetHttpCacheHeaders(DateTime.UtcNow,
                cachePeriod,
                expiryDates,
                UmbracoContext.Current.InPreviewMode, cachePolicy);
        }

        /// <summary>
        /// Set HTTP cache headers based on data passed in.
        /// </summary>
        /// <param name="relativeToDate">The start date cache time spans are relative to, usually <see cref="DateTime.UtcNow"/>.</param>
        /// <param name="defaultCachePeriod">The default cache period.</param>
        /// <param name="contentExpiryDates">Expiry dates for any content, either part of a page or the whole page.</param>
        /// <param name="isPreview">if set to <c>true</c> [is preview].</param>
        /// <param name="cachePolicy">The cache policy.</param>
        private static void SetHttpCacheHeaders(DateTime relativeToDate, TimeSpan defaultCachePeriod, IList<DateTime> contentExpiryDates, bool isPreview, HttpCachePolicyBase cachePolicy)
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

        /// <summary>
        /// Works out how long to cache based on a default period and any expiry dates, relative to a start date.
        /// </summary>
        /// <param name="relativeToDate">The start date cache time spans are relative to, usually <see cref="DateTime.UtcNow"/>.</param>
        /// <param name="defaultCachePeriod">The default cache period.</param>
        /// <param name="contentExpiryDates">Expiry dates for any content, either part of a page or the whole page.</param>
        /// <returns>An absolute time and relative timespan representing how long to cache the content for.</returns>
        public static CacheFreshness WorkOutCacheFreshness(DateTime relativeToDate, TimeSpan defaultCachePeriod, IList<DateTime> contentExpiryDates)
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
                contentExpiryDates[i] = contentExpiryDates[i].ToUniversalTime();

                // Check if some or all of the content expires sooner than the default cache period?
                OverrideDueToContentExpiry(freshness, relativeToDate, contentExpiryDates[i]);
            }

            return freshness;

        }

        private static void OverrideDueToContentExpiry(CacheFreshness freshness, DateTime relativeToDate, DateTime expiryDate)
        {
            if (expiryDate > relativeToDate && expiryDate < freshness.FreshUntil)
            {
                freshness.FreshFor = expiryDate.Subtract(relativeToDate);
                freshness.FreshUntil = relativeToDate.Add(freshness.FreshFor);
            }
        }

        /// <summary>
        /// Parses a time span from hardcoded values which are expected to be found in an Umbraco dropdown list property
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
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