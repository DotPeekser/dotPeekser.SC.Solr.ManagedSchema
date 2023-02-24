namespace dotPeekser.SC.Solr.ManagedSchema.Attributes
{
    using System;

    public class SolrNodeTypeAttribute : Attribute
    {
        public string Name { get; set; }

        public SolrNodeTypeAttribute(string name)
        {
            this.Name = name;
        }
    }
}
