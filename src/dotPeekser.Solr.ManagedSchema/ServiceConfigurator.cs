namespace dotPeekser.Solr.ManagedSchema
{
    using Microsoft.Extensions.DependencyInjection;
    using Sitecore.DependencyInjection;
    using dotPeekser.Solr.ManagedSchema.Factories;
    using dotPeekser.Solr.ManagedSchema.Interfaces;
    using dotPeekser.Solr.ManagedSchema.Logging;

    public class ServiceConfigurator : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IXmlReaderFactory, ScXmlReaderFactory>();
            serviceCollection.AddSingleton<IManagedSchemaLogger, ScLogger>();
        }
    }
}
