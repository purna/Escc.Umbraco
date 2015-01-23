using System;
using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Escc.Umbraco
{
    /// <summary>
    /// Read target URLs from a combination of two fields on an Umbraco page
    /// </summary>
    public interface ITargetUrlReader
    {
        /// <summary>
        /// Read target URLs from a combination of two fields on an Umbraco page
        /// </summary>
        /// <param name="content">The Umbraco page.</param>
        /// <param name="multiNodeTreePickerAlias">The alias of a multi node tree picker field.</param>
        /// <param name="textboxAlias">The alias of a textbox field.</param>
        /// <param name="relativeOrAbsolute">Specify whether relative or absolute URLs are required.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">content</exception>
        IEnumerable<Uri> GetTargetUrls(IPublishedContent content, string multiNodeTreePickerAlias, string textboxAlias, UriKind relativeOrAbsolute = UriKind.Relative);
    }
}