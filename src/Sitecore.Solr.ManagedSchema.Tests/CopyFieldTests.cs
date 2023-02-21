namespace Sitecore.Solr.ManagedSchema.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
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

    public class CopyFieldTests
    {
        private readonly IManagedSchemaLogger _managedSchemaLogger = new TestManagedSchemaLogger();
        private string _baseFieldsTestDataPath;

        [OneTimeSetUp]
        public void Setup()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            string appPath = Assembly.GetExecutingAssembly().Location;
            string appDir = Path.GetDirectoryName(appPath);

            this._baseFieldsTestDataPath = appDir + "/App_Data/CopyFields";
        }

        [Test]
        public void Serialize_CopyField_Add_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-copyfield.config";

            SolrSchema schema = new SolrSchema();

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            CopyFieldResult result = new CopyFieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.CopyField, Operation.Add);

            Assert.AreEqual(result.Command, command);
        }

        [Test]
        public void Serialize_CopyField_Delete_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-delete-copyfield.config";

            SolrSchema schema = new SolrSchema();
            schema.SolrCopyFields.Add(new SolrCopyField("compositecontentfield_t", "autosuggest"));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            CopyFieldResult result = new CopyFieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.CopyField, Operation.Delete);

            Assert.AreEqual(result.Command, command);
        }

        /// <summary>
        /// Non existing copy fields in the solrschema are not valid to delete.
        /// </summary>
        [Test]
        public void Serialize_CopyField_Delete_Command_Not_Exists_Is_Empty()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-delete-copyfield.config";

            SolrSchema schema = new SolrSchema();
            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);
            IEnumerable<XElement> elements = schemaPopulateHelper.GetAllFields();

            Assert.NotNull(elements);
            Assert.AreEqual(elements.Count(), 0);
        }

        private ISchemaPopulateHelper GetHelper(SolrSchema schema, string indexName, string configFilePath)
        {
            ISchemaPopulateHelper schemaPopulateHelper = new CustomSchemaPopulateHelper(
                schema, indexName, new XmlReaderFactory(configFilePath), this._managedSchemaLogger);

            return schemaPopulateHelper;
        }
    }
}