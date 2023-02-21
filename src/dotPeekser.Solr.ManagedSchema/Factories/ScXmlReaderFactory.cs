namespace dotPeekser.Solr.ManagedSchema.Factories
{
    using System.Xml;
    using Sitecore.Configuration;
    using dotPeekser.Solr.ManagedSchema.Interfaces;

    internal class ScXmlReaderFactory : IXmlReaderFactory
    {
        public XmlNodeList GetConfigNodes(string xpath)
        {
            return Factory.GetConfigNodes(xpath);
        }
    }
}
