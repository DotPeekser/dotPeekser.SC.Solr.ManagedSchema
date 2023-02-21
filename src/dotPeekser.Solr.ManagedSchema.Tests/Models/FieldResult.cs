namespace dotPeekser.Solr.ManagedSchema.Tests.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class FieldResult
    {
        private const string ENTRY_PATH = "$.*";

        private readonly JToken _jtoken;

        public FieldResult(string solrCommandData)
        {
            this._jtoken = JsonConvert.DeserializeObject<JToken>(solrCommandData);

            this.Setup();
        }

        public string Command { get; private set; }
        public string Type { get; private set; }
        public bool? Indexed { get; private set; }
        public bool? Stored { get; private set; }

        private void Setup()
        {
            this.Command = (this._jtoken.First as JProperty)?.Name;

            this.Type = (this._jtoken.SelectToken($"{ENTRY_PATH}.{ManagedSchemaConstants.SolrGeneralField.Type}") as JValue)?.Value<string>();
            this.Indexed = (this._jtoken.SelectToken($"{ENTRY_PATH}.{ManagedSchemaConstants.SolrField.Indexed}") as JValue)?.Value<bool>();
            this.Stored = (this._jtoken.SelectToken($"{ENTRY_PATH}.{ManagedSchemaConstants.SolrField.Stored}") as JValue)?.Value<bool>();
        }
    }
}
