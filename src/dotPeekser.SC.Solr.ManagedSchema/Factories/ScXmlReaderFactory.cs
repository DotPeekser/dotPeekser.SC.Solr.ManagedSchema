namespace dotPeekser.SC.Solr.ManagedSchema.Factories
{
    using System.Xml;
    using Sitecore.Configuration;
    using dotPeekser.SC.Solr.ManagedSchema.Interfaces;

    internal class ScXmlReaderFactory : IXmlReaderFactory
    {
        public XmlNodeList GetConfigNodes(string xpath)
        {
            return Factory.GetConfigNodes(xpath);
        }
    }
}
