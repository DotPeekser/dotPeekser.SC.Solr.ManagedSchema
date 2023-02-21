namespace Sitecore.Solr.ManagedSchemaTests.Logging
{
    using System;
    using Sitecore.Solr.ManagedSchema.Interfaces;

    internal class TestManagedSchemaLogger : IManagedSchemaLogger
    {
        public void Debug(string message, object owner)
        {
            Console.WriteLine("[DEBUG]: " + message, owner);
        }

        public void Warn(string message, object owner)
        {
            Console.WriteLine("[WARN]: " + message, owner);
        }

        public void Error(string message, object owner)
        {
            Console.WriteLine("[ERROR]: " + message, owner);
        }
    }
}
