using System;

namespace Escc.Umbraco
{
    public class Image
    {
        public Uri ImageUrl { get; set; }
        public string AlternativeText { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
