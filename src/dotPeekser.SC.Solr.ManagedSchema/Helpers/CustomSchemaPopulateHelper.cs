namespace dotPeekser.SC.Solr.ManagedSchema.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Xml;
    using System.Xml.Linq;
    using Microsoft.Practices.EnterpriseLibrary.Common.Utility;
    using Sitecore.ContentSearch.Linq.Utilities;
    using Sitecore.ContentSearch.SolrProvider.Pipelines.PopulateSolrSchema;
    using dotPeekser.SC.Solr.ManagedSchema.Data;
    using dotPeekser.SC.Solr.ManagedSchema.Extensions;
    using dotPeekser.SC.Solr.ManagedSchema.Interfaces;
    using Sitecore.Xml;
    using SolrNet.Schema;

    public class CustomSchemaPopulateHelper : ISchemaPopulateHelper
    {
        private static readonly XNamespace JsonNamespace = "http://james.newtonking.com/projects/json";

        private readonly SolrSchema _solrSchema;
        private readonly string _indexName;
        private readonly IXmlReaderFactory _xmlReaderFactory;
        private readonly IManagedSchemaLogger _managedSchemaLogger;

        private readonly ConfigNodeType[] NodeFields = {
            ConfigNodeType.Field,
            ConfigNodeType.DynamicField,
            ConfigNodeType.CopyField
        };

        private readonly ConfigNodeType[] NodeTypes = {
            ConfigNodeType.Type
        };

        public CustomSchemaPopulateHelper(SolrSchema solrSchema, string indexName, IXmlReaderFactory xmlReaderFactory, IManagedSchemaLogger managedSchemaLogger)
        {
            this._managedSchemaLogger = managedSchemaLogger;
            this._solrSchema = solrSchema;
            this._indexName = indexName;
            this._xmlReaderFactory = xmlReaderFactory;
        }

        public IEnumerable<XElement> GetAllFields()
        {
            IEnumerable<XElement> entries =
                this.LoadElementsByConfigNodeTypes(this.NodeFields);

            return entries;
        }

        public IEnumerable<XElement> GetAllFieldTypes()
        {
            IEnumerable<XElement> entries =
                this.LoadElementsByConfigNodeTypes(this.NodeTypes);

            return entries;
        }

        #region Private Methods

        /// <summary>
        /// Loads the elements by specific config node types.
        /// </summary>
        /// <param name="nodeTypes"></param>
        /// <returns></returns>
        private IEnumerable<XElement> LoadElementsByConfigNodeTypes(
            params ConfigNodeType[] nodeTypes)
        {
            Expression<Func<XmlNode, bool>> exp = PredicateBuilder.False<XmlNode>();

            foreach (ConfigNodeType item in nodeTypes)
            {
                string nodeType = item.ToString().ToLower();

                exp = exp.Or(x => x.Name.ToLower() == nodeType);
            }

            return this.LoadElements(exp).Select(x => x.Value);
        }

        /// <summary>
        /// Loads the specific elements from the xml config.
        /// </summary>
        /// <param name="predicate">
        /// Predicate to filter the elements.
        /// </param>
        /// <returns>
        /// Returns the specific elements.
        /// </returns>
        private List<KeyValuePair<ConfigNodeType, XElement>> LoadElements(
            Expression<Func<XmlNode, bool>> predicate)
        {
            List<KeyValuePair<ConfigNodeType, XElement>> elements = new();

            XmlNodeList commands = this._xmlReaderFactory.GetConfigNodes(ManagedSchemaConstants.CommandsXmlPath);

            foreach (XmlNode command in commands)
            {
                if (!this.IsApplicable(command, this._indexName))
                {
                    this._managedSchemaLogger.Debug(
                        $"Skipping command with applyToIndex '{XmlUtil.GetAttribute("applyToIndex", command)}' because its not applicable with the current index name '{this._indexName}'.",
                        this);

                    continue;
                }

                IEnumerable<XmlNode> entries = command.ChildNodes.Cast<XmlNode>();

                foreach (XmlNode entry in entries.AsQueryable().Where(predicate))
                {
                    entry.RemoveComments();

                    KeyValuePair<ConfigNodeType, XElement> pair =
                        this.GetElement(XElement.Parse(entry.OuterXml));

                    if (pair.Equals(default) || pair.Value == null)
                    {
                        continue;
                    }

                    elements.Add(pair);
                }
            }

            return elements;
        }

        /// <summary>
        /// Gets the specific element based on the base element.
        /// </summary>
        /// <param name="baseElement">
        /// The base element.
        /// </param>
        /// <returns>
        /// Returns the specific element.
        /// </returns>
        private KeyValuePair<ConfigNodeType, XElement> GetElement(
            XElement baseElement)
        {
            string name = baseElement.Name.ToString();

            if (!Enum.TryParse(name, out ConfigNodeType type))
            {
                string allowedNodeTypes = string.Join(",", Enum.GetValues(typeof(ConfigNodeType)));

                this._managedSchemaLogger.Error(
                    $"Element name does not match with one of the defined config node types. Name: {name} - Valid names: {allowedNodeTypes}",
                    this);

                return default;
            }

            XElement element = null;

            switch (type)
            {
                case ConfigNodeType.Type:
                    element = this.CreateFieldType(baseElement, type);
                    break;
                case ConfigNodeType.Field:
                case ConfigNodeType.DynamicField:
                    element = this.CreateField(baseElement, type);
                    break;
                case ConfigNodeType.CopyField:
                    element = this.CreateCopyField(baseElement, type);
                    break;
            }

            return element == null
                ? default
                : new KeyValuePair<ConfigNodeType, XElement>(type, element);
        }

        /// <summary>
        /// Creates an element for the specific config node type.
        /// </summary>
        /// <param name="element">
        /// The base element.
        /// </param>
        /// <param name="type">
        /// The config node type.
        /// </param>
        /// <returns>
        /// Returns an element based on the config node type.
        /// </returns>
        private XElement CreateElement(XElement element, ConfigNodeType type)
        {
            string nameValue = this.GetNameValue(element);

            if (string.IsNullOrEmpty(nameValue))
            {
                this._managedSchemaLogger.Error("The element needs a valid name to be created.", this);

                return null;
            }

            Operation operation = this.GetOperation(element, type);
            string command = CommandHelper.GetCommand(type, operation);

            XElement finalElement;

            if (operation == Operation.Delete)
            {
                finalElement = new XElement(command);
                finalElement.Add(new XElement(ManagedSchemaConstants.SolrGeneralField.Name, nameValue));
            }
            else
            {
                finalElement = new XElement(
                    command, this.GetJsonAttribute(), element.Attributes(), element.Elements());

                XName arrayName = XName.Get("Array", CustomSchemaPopulateHelper.JsonNamespace.NamespaceName);

                /* Ensure filters and charFilters are always an array (Only works with the json attribute set on the element. */
                finalElement.Descendants("filters").ForEach(f =>
                {
                    f.SetAttributeValue(arrayName, "true");
                });

                finalElement.Descendants("charFilters").ForEach(f =>
                {
                    f.SetAttributeValue(arrayName, "true");
                });
            }

            return finalElement;
        }

        private XAttribute GetJsonAttribute()
        {
            return new(XNamespace.Xmlns + "json", CustomSchemaPopulateHelper.JsonNamespace.NamespaceName);
        }

        /// <summary>
        /// Creates the field type. (field or dynamic field)
        /// </summary>
        /// <param name="element">
        /// The base element.
        /// </param>
        /// <param name="type">
        /// The config node type.
        /// (Should be <see cref="ConfigNodeType.Field"/> or 
        /// <see cref="ConfigNodeType.Field"/>)
        /// </param>
        /// <returns></returns>
        private XElement CreateField(XElement element, ConfigNodeType type)
        {
            string typeValue = this.GetValueFromElement(element, ManagedSchemaConstants.SolrGeneralField.Type);

            if (!this.TypeExists(typeValue))
            {
                // Fields without a defined type are not valid.
                this._managedSchemaLogger.Warn(
                    $"Can't create the field because the defined type doesn't exists in the solr schema. Missing type: {typeValue}",
                    this);

                return null;
            }

            return this.CreateElement(element, type);
        }

        public XElement CreateCopyField(XElement element, ConfigNodeType type)
        {
            Operation operation = this.HasDeleteFlag(element) ? Operation.Delete : Operation.Add;

            string command = CommandHelper.GetCommand(type, operation);
            string source = this.GetValueFromElement(element, ManagedSchemaConstants.SolrCopyField.Source);
            string destinations = string.Join(", ", element.Elements(ManagedSchemaConstants.SolrCopyField.Dest).Select(x => x.Value));

            if (operation == Operation.Delete)
            {
                if (!this.CopyFieldExists(source, destinations))
                {
                    return null;
                }
            }
            else
            {
                if (this.CopyFieldExists(source, destinations))
                {
                    return null;
                }
            }

            XElement finalElement = new(command, element.Elements());

            return finalElement;
        }

        /// <summary>
        /// Creates the field type element.
        /// </summary>
        /// <param name="element">
        /// The base element.
        /// (Should be <see cref="ConfigNodeType.Type"/>)
        /// </param>
        /// <param name="type">
        /// The config node type.
        /// </param>
        /// <returns></returns>
        private XElement CreateFieldType(XElement element, ConfigNodeType type)
        {
            Operation operation = this.GetOperation(element, type);
            string nameValue = this.GetNameValue(element);

            if (operation == Operation.Delete && this.TypeExists(nameValue) is false)
            {
                this._managedSchemaLogger.Warn(
                    $"Skipping the delete operation for '{nameValue}' because the current type doesn't exists in the solr schema.",
                    this);

                return null;
            }

            return this.CreateElement(element, type);
        }

        /// <summary>
        /// Gets the operation from the name.
        /// </summary>
        /// <param name="element">
        /// The base element
        /// </param>
        /// <param name="type">
        /// Th config node type.
        /// </param>
        /// <returns>
        /// Returns the operation for the name and type.
        /// </returns>
        private Operation GetOperation(XElement element, ConfigNodeType type)
        {
            if (this.HasDeleteFlag(element))
            {
                return Operation.Delete;
            }

            string name = this.GetNameValue(element);
            Operation operation = Operation.Add;

            if ((ConfigNodeType.Type == type && this.TypeExists(name)) ||
                (ConfigNodeType.Field == type && this.FieldExists(name)) ||
                (ConfigNodeType.DynamicField == type && this.DynamicFieldExists(name)))
            {
                operation = Operation.Replace;
            }

            return operation;
        }

        /// <summary>
        /// Checks if the type already exists in the solr schema.
        /// </summary>
        /// <param name="name">
        /// The name of the type.
        /// </param>
        /// <returns>
        /// Returns true if the type name exists otherwise false.
        /// </returns>
        private bool TypeExists(string name)
        {
            return this._solrSchema.FindSolrFieldTypeByName(name) != null;
        }

        /// <summary>
        /// Checks if the element has the delete attribute with a true value.
        /// </summary>
        /// <param name="element">
        /// The element to check.
        /// </param>
        /// <returns>
        /// Returns true if the delete attribute has the 
        /// value "true" otherwise false.
        /// </returns>
        private bool HasDeleteFlag(XElement element)
        {
            XAttribute deleteAttr = element.Attribute(ManagedSchemaConstants.Node.DeleteAttribute);

            return bool.TryParse(deleteAttr?.Value, out bool hasDelete) && hasDelete;
        }

        /// <summary>
        /// Checks if the field name already exists in the solr schema.
        /// </summary>
        /// <param name="name">
        /// The name of the field.
        /// </param>
        /// <returns>
        /// Returns true if the field name already exists otherwise false.
        /// </returns>
        private bool FieldExists(string name)
        {
            return this._solrSchema.FindSolrFieldByName(name) != null;
        }

        /// <summary>
        /// Checks if the dynamic field name already exists in the solr schema.
        /// </summary>
        /// <param name="name">
        /// The name of the dynamic field.
        /// </param>
        /// <returns>
        /// Returns true if the dynamic field name already exists 
        /// otherwise false.
        /// </returns>
        private bool DynamicFieldExists(string name)
        {
            return this._solrSchema.SolrDynamicFields.Any(x => x.Name == name);
        }

        private bool CopyFieldExists(string source, string dest)
        {
            return this._solrSchema.SolrCopyFields.Any(x => x.Source == source && x.Destination == dest);
        }

        /// <summary>
        /// Gets the value from the name tag.
        /// </summary>
        /// <param name="element">
        /// The element.
        /// </param>
        /// <returns>
        /// Returns the value from the "name" tag.
        /// </returns>
        private string GetNameValue(XElement element)
        {
            return this.GetValueFromElement(element, ManagedSchemaConstants.SolrGeneralField.Name);
        }

        private string GetValueFromElement(XElement element, string name)
        {
            return element.Element(name)?.Value;
        }

        /// <summary>
        /// Checks if the applyToIndex attribute has a valid value.
        /// </summary>
        /// <param name="command">
        /// The xml node.
        /// </param>
        /// <param name="indexName">
        /// The index name to populate with the new data.
        /// </param>
        /// <returns>
        /// Returns false if the applyToIndex attribute is 
        /// not defined, empty or has not the appropriate index defined.
        /// </returns>
        private bool IsApplicable(XmlNode command, string indexName)
        {
            string indicies = XmlUtil.GetAttribute(ManagedSchemaConstants.Node.ApplyToIndexAttribute, command);

            if (string.IsNullOrEmpty(indicies))
            {
                return false;
            }

            if (indicies.ToLower() == "all")
            {
                return true;
            }

            return indicies.Split('|').Any(i => i.ToLower().Equals(indexName.ToLower()));
        }

        #endregion
    }
}
