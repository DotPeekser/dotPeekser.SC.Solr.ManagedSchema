namespace dotPeekser.SC.Solr.ManagedSchema.Interfaces
{
    using System.Xml;

    public interface IXmlReaderFactory
    {
        XmlNodeList GetConfigNodes(string xpath);
    }
}
