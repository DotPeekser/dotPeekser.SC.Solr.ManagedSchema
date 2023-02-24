namespace dotPeekser.SC.Solr.ManagedSchemaTests.Factories
{
    using System.Xml;
    using dotPeekser.SC.Solr.ManagedSchema.Interfaces;

    internal class XmlReaderFactory : IXmlReaderFactory
    {
        private readonly string _filepath;

        public XmlReaderFactory(string filepath)
        {
            this._filepath = filepath;
        }

        public XmlNodeList GetConfigNodes(string xpath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(this._filepath);

            XmlNode sitecoreNode = doc.DocumentElement.SelectSingleNode("./sitecore");

            return sitecoreNode.SelectNodes(xpath);
        }
    }
}
