namespace Sitecore.Solr.ManagedSchemaTests.Serializer
{
    using System.Collections.Generic;
    using System.Text;
    using System.Xml.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Copied from Sitecore.ContentSearch.SolrNetExtension.Commands.SchemaCommand.Serialize
    /// </remarks>
    internal class SchemaCommandSerializer
    {
        public static string Serialize(List<XElement> elements)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            foreach (XElement element in elements)
            {
                if (!element.IsEmpty)
                {
                    string text = JsonConvert.SerializeXNode(element);
                    stringBuilder.Append(text.Substring(1, text.Length - 2));
                    stringBuilder.Append(",");
                }
            }
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }
    }
}
