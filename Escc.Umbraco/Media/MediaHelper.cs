using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Escc.Umbraco.Media
{
    /// <summary>
    /// Helper for working with Umbraco media items
    /// </summary>
    public class MediaHelper
    {
        /// <summary>
        /// Gets an Umbraco media item.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// Code from http://shazwazza.com/post/ultra-fast-media-performance-in-umbraco/
        /// That page recommends Examine as the first choice, but we know Examine doesn't work on Azure so go for the second best method.
        /// </remarks>
        public static MediaValues GetUmbracoMedia(int id)
        {
            var media = umbraco.library.GetMedia(id, false);
            if (media != null && media.Current != null)
            {
                media.MoveNext();
                return new MediaValues(media.Current);
            }

            return null;
        }
    }
}
