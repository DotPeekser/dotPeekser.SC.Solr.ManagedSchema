namespace Sitecore.Solr.ManagedSchema
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using Sitecore.Solr.ManagedSchema.Factories;
    using Sitecore.Solr.ManagedSchema.Interfaces;
    using Sitecore.Solr.ManagedSchema.Logging;

    public class ServiceConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IXmlReaderFactory, ScXmlReaderFactory>();
            serviceCollection.AddSingleton<IManagedSchemaLogger, ScLogger>();
        }
    }
}
