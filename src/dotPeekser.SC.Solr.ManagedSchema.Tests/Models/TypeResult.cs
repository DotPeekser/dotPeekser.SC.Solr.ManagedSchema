namespace dotPeekser.SC.Solr.ManagedSchema.Tests.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class TypeResult
    {
        private const string ENTRY_PATH = "$.*";

        private readonly JToken _jtoken;

        public TypeResult(string solrCommandData)
        {
            this._jtoken = JsonConvert.DeserializeObject<JToken>(solrCommandData);

            this.Setup();
        }

        public string Command { get; private set; }
        public string Class { get; private set; }
        public AnalyzerResult QueryAnalyzers { get; private set; }
        public AnalyzerResult IndexAnalyzers { get; private set; }
        public AnalyzerResult Analyzers { get; private set; }

        private void Setup()
        {
            this.Command = (this._jtoken.First as JProperty)?.Name;

            /* '*' because the first property can be any of the command like add-field / replace-field etc. */
            this.Class = (this._jtoken.SelectToken($"{TypeResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrType.Class}") as JValue)?.Value<string>();
            this.IndexAnalyzers = this.GetAnalyzerResult($"{TypeResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrType.IndexAnalyzer}");
            this.QueryAnalyzers = this.GetAnalyzerResult($"{TypeResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrType.QueryAnalyzer}");
            this.Analyzers = this.GetAnalyzerResult($"{TypeResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrType.Analyzer}");
        }

        private AnalyzerResult GetAnalyzerResult(string selectToken)
        {
            if (!(this._jtoken.SelectToken(selectToken) is JObject analyzerResultObj))
            {
                return null;
            }

            return new AnalyzerResult(analyzerResultObj);
        }
    }
}
