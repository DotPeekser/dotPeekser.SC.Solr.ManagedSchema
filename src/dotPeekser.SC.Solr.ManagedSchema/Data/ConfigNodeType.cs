namespace dotPeekser.SC.Solr.ManagedSchema.Data
{
    using dotPeekser.SC.Solr.ManagedSchema.Attributes;

    /// <summary>
    /// Represents the available config node types in the config xml.
    /// </summary>
    public enum ConfigNodeType
    {
        [SolrNodeType("field-type")]
        Type = 0,
        [SolrNodeType("field")]
        Field = 1,
        [SolrNodeType("dynamic-field")]
        DynamicField = 2,
        [SolrNodeType("copy-field")]
        CopyField = 3
    }
}
