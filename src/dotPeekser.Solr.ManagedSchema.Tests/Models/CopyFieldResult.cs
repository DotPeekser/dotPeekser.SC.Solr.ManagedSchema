namespace dotPeekser.Solr.ManagedSchema.Tests.Models
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class CopyFieldResult
    {
        private const string ENTRY_PATH = "$.*";

        private readonly JToken _jtoken;

        public CopyFieldResult(string solrCommandData)
        {
            this._jtoken = JsonConvert.DeserializeObject<JToken>(solrCommandData);

            this.Setup();
        }

        public string Command { get; private set; }
        public string Source { get; private set; }
        public string Destination { get; private set; }

        private void Setup()
        {
            this.Command = (this._jtoken.First as JProperty)?.Name;

            this.Source = (this._jtoken.SelectToken($"{CopyFieldResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrCopyField.Source}") as JValue)?.Value<string>();
            this.Destination = (this._jtoken.SelectToken($"{CopyFieldResult.ENTRY_PATH}.{ManagedSchemaConstants.SolrCopyField.Dest}") as JValue)?.Value<string>();
        }
    }
}
