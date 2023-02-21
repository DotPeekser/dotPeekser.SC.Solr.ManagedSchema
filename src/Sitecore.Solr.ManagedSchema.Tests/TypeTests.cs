namespace Sitecore.Solr.ManagedSchema.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using Newtonsoft.Json.Linq;
    using NUnit.Framework;
    using Sitecore.ContentSearch.SolrProvider.Pipelines.PopulateSolrSchema;
    using Sitecore.Solr.ManagedSchema.Data;
    using Sitecore.Solr.ManagedSchema.Helpers;
    using Sitecore.Solr.ManagedSchema.Interfaces;
    using Sitecore.Solr.ManagedSchema.Tests.Models;
    using Sitecore.Solr.ManagedSchemaTests.Factories;
    using Sitecore.Solr.ManagedSchemaTests.Logging;
    using Sitecore.Solr.ManagedSchemaTests.Serializer;
    using SolrNet.Schema;

    public class TypeTests
    {
        private readonly IManagedSchemaLogger _managedSchemaLogger = new TestManagedSchemaLogger();
        private string _baseFieldsTestDataPath;

        [OneTimeSetUp]
        public void Setup()
        {
            /* https://docs.nunit.org/articles/vs-test-adapter/Trace-and-Debug.html */
            Trace.Listeners.Add(new ConsoleTraceListener());

            string appPath = Assembly.GetExecutingAssembly().Location;
            string appDir = Path.GetDirectoryName(appPath);

            this._baseFieldsTestDataPath = appDir + "/App_Data/Types";
        }

        [Test]
        public void Serialize_Type_Add_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-single-globalfilter.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Type, Operation.Add);

            Assert.AreEqual(result.Command, command);
        }

        [Test]
        public void Serialize_Type_Replace_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-single-globalfilter.config";

            SolrSchema schema = new SolrSchema();
            SolrFieldType strFieldType = new SolrFieldType("my_type", "solr.StrField");
            schema.SolrFieldTypes.Add(strFieldType);

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Type, Operation.Replace);

            Assert.AreEqual(result.Command, command);
            Assert.AreNotEqual(strFieldType.Type, result.Class);
            Assert.AreEqual(schema.SolrFieldTypes.Count, 1);
        }

        /// <summary>
        /// Filters must be an array to push to solr otherwise request will fail.
        /// </summary>
        [Test]
        public void Serialize_Type_Single_GlobalFilter_IsArray()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-single-globalfilter.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            Assert.NotNull(result.Analyzers?.Filters);
        }

        [Test]
        public void Serialize_Type_Multiple_GlobalFilter_Count_Is_3()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-multiple-globalfilter.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            Assert.NotNull(result.Analyzers?.Filters);
            Assert.AreEqual(result.Analyzers.Filters.Count, 3);
        }

        [Test]
        public void Serialize_Type_Multiple_IndexAnalyzer_Filters_Count_Is_4_And_Tokenizer_Count_Is_1()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-index-query-analyzer.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            Assert.NotNull(result.IndexAnalyzers?.Filters);
            Assert.AreEqual(result.IndexAnalyzers.Filters.Count, 4);

            Assert.NotNull(result.IndexAnalyzers.Tokenizers);
            Assert.AreEqual(result.IndexAnalyzers.IsTokenizersArray, false);
        }

        [Test]
        public void Serialize_Type_Multiple_QueryAnalyzer_Filters_Count_Is_5_And_Tokenizer_Count_Is_2()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-type-index-query-analyzer.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFieldTypes().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            TypeResult result = new TypeResult(solrCommand);

            JArray tokenizers = result.QueryAnalyzers.IsTokenizersArray ? result.QueryAnalyzers.Tokenizers as JArray : null;

            Assert.NotNull(result.QueryAnalyzers);
            Assert.AreEqual(result.QueryAnalyzers.Filters.Count, 5);

            Assert.AreEqual(result.QueryAnalyzers.IsTokenizersArray, true);
            Assert.NotNull(tokenizers);
            Assert.AreEqual(tokenizers.Count, 2);
        }

        private ISchemaPopulateHelper GetHelper(SolrSchema schema, string indexName, string configFilePath)
        {
            ISchemaPopulateHelper schemaPopulateHelper = new CustomSchemaPopulateHelper(
                schema, indexName, new XmlReaderFactory(configFilePath), this._managedSchemaLogger);

            return schemaPopulateHelper;
        }
    }
}