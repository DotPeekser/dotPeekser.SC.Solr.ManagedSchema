namespace dotPeekser.Solr.ManagedSchema.Extensions
{
    using System.Xml;

    internal static class XmlNodeExtensions
    {
        public static void RemoveComments(this XmlNode node)
        {
            XmlNodeList commentNodes = node.SelectNodes(".//comment()");

            foreach (XmlNode commentNode in commentNodes)
            {
                commentNode.ParentNode.RemoveChild(commentNode);
            }
        }
    }
}
