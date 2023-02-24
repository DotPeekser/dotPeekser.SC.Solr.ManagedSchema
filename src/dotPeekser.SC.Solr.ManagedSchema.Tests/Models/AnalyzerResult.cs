namespace dotPeekser.SC.Solr.ManagedSchema.Tests.Models
{
    using Newtonsoft.Json.Linq;

    internal class AnalyzerResult
    {
        private readonly JObject _jobject;

        public AnalyzerResult(JObject obj)
        {
            this._jobject = obj;

            this.Setup();
        }

        /* Filters must be an array. Solr only accept filter arrays */
        public JArray Filters { get; private set; }
        /* Can be either an array or a single entry */
        public JToken Tokenizers { get; private set; }
        public bool IsTokenizersArray => this.Tokenizers is JArray;

        private void Setup()
        {
            this.Filters = this._jobject.SelectToken($"$.{ManagedSchemaConstants.SolrType.Filters}") as JArray;
            this.Tokenizers = this._jobject.SelectToken($"$.{ManagedSchemaConstants.SolrType.Tokenizer}");
        }
    }
}
