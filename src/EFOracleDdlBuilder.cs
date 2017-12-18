// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EFOracleDdlBuilder
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal sealed class EFOracleDdlBuilder
  {
    private static bool m_bCreateSequenceAndTrigger = false;
    private static bool m_bStoreGeneratedPatternComputed = false;
    private static List<string> SequenceAndTriggerList = new List<string>();
    private readonly StringBuilder stringBuilder = new StringBuilder();
    private readonly HashSet<EntitySet> ignoredEntitySets = new HashSet<EntitySet>();

    internal static string CreateObjectsScript(StoreItemCollection itemCollection, string providerManifestToken)
    {
      EFOracleDdlBuilder oracleDdlBuilder = new EFOracleDdlBuilder();
      foreach (EntityContainer entityContainer in itemCollection.GetItems<EntityContainer>())
      {
        entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name));
        foreach (EntitySet entitySet in (IEnumerable<EntitySet>) entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name)))
          oracleDdlBuilder.AppendCreateTable(entitySet, providerManifestToken);
        foreach (AssociationSet associationSet in (IEnumerable<AssociationSet>) entityContainer.BaseEntitySets.OfType<AssociationSet>().OrderBy<AssociationSet, string>((Func<AssociationSet, string>) (s => s.Name)))
          oracleDdlBuilder.AppendCreateForeignKeys(associationSet);
        for (int index = 0; index < EFOracleDdlBuilder.SequenceAndTriggerList.Count; ++index)
          oracleDdlBuilder.AppendSql(EFOracleDdlBuilder.SequenceAndTriggerList[index]);
        EFOracleDdlBuilder.SequenceAndTriggerList.Clear();
      }
      return oracleDdlBuilder.GetCommandText();
    }

    internal static string CreateDatabaseScript(string databaseName, string dataFileName, string logFileName)
    {
      return string.Empty;
    }

    internal static string CreateDatabaseExistsScript(string databaseName)
    {
      EFOracleDdlBuilder oracleDdlBuilder = new EFOracleDdlBuilder();
      oracleDdlBuilder.AppendSql("select count(*) from dual where upper(sys_context('userenv', 'db_name')) = ");
      oracleDdlBuilder.AppendStringLiteral(databaseName.ToUpper());
      return oracleDdlBuilder.stringBuilder.ToString();
    }

    internal static string DropDatabaseScript(string databaseName, StoreItemCollection itemCollection)
    {
      EFOracleDdlBuilder oracleDdlBuilder = new EFOracleDdlBuilder();
      foreach (EntityContainer entityContainer in itemCollection.GetItems<EntityContainer>())
      {
        entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name));
        foreach (EntitySet entitySet in (IEnumerable<EntitySet>) entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name)))
        {
          oracleDdlBuilder.AppendSql("drop table ");
          oracleDdlBuilder.AppendIdentifier(entitySet);
          oracleDdlBuilder.AppendSql(" cascade constraints \n\n");
          oracleDdlBuilder.AppendSql("drop sequence ");
          oracleDdlBuilder.AppendIdentifier(EFOracleDdlBuilder.GetSchemaName(entitySet));
          oracleDdlBuilder.AppendSql(".");
          string str = OracleMigrationSqlGenerator.SequencePrefix + OracleMigrationSqlGenerator.NameSeparator + EFOracleDdlBuilder.GetTableName(entitySet);
          if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
            str = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30);
          oracleDdlBuilder.AppendIdentifier(str);
          oracleDdlBuilder.AppendSql("\n\n");
        }
      }
      oracleDdlBuilder.AppendSql("drop table ");
      oracleDdlBuilder.AppendIdentifier("__MigrationHistory");
      oracleDdlBuilder.AppendSql(" cascade constraints \n\n");
      oracleDdlBuilder.AppendSql("drop table ");
      oracleDdlBuilder.AppendIdentifier("EdmMetadata");
      oracleDdlBuilder.AppendSql(" cascade constraints \n\n");
      return oracleDdlBuilder.stringBuilder.ToString();
    }

    internal string GetCommandText()
    {
      return this.stringBuilder.ToString();
    }

    private static string GetSchemaName(EntitySet entitySet)
    {
      return entitySet.MetadataProperties["Schema"].Value as string ?? entitySet.EntityContainer.Name;
    }

    private static string GetTableName(EntitySet entitySet)
    {
      return entitySet.MetadataProperties["Table"].Value as string ?? entitySet.Name;
    }

    private void AppendCreateForeignKeys(AssociationSet associationSet)
    {
      ReferentialConstraint referentialConstraint = associationSet.ElementType.ReferentialConstraints.Single<ReferentialConstraint>();
      AssociationSetEnd associationSetEnd1 = associationSet.AssociationSetEnds[referentialConstraint.FromRole.Name];
      AssociationSetEnd associationSetEnd2 = associationSet.AssociationSetEnds[referentialConstraint.ToRole.Name];
      if (this.ignoredEntitySets.Contains(associationSetEnd1.EntitySet) || this.ignoredEntitySets.Contains(associationSetEnd2.EntitySet))
      {
        this.AppendSql("-- Ignoring association set with participating entity set with defining query: ");
        this.AppendIdentifierEscapeNewLine(associationSet.Name);
      }
      else
      {
        this.AppendSql("alter table ");
        this.AppendIdentifier(associationSetEnd2.EntitySet);
        this.AppendSql(" add constraint ");
        this.AppendIdentifier(associationSet.Name);
        this.AppendSql(" foreign key (");
        this.AppendIdentifiers((IEnumerable<EdmProperty>) referentialConstraint.ToProperties);
        this.AppendSql(") references ");
        this.AppendIdentifier(associationSetEnd1.EntitySet);
        this.AppendSql("(");
        this.AppendIdentifiers((IEnumerable<EdmProperty>) referentialConstraint.FromProperties);
        this.AppendSql(")");
        if (associationSetEnd1.CorrespondingAssociationEndMember.DeleteBehavior == OperationAction.Cascade)
          this.AppendSql(" on delete cascade");
        this.AppendSql(";\n\n");
      }
      this.AppendNewLine();
    }

    private void AppendCreateTable(EntitySet entitySet, string providerManifestToken)
    {
      if (entitySet.MetadataProperties["DefiningQuery"].Value != null)
      {
        this.AppendSql("-- Ignoring entity set with defining query: ");
        this.AppendIdentifier(entitySet, new Action<string>(this.AppendIdentifierEscapeNewLine));
        this.ignoredEntitySets.Add(entitySet);
      }
      else
      {
        this.AppendSql("create table ");
        this.AppendIdentifier(entitySet);
        this.AppendSql(" (");
        this.AppendNewLine();
        foreach (EdmProperty property in entitySet.ElementType.Properties)
        {
          this.AppendSql("    ");
          this.AppendIdentifier(property.Name);
          this.AppendSql(" ");
          this.AppendType(property, providerManifestToken);
          this.AppendSql(",");
          this.AppendNewLine();
          if (EFOracleDdlBuilder.m_bCreateSequenceAndTrigger)
          {
            string tableName = EFOracleDdlBuilder.GetTableName(entitySet);
            string text = (tableName.Length <= 12 ? tableName : tableName.Substring(0, 12)) + "_" + (property.Name.Length <= 11 ? property.Name : property.Name.Substring(0, 11));
            EFOracleDdlBuilder oracleDdlBuilder1 = new EFOracleDdlBuilder();
            oracleDdlBuilder1.AppendSql("\"");
            oracleDdlBuilder1.AppendSql(text);
            oracleDdlBuilder1.AppendSql("_");
            oracleDdlBuilder1.AppendSql(property.TypeUsage.EdmType.Name.Substring(0, 2));
            oracleDdlBuilder1.AppendSql("_");
            oracleDdlBuilder1.AppendSql("sq");
            oracleDdlBuilder1.AppendSql("\"");
            EFOracleDdlBuilder oracleDdlBuilder2 = new EFOracleDdlBuilder();
            if (property.TypeUsage.EdmType.Name.ToLowerInvariant() != "guid" && property.TypeUsage.EdmType.Name.ToLowerInvariant() != "date")
            {
              oracleDdlBuilder2.AppendSql("create sequence ");
              oracleDdlBuilder2.AppendSql(oracleDdlBuilder1.GetCommandText());
              oracleDdlBuilder2.AppendSql(" start with 1");
              oracleDdlBuilder2.AppendSql(";\n\n");
            }
            oracleDdlBuilder2.AppendSql(EFOracleDdlBuilder.CreateTrigger(EFOracleDdlBuilder.GetTableName(entitySet), property, "insert", oracleDdlBuilder1.GetCommandText()));
            EFOracleDdlBuilder.m_bCreateSequenceAndTrigger = false;
            if (EFOracleDdlBuilder.m_bStoreGeneratedPatternComputed)
              oracleDdlBuilder2.AppendSql(EFOracleDdlBuilder.CreateTrigger(EFOracleDdlBuilder.GetTableName(entitySet), property, "update", oracleDdlBuilder1.GetCommandText()));
            EFOracleDdlBuilder.m_bStoreGeneratedPatternComputed = false;
            EFOracleDdlBuilder.SequenceAndTriggerList.Add(oracleDdlBuilder2.GetCommandText());
          }
        }
        this.AppendSql("CONSTRAINT \"PK_");
        this.AppendSql(EFOracleDdlBuilder.GetTableName(entitySet));
        this.AppendSql("\" primary key (");
        this.AppendJoin<EdmMember>((IEnumerable<EdmMember>) entitySet.ElementType.KeyMembers, (Action<EdmMember>) (k => this.AppendIdentifier(k.Name)), ", ");
        this.AppendSql(")");
        this.AppendNewLine();
        if (EFOracleDdlBuilder.GetTableName(entitySet) == "__MigrationHistory")
          this.AppendSql(")\n\n");
        else
          this.AppendSql(");\n\n");
      }
      this.AppendNewLine();
    }

    private void AppendCreateSchema(string schema)
    {
    }

    private void AppendIdentifier(EntitySet table)
    {
      this.AppendIdentifier(table, new Action<string>(this.AppendIdentifier));
    }

    private void AppendIdentifier(EntitySet table, Action<string> AppendIdentifierEscape)
    {
      string tableName = EFOracleDdlBuilder.GetTableName(table);
      AppendIdentifierEscape(tableName);
    }

    private void AppendStringLiteral(string literalValue)
    {
      this.AppendSql("N'" + literalValue.Replace("'", "''") + "'");
    }

    private void AppendIdentifiers(IEnumerable<EdmProperty> properties)
    {
      this.AppendJoin<EdmProperty>(properties, (Action<EdmProperty>) (p => this.AppendIdentifier(p.Name)), ", ");
    }

    private void AppendIdentifier(string identifier)
    {
      this.AppendSql("\"" + identifier.Replace("\"", "\"\"") + "\"");
    }

    private void AppendIdentifierEscapeNewLine(string identifier)
    {
      this.AppendIdentifier(identifier.Replace("\r", "\r--").Replace("\n", "\n--"));
    }

    private void AppendFileName(string path)
    {
      this.AppendSql("(name=");
      this.AppendStringLiteral(Path.GetFileName(path));
      this.AppendSql(", filename=");
      this.AppendStringLiteral(path);
      this.AppendSql(")");
    }

    private void AppendJoin<T>(IEnumerable<T> elements, Action<T> appendElement, string unencodedSeparator)
    {
      bool flag = true;
      foreach (T element in elements)
      {
        if (flag)
          flag = false;
        else
          this.AppendSql(unencodedSeparator);
        appendElement(element);
      }
    }

    private void AppendType(EdmProperty column, string providerManifestToken)
    {
      TypeUsage typeUsage = column.TypeUsage;
      bool flag = false;
      Facet facet;
      if (typeUsage.EdmType.Name == "binary" && 8 == typeUsage.GetMaxLength() && (column.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet) && facet.Value != null) && StoreGeneratedPattern.Computed == (StoreGeneratedPattern) facet.Value)
      {
        flag = true;
        this.AppendIdentifier("rowversion");
      }
      else
      {
        string name = typeUsage.EdmType.Name;
        if (name.ToLowerInvariant() == "guid")
          this.AppendSql("raw(16)");
        else if (name.ToLowerInvariant() == "pl/sql boolean")
          this.AppendSql("number(1, 0)");
        else
          this.AppendSql(name);
        switch (typeUsage.EdmType.Name)
        {
          case "number":
            this.AppendSqlInvariantFormat("({0}, {1})", (object) typeUsage.GetPrecision(), (object) typeUsage.GetScale());
            break;
          case "float":
            this.AppendSqlInvariantFormat("({0})", (object) typeUsage.GetPrecision());
            break;
          case "char":
          case "nchar":
          case "varchar2":
          case "nvarchar2":
          case "raw":
            this.AppendSqlInvariantFormat("({0})", (object) typeUsage.GetMaxLength());
            break;
        }
      }
      if (!flag && column.TypeUsage.Facets.TryGetValue("StoreGeneratedPattern", false, out facet) && facet.Value != null)
      {
        switch ((StoreGeneratedPattern) facet.Value)
        {
          case StoreGeneratedPattern.Identity:
            if (EFOracleVersionUtils.GetStorageVersion(providerManifestToken) >= EFOracleVersion.Oracle12cR1)
            {
              this.AppendSql(" generated by default as identity");
              break;
            }
            EFOracleDdlBuilder.m_bCreateSequenceAndTrigger = true;
            break;
          case StoreGeneratedPattern.Computed:
            EFOracleDdlBuilder.m_bCreateSequenceAndTrigger = true;
            EFOracleDdlBuilder.m_bStoreGeneratedPatternComputed = true;
            break;
        }
      }
      this.AppendSql(column.Nullable ? " null" : " not null");
    }

    private void AppendSql(string text)
    {
      this.stringBuilder.Append(text);
    }

    private void AppendNewLine()
    {
      this.stringBuilder.Append("\r\n");
    }

    private void AppendSqlInvariantFormat(string format, params object[] args)
    {
      this.stringBuilder.AppendFormat((IFormatProvider) CultureInfo.InvariantCulture, format, args);
    }

    internal static string CreateTableExistsScript(StoreItemCollection itemCollection, out int count)
    {
      int num = 0;
      EFOracleDdlBuilder oracleDdlBuilder = new EFOracleDdlBuilder();
      oracleDdlBuilder.AppendSql("select count(*) from ");
      oracleDdlBuilder.AppendSql("all_tables");
      oracleDdlBuilder.AppendSql(" where ");
      foreach (EntityContainer entityContainer in itemCollection.GetItems<EntityContainer>())
      {
        entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name));
        foreach (EntitySet entitySet in (IEnumerable<EntitySet>) entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy<EntitySet, string>((Func<EntitySet, string>) (s => s.Name)))
        {
          string schemaName = EFOracleDdlBuilder.GetSchemaName(entitySet);
          ++num;
          if (num > 1)
            oracleDdlBuilder.AppendSql(" or ");
          oracleDdlBuilder.AppendSql("(");
          oracleDdlBuilder.AppendSql("owner=");
          if (entitySet.Name != "HistoryRow" || entitySet.Name == "HistoryRow" && schemaName != "dbo")
          {
            oracleDdlBuilder.AppendSql("'");
            oracleDdlBuilder.AppendSql(schemaName);
            oracleDdlBuilder.AppendSql("'");
          }
          else
            oracleDdlBuilder.AppendSql("user");
          oracleDdlBuilder.AppendSql(" and ");
          oracleDdlBuilder.AppendSql("table_name=");
          oracleDdlBuilder.AppendSql("'");
          oracleDdlBuilder.AppendSql(EFOracleDdlBuilder.GetTableName(entitySet));
          oracleDdlBuilder.AppendSql("'");
          oracleDdlBuilder.AppendSql(")");
        }
      }
      count = num;
      return oracleDdlBuilder.GetCommandText();
    }

    internal static string CreateTrigger(string TableName, EdmProperty column, string Operation, string SequencName)
    {
      EFOracleDdlBuilder oracleDdlBuilder = new EFOracleDdlBuilder();
      string name = column.TypeUsage.EdmType.Name;
      string text = (TableName.Length <= 14 ? TableName : TableName.Substring(0, 14)) + "_" + (column.Name.Length <= 13 ? column.Name : column.Name.Substring(0, 13));
      oracleDdlBuilder.AppendSql("create or replace trigger ");
      oracleDdlBuilder.AppendSql("\"");
      oracleDdlBuilder.AppendSql(text);
      oracleDdlBuilder.AppendSql("_");
      oracleDdlBuilder.AppendSql(Operation.Substring(0, 1));
      oracleDdlBuilder.AppendSql("\" \n");
      oracleDdlBuilder.AppendSql("before ");
      oracleDdlBuilder.AppendSql(Operation);
      oracleDdlBuilder.AppendSql(" on ");
      oracleDdlBuilder.AppendSql("\"");
      oracleDdlBuilder.AppendSql(TableName);
      oracleDdlBuilder.AppendSql("\"");
      oracleDdlBuilder.AppendSql(" for each row \n");
      oracleDdlBuilder.AppendSql("begin \n");
      if (Operation == "insert")
      {
        oracleDdlBuilder.AppendSql("  if :new.");
        oracleDdlBuilder.AppendSql("\"");
        oracleDdlBuilder.AppendSql(column.Name);
        oracleDdlBuilder.AppendSql("\"");
        oracleDdlBuilder.AppendSql(" is NULL then \n");
      }
      oracleDdlBuilder.AppendSql("    select ");
      if (name.ToLowerInvariant() == "guid")
        oracleDdlBuilder.AppendSql("SYS_GUID ");
      else if (name.ToLowerInvariant() == "date")
      {
        oracleDdlBuilder.AppendSql("SYSDATE ");
      }
      else
      {
        oracleDdlBuilder.AppendSql(SequencName);
        oracleDdlBuilder.AppendSql(".nextval ");
      }
      oracleDdlBuilder.AppendSql("into ");
      oracleDdlBuilder.AppendSql(":new.");
      oracleDdlBuilder.AppendSql("\"");
      oracleDdlBuilder.AppendSql(column.Name);
      oracleDdlBuilder.AppendSql("\"");
      oracleDdlBuilder.AppendSql(" from dual; \n ");
      if (Operation == "insert")
        oracleDdlBuilder.AppendSql("  end if; \n");
      oracleDdlBuilder.AppendSql("end;\n\n");
      return oracleDdlBuilder.GetCommandText();
    }
  }
}
