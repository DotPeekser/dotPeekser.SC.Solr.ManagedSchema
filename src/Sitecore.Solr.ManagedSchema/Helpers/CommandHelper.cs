namespace Sitecore.Solr.ManagedSchema.Helpers
{
    using Sitecore.Solr.ManagedSchema.Attributes;
    using Sitecore.Solr.ManagedSchema.Data;
    using Sitecore.Solr.ManagedSchema.Extensions;

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
