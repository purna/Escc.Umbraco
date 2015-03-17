using System;

namespace Escc.Umbraco.PropertyTypes
{
    /// <summary>
    /// A link to be displayed as HTML
    /// </summary>
    public class HtmlLink
    {
        /// <summary>
        /// Gets or sets the text of the link.
        /// </summary>
        /// <value>
        /// The text.
        /// </value>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the URL to link to.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        public Uri Url { get; set; }
    }
}