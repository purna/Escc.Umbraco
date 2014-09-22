using System;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;

namespace Escc.Umbraco
{
    /// <summary>
    /// In Umbraco 7.1.4 when you publish a page without a start publishing date, then you add a start publishing date, the cache always retains the first version. This is a workaround to prevent that.
    /// </summary>
    public class PublishingDateBugFixHandler : IApplicationEventHandler
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
        /// <param name="umbracoApplication"/><param name="applicationContext"/>
        public void OnApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Publishing += ContentService_Publishing;
        }

        /// <summary>
        /// When publishing a page, call Save() first as it prevents the bug from occurring
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void ContentService_Publishing(IPublishingStrategy sender, PublishEventArgs<global::Umbraco.Core.Models.IContent> e)
        {
            foreach (var entity in e.PublishedEntities)
            {
                // Minimise calls to Save which appear in the audit log. Aim is to work around a bug which only occurs 
                // when you set a release date, so check first that there is one. Time check means it should save when you
                // hit 'Save and publish' but when the release date comes and this event fires again, the release date will 
                // be a few seconds in the past and it won't do an unnecessary extra Save().
                if (entity.ReleaseDate.HasValue &&
                    entity.ReleaseDate.Value.ToUniversalTime() > DateTime.UtcNow)
                {
                    ApplicationContext.Current.Services.ContentService.Save(entity);
                }
            }
        }

        /// <summary>
        /// Bootup is completed, this allows you to perform any other bootup logic required for the application.
        ///             Resolution is frozen so now they can be used to resolve instances.
        /// </summary>
        /// <param name="umbracoApplication"/><param name="applicationContext"/>
        public void OnApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {

        }
    }
}