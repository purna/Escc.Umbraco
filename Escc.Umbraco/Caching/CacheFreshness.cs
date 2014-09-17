using System;

namespace Escc.Umbraco.Caching
{
    public class CacheFreshness
    {
        /// <summary>
        /// How long is this content fresh for? 
        /// </summary>
        public TimeSpan FreshFor { get; set; }

        /// <summary>
        /// Based on <see cref="FreshFor" /> the content will be fresh until this date
        /// </summary>
        public DateTime FreshUntil { get; set; }
    }
}