
namespace Escc.Umbraco
{
    /// <summary>
    /// Base class for common properties to be available to all Umbraco view models
    /// </summary>
    public abstract class BaseViewModel
    {
        /// <summary>
        /// Gets or sets the page title
        /// </summary>
        public string PageTitle { get; set; }
    }
}
