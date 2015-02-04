using System;
using System.Collections.Generic;
using System.Xml.XPath;

namespace Escc.Umbraco.Media
{
    /// <summary>
    /// Reads properties of a media item from XML - needed when dealing with extra properties added to the default media type
    /// Code from http://shazwazza.com/post/ultra-fast-media-performance-in-umbraco/
    /// </summary>
    public class MediaValues
    {
        public MediaValues(XPathNavigator xpath)
        {
            if (xpath == null) throw new ArgumentNullException("xpath");
            Name = xpath.GetAttribute("nodeName", "");
            Values = new Dictionary<string, string>();
            var result = xpath.SelectChildren(XPathNodeType.Element);
            while (result.MoveNext())
            {
                if (result.Current != null && !result.Current.HasAttributes)
                {
                    Values.Add(result.Current.Name, result.Current.Value);
                }
            }
        }

        public string Name { get; private set; }
        public IDictionary<string, string> Values { get; private set; }

    }
}