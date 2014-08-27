
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

        /// <summary>
        /// Gets or sets the JavaScript for a Google Analytics content experiment
        /// </summary>
        public string ContentExperimentScript { get; set; }

        /// <summary>
        /// Gets or sets whether the current view is a published page
        /// </summary>
        public bool IsPublishedView { get; set; }
    }
}
