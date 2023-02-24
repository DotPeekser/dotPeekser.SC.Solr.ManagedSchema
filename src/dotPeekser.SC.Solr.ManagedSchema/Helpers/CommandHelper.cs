namespace dotPeekser.SC.Solr.ManagedSchema.Helpers
{
    using dotPeekser.SC.Solr.ManagedSchema.Attributes;
    using dotPeekser.SC.Solr.ManagedSchema.Data;
    using dotPeekser.SC.Solr.ManagedSchema.Extensions;

    public static class CommandHelper
    {
        public static string GetCommand(ConfigNodeType configNodeType, Operation operation)
        {
            string name = configNodeType.GetAttributeOfType<SolrNodeTypeAttribute>().Name;
            string command = $"{operation.ToString().ToLower()}-{name}";

            return command;
        }
    }
}
