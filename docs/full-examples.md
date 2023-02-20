# Full Examples

**Add / Replace / Remove a Field**

```xml
<!-- ADD / REPLACE -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Adds / Replace (if exists) a field -->
          <Field>
            <name>autosuggest</name>
            <type>string</type>
            <indexed>false</indexed>
            <stored>true</stored>
          </Field>
        </commands>
      </customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>

<!-- REMOVE -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Removes the __boost field -->
          <Field delete="true">
            <name>__boost</name>
          </Field>
        </commands>
      </customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>
```

**Add / Replace / Remove a Dynamic Field**

```xml
<!-- ADD / REPLACE -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Adds / Replace (if exists) a dynamic field -->
          <DynamicField>
            <name>*_ls_en</name>
            <type>lang_string</type>
            <indexed>true</indexed>
            <stored>true</stored>
          </DynamicField>
        </commands>
      </customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>

<!-- REMOVE -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Removes the *_ls_en dynamic field -->
          <DynamicField delete="true">
            <name>*_ls_en</name>
          </DynamicField>
        </commands>
      </customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>
```

**Add / Remove a Copy Field**

```xml
<!-- ADD -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <solr.customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Adds a copyfield -->
          <CopyField>
            <source>compiledcontentfield_t</source>
            <dest>autosuggest</dest>
          </CopyField>
          
      <!-- Adds a copyfield with multiple destinations -->
          <CopyField>
            <source>compiledcontentfield_t</source>
            <dest>autosuggest1</dest>
            <dest>autosuggest2</dest>
          </CopyField>
        </commands>
      </solr.customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>

<!-- REMOVE -->
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Removes a copyfield -->
          <CopyField delete="true">
            <source>compiledcontentfield_t</source>
            <dest>autosuggest</dest>
          </CopyField>
        </commands>
      </customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>
``´

**Multiple commands

```xml
<configuration xmlns:patch="http://www.sitecore.net/xmlconfig/">
  <sitecore>
    <contentSearch>
      <solr.customSolrManagedSchema>
        <commands applyToIndex="sitecore_master_index">
          <!-- Remove __boost field -->
          <Field delete="true">
            <name>__boost</name>
           </Field>
         </commands>
               
        <commands applyToIndex="my_custom_index">
          <!-- Add a copyfield -->
          <CopyField>
            <source>compositecontent_t</source>
            <dest>autosuggest</dest>
          </CopyField>
        </commands>
      </solr.customSolrManagedSchema>
    </contentSearch>
  </sitecore>
</configuration>
```