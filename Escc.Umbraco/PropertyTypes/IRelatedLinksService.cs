using System.Collections.Generic;
using Umbraco.Core.Models;

namespace Escc.Umbraco.PropertyTypes
{
    /// <summary>
    /// Service for reading link data from a property in an <see cref="IPublishedContent" />
    /// </summary>
    public interface IRelatedLinksService
    {
        IList<HtmlLink> BuildRelatedLinksViewModelFromUmbracoContent(IPublishedContent content, string propertyAlias);
    }
}