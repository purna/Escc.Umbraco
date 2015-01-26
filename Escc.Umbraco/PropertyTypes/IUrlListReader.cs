using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Escc.Umbraco.PropertyTypes
{
    /// <summary>
    /// Read target URLs from a combination of two fields on an Umbraco page
    /// </summary>
    public interface IUrlListReader
    {
        /// <summary>
        /// Read target URLs from a combination of two fields on an Umbraco page
        /// </summary>
        /// <param name="content">The Umbraco page.</param>
        /// <param name="internalUrlsPropertyAlias">The alias of a multi node tree picker field.</param>
        /// <param name="externalUrlsPropertyAlias">The alias of a textbox field.</param>
        /// <param name="relativeOrAbsolute">Specify whether relative or absolute URLs are required.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">content</exception>
        IEnumerable<Uri> ReadUrls(IPublishedContent content, string internalUrlsPropertyAlias, string externalUrlsPropertyAlias, UriKind relativeOrAbsolute = UriKind.Relative);
    }
}