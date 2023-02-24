namespace dotPeekser.SC.Solr.ManagedSchema.Tests
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Xml.Linq;
    using dotPeekser.SC.Solr.ManagedSchema.Data;
    using dotPeekser.SC.Solr.ManagedSchema.Helpers;
    using dotPeekser.SC.Solr.ManagedSchema.Interfaces;
    using dotPeekser.SC.Solr.ManagedSchema.Tests.Models;
    using dotPeekser.SC.Solr.ManagedSchemaTests.Factories;
    using dotPeekser.SC.Solr.ManagedSchemaTests.Logging;
    using dotPeekser.SC.Solr.ManagedSchemaTests.Serializer;
    using NUnit.Framework;
    using Sitecore.ContentSearch.SolrProvider.Pipelines.PopulateSolrSchema;
    using SolrNet.Schema;

    public class FieldTests
    {
        private readonly IManagedSchemaLogger _managedSchemaLogger = new TestManagedSchemaLogger();
        private string _baseFieldsTestDataPath;

        [OneTimeSetUp]
        public void Setup()
        {
            Trace.Listeners.Add(new ConsoleTraceListener());

            string appPath = Assembly.GetExecutingAssembly().Location;
            string appDir = Path.GetDirectoryName(appPath);

            this._baseFieldsTestDataPath = appDir + "/App_Data/Fields";
        }

        [Test]
        public void Serialize_Field_Add_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-field.config";

            SolrSchema schema = new SolrSchema();
            schema.SolrFieldTypes.Add(new SolrFieldType("string", "solr.StrField"));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            FieldResult result = new FieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Field, Operation.Add);

            Assert.AreEqual(result.Command, command);
        }

        [Test]
        public void Serialize_Field_Add_Command_No_Index_No_Stored_Values()
        {
            string configFile = this._baseFieldsTestDataPath + "/minimal-single-field.config";

            SolrSchema schema = new SolrSchema();
            schema.SolrFieldTypes.Add(new SolrFieldType("string", "solr.StrField"));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            FieldResult result = new FieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Field, Operation.Add);

            Assert.AreEqual(result.Command, command);
            Assert.IsNull(result.Indexed);
            Assert.IsNull(result.Stored);
        }

        [Test]
        public void Serialize_Field_Add_Command_Indexed_Is_False_And_Stored_Is_True()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-field.config";

            SolrSchema schema = new SolrSchema();
            schema.SolrFieldTypes.Add(new SolrFieldType("string", "solr.StrField"));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();

            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            FieldResult result = new FieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Field, Operation.Add);

            Assert.AreEqual(result.Command, command);
            Assert.IsFalse(result.Indexed);
            Assert.IsTrue(result.Stored);
        }

        [Test]
        public void Serialize_Field_Replace_Command()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-field.config";

            SolrSchema schema = new SolrSchema();
            SolrFieldType stringSolrType = new SolrFieldType("string", "solr.StrField");
            schema.SolrFieldTypes.Add(stringSolrType);
            schema.SolrFields.Add(new SolrField("mySimpleField", stringSolrType));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            FieldResult result = new FieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Field, Operation.Replace);

            Assert.AreEqual(result.Command, command);
        }

        [Test]
        public void Serialize_Field_Replace_Command_Type_Switch()
        {
            string configFile = this._baseFieldsTestDataPath + "/single-field.config";

            SolrSchema schema = new SolrSchema();
            /* Types needs to be defined in the schema otherwise its not possible to replace types of a field (Checks if the target type exists) */
            SolrFieldType stringSolrType = new SolrFieldType("string", "solr.StrField");
            SolrFieldType textSolrType = new SolrFieldType("text", "solr.TextField");
            schema.SolrFieldTypes.Add(stringSolrType);
            schema.SolrFieldTypes.Add(textSolrType);
            schema.SolrFields.Add(new SolrField("mySimpleField", textSolrType));

            ISchemaPopulateHelper schemaPopulateHelper = this.GetHelper(schema, "test_index", configFile);

            List<XElement> elements = schemaPopulateHelper.GetAllFields().ToList();
            string solrCommand = SchemaCommandSerializer.Serialize(elements);
            FieldResult result = new FieldResult(solrCommand);

            string command = CommandHelper.GetCommand(ConfigNodeType.Field, Operation.Replace);

            Assert.AreEqual(result.Command, command);
            Assert.AreEqual(result.Type, stringSolrType.Name);
        }

        private ISchemaPopulateHelper GetHelper(SolrSchema schema, string indexName, string configFilePath)
        {
            ISchemaPopulateHelper schemaPopulateHelper = new CustomSchemaPopulateHelper(
                schema, indexName, new XmlReaderFactory(configFilePath), this._managedSchemaLogger);

            return schemaPopulateHelper;
        }
    }
}