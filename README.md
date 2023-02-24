# dotPeekser.SC.Solr.ManagedSchema

[![NuGet version (dotPeekser.SC.Solr.ManagedSchema)](https://img.shields.io/nuget/v/dotPeekser.SC.Solr.ManagedSchema.svg?style=flat-square)](https://www.nuget.org/packages/dotPeekser.SC.Solr.ManagedSchema/)
[![dotPeekser.SC.Solr.ManagedSchema License](https://img.shields.io/github/license/DotPeekser/dotPeekser.SC.Solr.ManagedSchema?color=blue&style=flat-square  )](/LICENSE)
<br />
Provides a processor to configure the managed-schema through Sitecore.  
The definition is based on the solr documentation: https://solr.apache.org/guide/8_2/schema-api.html  

If a field or type already exists in solr it will be replaced. If you want to delete a specific field, the node needs to have the delete attribute with the value true in place. delete="true"

> Sitecore removes all fields and types by default on every “Populate Schema” process and populates it with his own configured data.
> This documentation provides a flexible way to add dynamically new definitions to the managed-schema file.
<br />

> Caveat
> Example: If you configure an auto suggest handler which references a new created type you have to configure the new type in the managed-schema file even if its defined with this configuration.
> If you don't do it you will have a corrupt managed-schema file because solr checks the file before Sitecore populates it. Workaround: Configure it in both places to ensure the new field still exists after populate.

Compatibility list  

| Sitecore Version | Project Version |
|------------------|-----------------|
| 10.2             | >=1.0.0         |


# Base Configuration

## Base node for the configuration

```xml
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <solr.customSolrManagedSchema>
        <!-- YOUR SCHEMA CONFIGURATION -->
      </solr.customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>
```

To apply field or type configurations, you have to define a `command` to reference the fields to one or multiple indexes.

To apply the configurations to a specific index, for example `sitecore_master_index` and `sitecore_web_index` you have to set the `applyToIndex` attribute.
Each index have to be separated through a pipe character `|`.  
If you want to apply your configuration to all indicies you can set the value to all.

```xml
<commands applyToIndex="sitecore_master_index|sitecore_web_index">
</command>
```

**Available Command Node Attributes**

`applyToIndex` - Which index should be modified with the configured field and types

**Supported Field / Types**

`DynamicField`, `CopyField`, `Field`, `Type`

**Supported Field / Type Attributes**

`delete="true"` - Definition if a field or type should be deleted

# Examples (Snippets)

**Dynamic Field**  
*Add two dynamic fields `*_ls_en` and `*_ls_de`*

```xml
<DynamicField>
  <name>*_ls_en</name>
  <type>lang_string</type>
  <indexed>true</indexed>
  <stored>true</stored>
</DynamicField>

<DynamicField>
  <name>*_ls_de</name>
  <type>lang_string</type>
  <indexed>true</indexed>
  <stored>true</stored>
</DynamicField>
```

**Copy Field**  
*Add a CopyField*

```xml
<DynamicField>
  <name>*_ls_en</name>
  <type>lang_string</type>
  <indexed>true</indexed>
  <stored>true</stored>
</DynamicField>

<DynamicField>
  <name>*_ls_de</name>
  <type>lang_string</type>
  <indexed>true</indexed>
  <stored>true</stored>
</DynamicField>
```

**Field Type**  
*Configuration to replace `text_en` and `text_de` to have separate synonym.txt file for each language.  
Ensure that all referenced filesystem files (ex. `synonyms_de.txt`) are placed in the correct solr folder before applying this configuration, otherwise this will have no effect! Sitecore does not show this as an error.*

```xml
<!-- Can only be successful if the referenced files (ex. synonyms_de.txt) already exists in the solr folder -->
<Type>
  <name>text_de</name>
  <class>solr.TextField</class>
  <positionIncrementGap>100</positionIncrementGap>
  <indexAnalyzer>
    <tokenizer>
      <class>solr.StandardTokenizerFactory</class>
    </tokenizer>
    <filters>
      <class>solr.LowerCaseFilterFactory</class>
    </filters>
    <filters>
      <class>solr.StopFilterFactory</class>
      <format>snowball</format>
      <words>lang/stopwords_de.txt</words>
      <ignoreCase>true</ignoreCase>
    </filters>
    <filters>
      <class>solr.GermanNormalizationFilterFactory</class>
    </filters>
    <filters>
      <class>solr.GermanLightStemFilterFactory</class>
    </filters>
  </indexAnalyzer>
  <queryAnalyzer>
    <tokenizer>
      <class>solr.StandardTokenizerFactory</class>
    </tokenizer>
    <filters>
      <class>solr.LowerCaseFilterFactory</class>
    </filters>
    <filters>
      <class>solr.SynonymGraphFilterFactory</class>
      <expand>true</expand>
      <ignoreCase>true</ignoreCase>
      <synonyms>lang/synonyms_de.txt</synonyms>
    </filters>
    <filters>
      <class>solr.StopFilterFactory</class>
      <format>snowball</format>
      <words>lang/stopwords_de.txt</words>
      <ignoreCase>true</ignoreCase>
    </filters>
    <filters>
      <class>solr.GermanNormalizationFilterFactory</class>
    </filters>
    <filters>
      <class>solr.GermanLightStemFilterFactory</class>
    </filters>
  </queryAnalyzer>
</Type>

<Type>
  <name>text_en</name>
  <class>solr.TextField</class>
  <positionIncrementGap>100</positionIncrementGap>
  <indexAnalyzer>
    <tokenizer>
      <class>solr.StandardTokenizerFactory</class>
    </tokenizer>
    <filters>
      <class>solr.StopFilterFactory</class>
      <words>lang/stopwords_en.txt</words>
      <ignoreCase>true</ignoreCase>
    </filters>
    <filters>
      <class>solr.LowerCaseFilterFactory</class>
    </filters>
    <filters>
      <class>solr.EnglishPossessiveFilterFactory</class>
    </filters>
    <filters>
      <class>solr.KeywordMarkerFilterFactory</class>
      <protected>protwords.txt</protected>
    </filters>
    <filters>
      <class>solr.PorterStemFilterFactory</class>
    </filters>
  </indexAnalyzer>
  <queryAnalyzer>
    <tokenizer>
      <class>solr.StandardTokenizerFactory</class>
    </tokenizer>
    <filters>
      <class>solr.SynonymGraphFilterFactory</class>
      <expand>true</expand>
      <ignoreCase>true</ignoreCase>
      <synonyms>lang/synonyms_en.txt</synonyms>
    </filters>
    <filters>
      <class>solr.StopFilterFactory</class>
      <words>lang/stopwords_en.txt</words>
      <ignoreCase>true</ignoreCase>
    </filters>
    <filters>
      <class>solr.LowerCaseFilterFactory</class>
    </filters>
    <filters>
      <class>solr.EnglishPossessiveFilterFactory</class>
    </filters>
    <filters>
      <class>solr.KeywordMarkerFilterFactory</class>
      <protected>protwords.txt</protected>
    </filters>
    <filters>
      <class>solr.PorterStemFilterFactory</class>
    </filters>
  </queryAnalyzer>
</Type>
```

For full examples take a look on this link. [Click here](docs/full-examples.md).