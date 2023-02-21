namespace dotPeekser.Solr.ManagedSchema.PopulateSolrSchema
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using CommonServiceLocator;
    using Sitecore.ContentSearch.SolrNetExtension;
    using Sitecore.ContentSearch.SolrProvider.Pipelines.PopulateSolrSchema;
    using Sitecore.ContentSearch.SolrProvider.SolrConnectors;
    using Sitecore.ContentSearch.SolrProvider.SolrOperations;
    using Sitecore.Diagnostics;
    using dotPeekser.Solr.ManagedSchema.Helpers;
    using dotPeekser.Solr.ManagedSchema.Interfaces;
    using SolrNet.Schema;

    public class CustomPopulateFields : PopulateManagedSchemaProcessor
    {
        private readonly ISolrConnector _solrConnector;
        private readonly IXmlReaderFactory _xmlReaderFactory;
        private readonly IManagedSchemaLogger _managedSchemaLogger;

        public CustomPopulateFields(IXmlReaderFactory xmlReaderFactory, IManagedSchemaLogger managedSchemaLogger)
        {
            this._managedSchemaLogger = managedSchemaLogger;
            this._xmlReaderFactory = xmlReaderFactory;

            /* Not working through constructor DI */
            this._solrConnector = ServiceLocator.Current.GetInstance<ISolrConnector>();
        }

        private string _indexName;

        public override void Process(PopulateManagedSchemaArgs args)
        {
            this._indexName = args.Index.Name;

            ISolrOperationsFactory operationsFactory = this._solrConnector.OperationsFactory;

            foreach (string core in args.Cores)
            {
                this.PopulateCore(core, operationsFactory);
            }
        }

        private void PopulateCore(string coreName, ISolrOperationsFactory solrOperationsFactory)
        {
            Assert.ArgumentNotNullOrEmpty(coreName, "coreName");
            ISolrOperationsEx<Dictionary<string, object>> solrOperations = solrOperationsFactory.GetSolrOperations(coreName);
            Assert.IsNotNull(solrOperations, "operations");
            this.DoPopulateFieldTypes(solrOperations);
            this.DoPopulateFields(solrOperations);
        }

        private void DoPopulateFieldTypes(ISolrOperationsEx<Dictionary<string, object>> operations)
        {
            ISchemaPopulateHelper schemaPopulateHelper = this.GetSchemaPopulateHelper(operations);
            Assert.IsNotNull(schemaPopulateHelper, "helper");
            this.SendCommands(operations, schemaPopulateHelper.GetAllFieldTypes().ToList());
        }

        private void DoPopulateFields(ISolrOperationsEx<Dictionary<string, object>> operations)
        {
            ISchemaPopulateHelper schemaPopulateHelper = this.GetSchemaPopulateHelper(operations);
            Assert.IsNotNull(schemaPopulateHelper, "helper");
            this.SendCommands(operations, schemaPopulateHelper.GetAllFields().ToList());
        }

        private ISchemaPopulateHelper GetSchemaPopulateHelper(ISolrOperationsEx<Dictionary<string, object>> operations)
        {
            SolrSchema managedSchema = operations.GetManagedSchema();
            Assert.IsNotNull(managedSchema, "schema");
            return new CustomSchemaPopulateHelper(managedSchema, this._indexName, this._xmlReaderFactory, this._managedSchemaLogger);
        }

        private void SendCommands(ISolrOperationsEx<Dictionary<string, object>> operations, List<XElement> commands)
        {
            Assert.ArgumentNotNull(operations, "operations");
            Assert.ArgumentNotNull(commands, "commands");
            if (!commands.Any())
            {
                Log.Warn("The list of fields to be populated is empty! Populate job will terminate.", this);
            }
            else
            {
                operations.UpdateSchema(commands);
            }
        }
    }
}
