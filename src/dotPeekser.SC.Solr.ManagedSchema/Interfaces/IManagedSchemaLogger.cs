namespace dotPeekser.SC.Solr.ManagedSchema.Interfaces
{
    public interface IManagedSchemaLogger
    {
        void Debug(string message, object owner);
        void Warn(string message, object owner);
        void Error(string message, object owner);
    }
}
