using System;
using System.Collections.Generic;
using System.Globalization;
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

        /// <summary>
        /// Get the size in kilobytes of a media item returned by <see cref="GetUmbracoMedia"/>
        /// </summary>
        /// <param name="mediaItem">The resource to return the size of</param>
        /// <returns>string with the size in kilobytes followed by a lowercase k, eg 123k</returns>
        public static string GetFileSizeInKilobytes(MediaValues mediaItem)
        {
            // get ref to media item
            if (mediaItem != null && mediaItem.Values.ContainsKey("umbracoBytes"))
            {
                // convert bytes to kbytes
                double kSize = Math.Round((double)(Int32.Parse(mediaItem.Values["umbracoBytes"], CultureInfo.InvariantCulture)) / 1024);
                return kSize.ToString(CultureInfo.CurrentCulture) + "k";
            }
            
            throw new ArgumentException("Resource does not represent an item in the Umbraco media section", "mediaItem");
        }
    }
}
