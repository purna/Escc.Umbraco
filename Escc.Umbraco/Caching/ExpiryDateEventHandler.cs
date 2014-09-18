using System;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace Escc.Umbraco.Caching
{
    /// <summary>
    /// Make a copy of the expiry date when content is saved, so that it can be retrieved from the Umbraco cache. 
    /// The built-in property is only available from the database.
    /// </summary>
    public class ExpiryDateEventHandler : IApplicationEventHandler
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
            ContentService.Published += ContentService_Published;
        }

        /// <summary>
        /// Bootup is completed, this allows you to perform any other bootup logic required for the application.
        ///             Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication"/><param name="applicationContext"/>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }

        void ContentService_Published(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            foreach (var entity in e.PublishedEntities)
            {
                // Ignore any document types not set up to support this
                if (entity.HasProperty("unpublishAt"))
                {
                    // Check if the property has changed, because calling Publish will fire this event again.
                    // ContentService.SaveAndPublishWithStatus() has the option not to raise events, but in 7.1.4 it's not working.
                    if (HasExpireDateChanged(entity))
                    {
                        if (entity.ExpireDate.HasValue)
                        {
                            entity.SetValue("unpublishAt", entity.ExpireDate.Value);
                        }
                        else
                        {
                            entity.SetValue("unpublishAt", null);
                        }

                        // Couldn't use sender.Publish() because it doesn't update the cache.
                        // Unfortunately using this method results in a double save in the audit trail.
                        ApplicationContext.Current.Services.ContentService.PublishWithStatus(entity,
                            entity.WriterId);
                    }
                }
            }
        }

        private static bool HasExpireDateChanged(IContent entity)
        {
            var currentValue = entity.GetValue<DateTime>("unpublishAt");
            if (currentValue == DateTime.MinValue)
            {
                return entity.ExpireDate.HasValue;
            }
            else
            {
                return !(entity.ExpireDate.HasValue && currentValue == entity.ExpireDate.Value);
            }
        }
    }
}