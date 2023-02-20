namespace Sitecore.Solr.ManagedSchema
{
    internal static class ManagedSchemaConstants
    {
        public const string CommandsXmlPath = "contentSearch/solr.solrManagedSchema/commands";

        public static class SolrGeneralField
        {
            public const string Name = "name";
            public const string Type = "type";
        }

        public static class SolrCopyField
        {
            public const string Source = "source";
            public const string Dest = "dest";
        }

        public static class Node
        {
            public const string DeleteAttribute = "delete";
            public const string ApplyToIndexAttribute = "applyToIndex";
        }
    }
}
