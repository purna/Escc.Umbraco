using System;

namespace Escc.Umbraco.PropertyTypes
{
    /// <summary>
    /// An image stored in the Umbraco media library
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Relative URL of the image
        /// </summary>
        public Uri ImageUrl { get; set; }

        /// <summary>
        /// Name of the image, used when the image is not available
        /// </summary>
        public string AlternativeText { get; set; }

        /// <summary>
        /// Width of the image in pixels
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// Height of the image in pixels
        /// </summary>
        public int Height { get; set; }
    }
}
