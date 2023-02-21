namespace dotPeekser.Solr.ManagedSchema.Helpers
{
    using dotPeekser.Solr.ManagedSchema.Attributes;
    using dotPeekser.Solr.ManagedSchema.Data;
    using dotPeekser.Solr.ManagedSchema.Extensions;

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
