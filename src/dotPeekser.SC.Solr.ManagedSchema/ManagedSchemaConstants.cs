namespace dotPeekser.SC.Solr.ManagedSchema
{
    internal static class ManagedSchemaConstants
    {
        public const string CommandsXmlPath = "contentSearch/solr.customSolrManagedSchema/commands";

        public static class SolrGeneralField
        {
            public const string Name = "name";
            public const string Type = "type";
        }

        public static class SolrType
        {
            public const string Analyzer = "analyzer";
            public const string QueryAnalyzer = "queryAnalyzer";
            public const string IndexAnalyzer = "indexAnalyzer";
            public const string Tokenizer = "tokenizer";
            public const string Filters = "filters";
            public const string Class = "class";
        }

        public static class SolrCopyField
        {
            public const string Source = "source";
            public const string Dest = "dest";
        }

        public static class SolrField
        {
            public const string Indexed = "indexed";
            public const string Stored = "stored";
        }

        public static class Node
        {
            public const string DeleteAttribute = "delete";
            public const string ApplyToIndexAttribute = "applyToIndex";
        }
    }
}
