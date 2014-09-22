using System;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Escc.Umbraco
{
    /// <summary>
    /// Make a copy of the release and expiry dates when content is saved, so that they can be retrieved from the Umbraco cache. 
    /// The built-in properties are only available from the database.
    /// </summary>
    public class PublishingDatesEventHandler : IApplicationEventHandler
    {
        /// <summary>
        /// ApplicationContext is created and other static objects that require initialization have been setup
        /// </summary>
        /// <param name="umbracoApplication"/><param name="applicationContext"/>
        public void OnApplicationInitialized(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        /// <summary>
        /// All resolvers have been initialized but resolution is not frozen so they can be modified in this method
        /// </summary>
        /// <param name="umbracoApplication"></param>
        /// <param name="applicationContext"></param>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // Using Saving rather than Published event for three reasons:
            //
            // 1. It fires at the moment of saving for pages set to publish in the future. 
            // 2. Save about to happen anyway so no need to call the ContentService, therefore audit trail not affected.
            // 3. Published event uses IPublishedStategy, and its Publish() method doesn't update the cache.
            ContentService.Saving += ContentService_Saving;
        }

        /// <summary>
        /// Bootup is completed, this allows you to perform any other bootup logic required for the application.
        ///             Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication"/><param name="applicationContext"/>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        void ContentService_Saving(IContentService contentService, SaveEventArgs<IContent> e)
        {
            foreach (var entity in e.SavedEntities)
            {
                CopyPublishingDateToProperty(entity, entity.ExpireDate, "unpublishAt");
            }
        }

        private static void CopyPublishingDateToProperty(IContent entity, DateTime? publishingDate, string copiedPropertyAlias)
        {
            // Ignore any document types not set up to support this
            if (entity.HasProperty(copiedPropertyAlias))
            {
                // Check if the property has changed, because calling Publish will fire this event again.
                // ContentService.SaveAndPublishWithStatus() has the option not to raise events, but in 7.1.4 it's not working.
                if (HasDatePropertyChanged(entity, publishingDate, copiedPropertyAlias))
                {
                    if (publishingDate.HasValue)
                    {
                        entity.SetValue(copiedPropertyAlias, publishingDate.Value);
                    }
                    else
                    {
                        entity.SetValue(copiedPropertyAlias, null);
                    }
                }
            }
        }

        private static bool HasDatePropertyChanged(IContent entity, DateTime? publishingDate, string copiedPropertyAlias)
        {
            var currentValue = entity.GetValue<DateTime>(copiedPropertyAlias);
            if (currentValue == DateTime.MinValue)
            {
                return publishingDate.HasValue;
            }
            else
            {
                return !(publishingDate.HasValue && currentValue == publishingDate.Value);
            }
        }
    }
}