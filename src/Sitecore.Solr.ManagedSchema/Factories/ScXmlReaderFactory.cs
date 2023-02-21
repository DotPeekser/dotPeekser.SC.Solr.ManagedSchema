namespace Sitecore.Solr.ManagedSchema.Factories
{
    using System.Xml;
    using Sitecore.Configuration;
    using Sitecore.Solr.ManagedSchema.Interfaces;

    internal class ScXmlReaderFactory : IXmlReaderFactory
    {
        public XmlNodeList GetConfigNodes(string xpath)
        {
            return Factory.GetConfigNodes(xpath);
        }
    }
}
