// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.OracleMigrationSqlGenerator
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Microsoft.CSharp.RuntimeBinder;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework.SqlGen;
using OracleInternal.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations.Model;
using System.Data.Entity.Migrations.Sql;
using System.Data.Entity.Migrations.Utilities;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal class OracleMigrationSqlGenerator : MigrationSqlGenerator
  {
    internal static string BatchTerminator = "/";
    internal static string PrimaryKeyPrefix = "PK";
    internal static string ForeignKeyPrefix = "FK";
    internal static string SequencePrefix = "SQ";
    internal static string TriggerPrefix = "TR";
    internal static string IndexPrefix = "IX";
    internal static string NameSeparator = "_";
    internal static int ModelChunkSize = 1024;
    internal static int MaxIdentifierLengthBytes = 30;
    private List<MigrationStatement> _migrationStatements;
    private string _providerManifestToken;
    private SqlGenerator _sqlGenerator;
    private EFOracleProviderManifest _providerManifest;
    private bool _generateSequenceAndTrigger;
    private string _identityColumnName;

    public OracleMigrationSqlGenerator()
    {
      if (!ODTSettings.m_bUseLongIdentifiers)
        return;
      OracleMigrationSqlGenerator.MaxIdentifierLengthBytes = 128;
    }

    public override IEnumerable<MigrationStatement> Generate(IEnumerable<MigrationOperation> migrationOperations, string providerManifestToken)
    {
      EntityUtils.CheckArgumentNull<IEnumerable<MigrationOperation>>(migrationOperations, nameof (migrationOperations));
      EntityUtils.CheckArgumentNull<string>(providerManifestToken, nameof (providerManifestToken));
      this._migrationStatements = new List<MigrationStatement>();
      this.InitializeProviderServices(providerManifestToken);
      this.GenerateStatements(migrationOperations);
      return (IEnumerable<MigrationStatement>) this._migrationStatements;
    }

    private void InitializeProviderServices(string providerManifestToken)
    {
      EntityUtils.CheckArgumentNull<string>(providerManifestToken, nameof (providerManifestToken));
      this._providerManifestToken = providerManifestToken;
      using (DbConnection connection = this.CreateConnection())
      {
        this._providerManifest = (EFOracleProviderManifest) DbProviderServices.GetProviderServices(connection).GetProviderManifest(providerManifestToken);
        this._sqlGenerator = new SqlGenerator(this._providerManifest, EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token));
      }
    }

    private DbConnection CreateConnection()
    {
      return OracleClientFactory.Instance.CreateConnection();
    }

      private static CallSite<Action<CallSite, OracleMigrationSqlGenerator, object>> callSite;
    private void GenerateStatements(IEnumerable<MigrationOperation> migrationOperations)
    {
            EntityUtils.CheckArgumentNull<IEnumerable<MigrationOperation>>(migrationOperations, nameof(migrationOperations));
            ((IEnumerable<object>)migrationOperations).Each<object>((Action<object>)(op =>
          {
      // ISSUE: reference to a compiler-generated field
      if (OracleMigrationSqlGenerator.callSite == null)
        {
                  // ISSUE: reference to a compiler-generated field
            callSite = CallSite<Action<CallSite, OracleMigrationSqlGenerator, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.InvokeSimpleName | CSharpBinderFlags.ResultDiscarded, "Generate", (IEnumerable<Type>)null, typeof(OracleMigrationSqlGenerator), (IEnumerable<CSharpArgumentInfo>)new CSharpArgumentInfo[2]
                {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
                }));
            }
              // ISSUE: reference to a compiler-generated field
              // ISSUE: reference to a compiler-generated field
              callSite.Target((CallSite)callSite, this, op);
        }));
    }

    protected virtual void Generate(CreateTableOperation createTableOperation)
    {
      EntityUtils.CheckArgumentNull<CreateTableOperation>(createTableOperation, nameof (createTableOperation));
      string name1 = (string) null;
      string[] strArray = createTableOperation.Name.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      this._generateSequenceAndTrigger = false;
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("create table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.WriteLine(SqlGenerator.QuoteIdentifier(name2));
        writer.WriteLine("(");
        ++writer.Indent;
        int count1 = createTableOperation.Columns.Count;
        for (int index = 0; index < count1; ++index)
        {
          ColumnModel column = createTableOperation.Columns[index];
          writer.Write(SqlGenerator.QuoteIdentifier(column.Name));
          writer.Write(" ");
          this.WriteColumnType(column, writer);
          if (column.DefaultValue != null || !string.IsNullOrEmpty(column.DefaultValueSql))
            this.WriteColumnDefault(column, writer);
          if (column.IsIdentity)
          {
            if (EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token) >= EFOracleVersion.Oracle12cR1)
            {
              writer.Write(" generated always as identity");
            }
            else
            {
              this._generateSequenceAndTrigger = true;
              this._identityColumnName = column.Name;
            }
          }
          if (column.IsNullable.HasValue)
          {
            bool? isNullable = column.IsNullable;
            if ((isNullable.GetValueOrDefault() ? 0 : (isNullable.HasValue ? 1 : 0)) != 0)
              writer.Write(" not");
          }
          writer.Write(" null");
          if (index < count1 - 1)
            writer.WriteLine(", ");
        }
        if (createTableOperation.PrimaryKey != null)
        {
          writer.WriteLine(",");
          writer.Write("constraint ");
          string str = createTableOperation.PrimaryKey.Name;
          if (name1 != null)
            str = str.Replace(name1 + ".", string.Empty);
          if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
            writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30)));
          else
            writer.Write(SqlGenerator.QuoteIdentifier(str));
          writer.Write(" primary key (");
          int count2 = createTableOperation.PrimaryKey.Columns.Count;
          for (int index = 0; index < count2; ++index)
          {
            writer.Write(SqlGenerator.QuoteIdentifier(createTableOperation.PrimaryKey.Columns[index]));
            if (index < count2 - 1)
              writer.Write(", ");
          }
          writer.WriteLine(")");
        }
        else
          writer.WriteLine();
        --writer.Indent;
        writer.Write(")");
        this.AddStatement(writer);
      }
      if (!this._generateSequenceAndTrigger)
        return;
      string str1 = OracleMigrationSqlGenerator.SequencePrefix + OracleMigrationSqlGenerator.NameSeparator + name2;
      if (Encoding.UTF8.GetByteCount(str1) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
        str1 = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str1, 30);
      string str2 = OracleMigrationSqlGenerator.TriggerPrefix + OracleMigrationSqlGenerator.NameSeparator + name2;
      if (Encoding.UTF8.GetByteCount(str2) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
        str2 = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str2, 30);
      string str3 = name1;
      string tblName = name2;
      this.GenerateSequenceCreate(str3, str1);
      this.GenerateIdentityTriggerCreate(str3, str2, tblName, str1);
    }

    protected virtual void Generate(DropTableOperation dropTableOperation)
    {
      EntityUtils.CheckArgumentNull<DropTableOperation>(dropTableOperation, nameof (dropTableOperation));
      string str1 = (string) null;
      string[] strArray = dropTableOperation.Name.Split('.');
      string name;
      if (strArray.Length == 2)
      {
        str1 = strArray[0];
        name = strArray[1];
      }
      else
        name = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("drop table ");
        if (str1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(str1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name));
        this.AddStatement(writer);
      }
      if (EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token) >= EFOracleVersion.Oracle12cR1)
        return;
      string str2 = OracleMigrationSqlGenerator.SequencePrefix + OracleMigrationSqlGenerator.NameSeparator + name;
      if (Encoding.UTF8.GetByteCount(str2) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
        str2 = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str2, 30);
      this.GenerateSequenceDrop(str1, str2);
    }

    protected virtual void Generate(RenameTableOperation renameTableOperation)
    {
      EntityUtils.CheckArgumentNull<RenameTableOperation>(renameTableOperation, nameof (renameTableOperation));
      string name1 = (string) null;
      string[] strArray = renameTableOperation.Name.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" rename to ");
        writer.Write(SqlGenerator.QuoteIdentifier(renameTableOperation.NewName));
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(AddColumnOperation addColumnOperation)
    {
      EntityUtils.CheckArgumentNull<AddColumnOperation>(addColumnOperation, nameof (addColumnOperation));
      string name1 = (string) null;
      string[] strArray = addColumnOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      this._generateSequenceAndTrigger = false;
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" add (");
        writer.Write(SqlGenerator.QuoteIdentifier(addColumnOperation.Column.Name));
        writer.Write(" ");
        this.WriteColumnType(addColumnOperation.Column, writer);
        if (addColumnOperation.Column.IsIdentity)
        {
          if (EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token) >= EFOracleVersion.Oracle12cR1)
          {
            writer.Write(" generated always as identity");
          }
          else
          {
            this._generateSequenceAndTrigger = true;
            this._identityColumnName = addColumnOperation.Column.Name;
          }
        }
        if (addColumnOperation.Column.IsNullable.HasValue)
        {
          bool? isNullable = addColumnOperation.Column.IsNullable;
          if ((isNullable.GetValueOrDefault() ? 0 : (isNullable.HasValue ? 1 : 0)) != 0)
            writer.Write(" not");
        }
        writer.Write(" null");
        writer.Write(")");
        this.AddStatement(writer);
      }
      if (!this._generateSequenceAndTrigger)
        return;
      string str1 = OracleMigrationSqlGenerator.SequencePrefix + OracleMigrationSqlGenerator.NameSeparator + name2;
      if (Encoding.UTF8.GetByteCount(str1) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
        str1 = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str1, 30);
      string str2 = OracleMigrationSqlGenerator.TriggerPrefix + OracleMigrationSqlGenerator.NameSeparator + name2;
      if (Encoding.UTF8.GetByteCount(str2) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
        str2 = OracleMigrationSqlGenerator.DeriveObjectName((string) null, str2, 30);
      string str3 = name1;
      string tblName = name2;
      this.GenerateSequenceCreate(str3, str1);
      this.GenerateIdentityTriggerCreate(str3, str2, tblName, str1);
    }

    protected virtual void Generate(DropColumnOperation dropColumnOperation)
    {
      EntityUtils.CheckArgumentNull<DropColumnOperation>(dropColumnOperation, "addColumnOperation");
      string name1 = (string) null;
      string[] strArray = dropColumnOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" drop column ");
        writer.Write(SqlGenerator.QuoteIdentifier(dropColumnOperation.Name));
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(AlterColumnOperation alterColumnOperation)
    {
      EntityUtils.CheckArgumentNull<AlterColumnOperation>(alterColumnOperation, nameof (alterColumnOperation));
      string name1 = (string) null;
      string[] strArray = alterColumnOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" modify (");
        writer.Write(SqlGenerator.QuoteIdentifier(alterColumnOperation.Column.Name));
        writer.Write(" ");
        this.WriteColumnType(alterColumnOperation.Column, writer);
        writer.Write(")");
        this.AddStatement(writer);
      }
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("begin");
        writer.WriteLine("  execute immediate");
        writer.Write("  'alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" modify (");
        writer.Write(SqlGenerator.QuoteIdentifier(alterColumnOperation.Column.Name));
        if (alterColumnOperation.Column.IsNullable.HasValue)
        {
          bool? isNullable = alterColumnOperation.Column.IsNullable;
          if ((isNullable.GetValueOrDefault() ? 0 : (isNullable.HasValue ? 1 : 0)) != 0)
            writer.Write(" not");
        }
        writer.Write(" null");
        writer.WriteLine(")';");
        writer.WriteLine("exception");
        writer.WriteLine("  when others then");
        writer.WriteLine("    if sqlcode <> -1442 and sqlcode <> -1451 then");
        writer.WriteLine("      raise;");
        writer.WriteLine("    end if;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(RenameColumnOperation renameColumnOperation)
    {
      EntityUtils.CheckArgumentNull<RenameColumnOperation>(renameColumnOperation, nameof (renameColumnOperation));
      string name1 = (string) null;
      string[] strArray = renameColumnOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" rename column ");
        writer.Write(SqlGenerator.QuoteIdentifier(renameColumnOperation.Name));
        writer.Write(" to ");
        writer.Write(SqlGenerator.QuoteIdentifier(renameColumnOperation.NewName));
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(CreateIndexOperation createIndexOperation)
    {
      EntityUtils.CheckArgumentNull<CreateIndexOperation>(createIndexOperation, nameof (createIndexOperation));
      string name1 = (string) null;
      string[] strArray = createIndexOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("begin");
        writer.WriteLine("  execute immediate");
        writer.Write("  'create ");
        if (createIndexOperation.IsUnique)
          writer.Write("unique ");
        writer.Write("index ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(OracleMigrationSqlGenerator.IndexPrefix);
        stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        stringBuilder.Append(name2);
        stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        int count = createIndexOperation.Columns.Count;
        for (int index = 0; index < count; ++index)
        {
          stringBuilder.Append(createIndexOperation.Columns[index]);
          if (index < count - 1)
            stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        }
        string str = stringBuilder.ToString();
        if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str));
        writer.Write(" on ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" (");
        for (int index = 0; index < count; ++index)
        {
          writer.Write(SqlGenerator.QuoteIdentifier(createIndexOperation.Columns[index]));
          if (index < count - 1)
            writer.Write(", ");
        }
        writer.WriteLine(")';");
        writer.WriteLine("exception");
        writer.WriteLine("  when others then");
        writer.WriteLine("    if sqlcode <> -1408 then");
        writer.WriteLine("      raise;");
        writer.WriteLine("    end if;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(DropIndexOperation dropIndexOperation)
    {
      EntityUtils.CheckArgumentNull<DropIndexOperation>(dropIndexOperation, nameof (dropIndexOperation));
      string name = (string) null;
      string[] strArray = dropIndexOperation.Table.Split('.');
      string str1;
      if (strArray.Length == 2)
      {
        name = strArray[0];
        str1 = strArray[1];
      }
      else
        str1 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("begin");
        writer.WriteLine("  execute immediate");
        writer.Write("  'drop index ");
        if (name != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name) + ".");
        StringBuilder stringBuilder = new StringBuilder();
        stringBuilder.Append(OracleMigrationSqlGenerator.IndexPrefix);
        stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        stringBuilder.Append(str1);
        stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        int count = dropIndexOperation.Columns.Count;
        for (int index = 0; index < count; ++index)
        {
          stringBuilder.Append(dropIndexOperation.Columns[index]);
          if (index < count - 1)
            stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
        }
        string str2 = stringBuilder.ToString();
        if (Encoding.UTF8.GetByteCount(str2) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str2, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str2));
        writer.WriteLine("';");
        writer.WriteLine("exception");
        writer.WriteLine("  when others then");
        writer.WriteLine("    if sqlcode <> -1418 then");
        writer.WriteLine("      raise;");
        writer.WriteLine("    end if;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(AddPrimaryKeyOperation addPrimaryKeyOperation)
    {
      EntityUtils.CheckArgumentNull<AddPrimaryKeyOperation>(addPrimaryKeyOperation, nameof (addPrimaryKeyOperation));
      string name1 = (string) null;
      string[] strArray = addPrimaryKeyOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" add constraint ");
        string str = addPrimaryKeyOperation.Name;
        if (name1 != null)
          str = str.Replace(name1 + ".", string.Empty);
        if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str));
        writer.Write(" primary key ( ");
        int count = addPrimaryKeyOperation.Columns.Count;
        for (int index = 0; index < count; ++index)
        {
          writer.Write(SqlGenerator.QuoteIdentifier(addPrimaryKeyOperation.Columns[index]));
          if (index < count - 1)
            writer.Write(", ");
        }
        writer.Write(")");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(DropPrimaryKeyOperation dropPrimaryKeyOperation)
    {
      EntityUtils.CheckArgumentNull<DropPrimaryKeyOperation>(dropPrimaryKeyOperation, nameof (dropPrimaryKeyOperation));
      string name1 = (string) null;
      string[] strArray = dropPrimaryKeyOperation.Table.Split('.');
      string name2;
      if (strArray.Length == 2)
      {
        name1 = strArray[0];
        name2 = strArray[1];
      }
      else
        name2 = strArray[0];
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" drop constraint ");
        string str = dropPrimaryKeyOperation.Name;
        if (name1 != null)
          str = str.Replace(name1 + ".", string.Empty);
        if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str));
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(AddForeignKeyOperation addForeignKeyOperation)
    {
      EntityUtils.CheckArgumentNull<AddForeignKeyOperation>(addForeignKeyOperation, nameof (addForeignKeyOperation));
      string name1 = (string) null;
      string name2 = (string) null;
      string[] strArray1 = addForeignKeyOperation.DependentTable.Split('.');
      string[] strArray2 = addForeignKeyOperation.PrincipalTable.Split('.');
      string name3;
      if (strArray1.Length == 2)
      {
        name1 = strArray1[0];
        name3 = strArray1[1];
      }
      else
        name3 = strArray1[0];
      string name4;
      if (strArray2.Length == 2)
      {
        name2 = strArray2[0];
        name4 = strArray2[1];
      }
      else
        name4 = strArray2[0];
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(OracleMigrationSqlGenerator.ForeignKeyPrefix);
      stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      stringBuilder.Append(name3);
      stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      int count1 = addForeignKeyOperation.DependentColumns.Count;
      for (int index = 0; index < count1; ++index)
      {
        stringBuilder.Append(addForeignKeyOperation.DependentColumns[index]);
        if (index < count1 - 1)
          stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      }
      string str = stringBuilder.ToString();
      if (name1 != null)
        str = str.Replace(name1 + ".", string.Empty);
      if (name2 != null)
        str = str.Replace(name2 + ".", string.Empty);
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name3));
        writer.Write(" add constraint ");
        if (Encoding.UTF8.GetByteCount(str) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str));
        writer.Write(" foreign key (");
        for (int index = 0; index < count1; ++index)
        {
          writer.Write(SqlGenerator.QuoteIdentifier(addForeignKeyOperation.DependentColumns[index]));
          if (index < count1 - 1)
            writer.Write(", ");
        }
        writer.Write(") references ");
        if (name2 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name2) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name4));
        writer.Write(" (");
        int count2 = addForeignKeyOperation.PrincipalColumns.Count;
        for (int index = 0; index < count2; ++index)
        {
          writer.Write(SqlGenerator.QuoteIdentifier(addForeignKeyOperation.PrincipalColumns[index]));
          if (index < count2 - 1)
            writer.Write(", ");
        }
        writer.Write(")");
        if (addForeignKeyOperation.CascadeDelete)
          writer.Write(" on delete cascade");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(DropForeignKeyOperation dropForeignKeyOperation)
    {
      EntityUtils.CheckArgumentNull<DropForeignKeyOperation>(dropForeignKeyOperation, nameof (dropForeignKeyOperation));
      string name1 = (string) null;
      string str1 = (string) null;
      string[] strArray1 = dropForeignKeyOperation.DependentTable.Split('.');
      string[] strArray2 = dropForeignKeyOperation.PrincipalTable.Split('.');
      string name2;
      if (strArray1.Length == 2)
      {
        name1 = strArray1[0];
        name2 = strArray1[1];
      }
      else
        name2 = strArray1[0];
      if (strArray2.Length == 2)
      {
        str1 = strArray2[0];
        string str2 = strArray2[1];
      }
      else
      {
        string str3 = strArray2[0];
      }
      StringBuilder stringBuilder = new StringBuilder();
      stringBuilder.Append(OracleMigrationSqlGenerator.ForeignKeyPrefix);
      stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      stringBuilder.Append(name2);
      stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      int count = dropForeignKeyOperation.DependentColumns.Count;
      for (int index = 0; index < count; ++index)
      {
        stringBuilder.Append(dropForeignKeyOperation.DependentColumns[index]);
        if (index < count - 1)
          stringBuilder.Append(OracleMigrationSqlGenerator.NameSeparator);
      }
      string str4 = stringBuilder.ToString();
      if (name1 != null)
        str4 = str4.Replace(name1 + ".", string.Empty);
      if (str1 != null)
        str4 = str4.Replace(str1 + ".", string.Empty);
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("alter table ");
        if (name1 != null)
          writer.Write(SqlGenerator.QuoteIdentifier(name1) + ".");
        writer.Write(SqlGenerator.QuoteIdentifier(name2));
        writer.Write(" drop constraint ");
        if (Encoding.UTF8.GetByteCount(str4) > OracleMigrationSqlGenerator.MaxIdentifierLengthBytes)
          writer.Write(SqlGenerator.QuoteIdentifier(OracleMigrationSqlGenerator.DeriveObjectName((string) null, str4, 30)));
        else
          writer.Write(SqlGenerator.QuoteIdentifier(str4));
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(HistoryOperation historyOperation)
    {
      EntityUtils.CheckArgumentNull<HistoryOperation>(historyOperation, nameof (historyOperation));
      foreach (DbModificationCommandTree commandTree in (IEnumerable<DbModificationCommandTree>) historyOperation.CommandTrees)
      {
        using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
        {
          EFOracleVersion storageVersion = EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token);
          List<OracleParameter> parameters;
          CommandType commandType;
          HashSet<string> ListOfParamsToMakeUnicodeFalse;
          string sqlText = SqlGenerator.GenerateSql((DbCommandTree) commandTree, this._providerManifest, storageVersion, out parameters, out commandType, out ListOfParamsToMakeUnicodeFalse);
          int firstBlobByteLength = this.GetFirstBlobByteLength(parameters);
          if (commandTree.CommandTreeKind != DbCommandTreeKind.Insert)
          {
            this.ReplaceParameters(ref sqlText, parameters);
            writer.Write(sqlText);
            this.AddStatement(writer);
          }
          else
          {
            byte[] numArray = (byte[]) null;
            foreach (OracleParameter Parameter in parameters)
            {
              if (Parameter.OracleDbType != OracleDbType.Blob)
              {
                this.ReplaceParameter(ref sqlText, Parameter, true, true, "");
              }
              else
              {
                this.ReplaceParameter(ref sqlText, Parameter, false, false, "model_blob");
                numArray = Parameter.Value as byte[];
              }
            }
            if (numArray != null)
            {
              writer.WriteLine("declare");
              writer.WriteLine("model_blob blob;");
              writer.WriteLine("begin");
              writer.WriteLine("dbms_lob.createtemporary(model_blob, true);");
              int startIndex = 0;
              while (startIndex < firstBlobByteLength)
              {
                int length = Math.Min(OracleMigrationSqlGenerator.ModelChunkSize, firstBlobByteLength - startIndex);
                writer.Write("dbms_lob.append(model_blob, to_blob(cast('");
                writer.Write(BitConverter.ToString(numArray, startIndex, length).Replace("-", string.Empty));
                writer.WriteLine("' as long raw)));");
                startIndex += length;
              }
              if (sqlText.EndsWith("\r\n"))
                sqlText = sqlText.Remove(sqlText.Length - 2, 2);
              writer.Write(sqlText);
              writer.WriteLine(";");
              writer.Write("end;");
              this.AddStatement(writer);
            }
          }
        }
      }
    }

    protected virtual void Generate(UpdateDatabaseOperation updateDatabaseOperation)
    {
      EntityUtils.CheckArgumentNull<UpdateDatabaseOperation>(updateDatabaseOperation, nameof (updateDatabaseOperation));
      if (updateDatabaseOperation.Migrations.Count < 1)
        return;
      this._migrationStatements.Clear();
      if (updateDatabaseOperation.HistoryQueryTrees.Count <= 0)
        return;
      EFOracleVersion storageVersion = EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token);
      List<OracleParameter> parameters;
      CommandType commandType;
      HashSet<string> ListOfParamsToMakeUnicodeFalse;
      string sql = SqlGenerator.GenerateSql((DbCommandTree) updateDatabaseOperation.HistoryQueryTrees[updateDatabaseOperation.HistoryQueryTrees.Count - 1], this._providerManifest, storageVersion, out parameters, out commandType, out ListOfParamsToMakeUnicodeFalse);
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("declare");
        writer.WriteLine("l_hist_tab_exists pls_integer;");
        writer.WriteLine("l_current_migration nvarchar2(4000);");
        writer.WriteLine("begin");
        writer.WriteLine(string.Format("select count(*) into l_hist_tab_exists from all_tables where owner = '{0}' and table_name = '{1}';", (object) SqlGenerator.TargetOwner.Single<string>(), (object) SqlGenerator.TargetTable.Single<string>()));
        writer.WriteLine("if l_hist_tab_exists > 0 then");
        if (sql != null)
        {
          writer.WriteLine("execute immediate '");
          writer.WriteLine("select * from (");
          writer.WriteLine(this.Escape(sql));
          writer.WriteLine(")' into l_current_migration;");
        }
        writer.WriteLine("end if;");
        writer.WriteLine();
        writer.WriteLine("if l_current_migration is null then");
        writer.WriteLine("l_current_migration := '0';");
        writer.WriteLine("end if;");
        writer.WriteLine();
        foreach (UpdateDatabaseOperation.Migration migration in (IEnumerable<UpdateDatabaseOperation.Migration>) updateDatabaseOperation.Migrations)
        {
          this.GenerateStatements((IEnumerable<MigrationOperation>) migration.Operations);
          if (this._migrationStatements.Count > 0)
          {
            writer.Write("if l_current_migration < N'");
            writer.Write(this.Escape(migration.MigrationId));
            writer.WriteLine("' then");
            foreach (MigrationStatement migrationStatement in this._migrationStatements)
            {
              if (!migrationStatement.Sql.StartsWith("begin", true, CultureInfo.InvariantCulture) && !migrationStatement.Sql.StartsWith("declare", true, CultureInfo.InvariantCulture))
              {
                writer.WriteLine("execute immediate '");
                writer.Write(this.Escape(migrationStatement.Sql));
                writer.WriteLine("';");
              }
              else
                writer.Write(migrationStatement.Sql);
              writer.WriteLine();
            }
            writer.WriteLine("end if;");
          }
          this._migrationStatements.Clear();
        }
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    protected virtual void Generate(CreateProcedureOperation createProcedureOperation)
    {
      EntityUtils.CheckArgumentNull<CreateProcedureOperation>(createProcedureOperation, nameof (createProcedureOperation));
      throw new NotImplementedException();
    }

    protected virtual void Generate(AlterProcedureOperation alterProcedureOperation)
    {
      EntityUtils.CheckArgumentNull<AlterProcedureOperation>(alterProcedureOperation, nameof (alterProcedureOperation));
      throw new NotImplementedException();
    }

    private void Generate(ProcedureOperation procedureOperation)
    {
      EntityUtils.CheckArgumentNull<ProcedureOperation>(procedureOperation, nameof (procedureOperation));
      throw new NotImplementedException();
    }

    public override string GenerateProcedureBody(ICollection<DbModificationCommandTree> commandTrees, string rowsAffectedParameter, string providerManifestToken)
    {
      EntityUtils.CheckArgumentNull<ICollection<DbModificationCommandTree>>(commandTrees, nameof (commandTrees));
      EntityUtils.CheckArgumentEmpty(providerManifestToken, nameof (providerManifestToken));
      return "null;";
    }

    private string GenerateInsertBodySql(DbInsertCommandTree commandTree)
    {
      EntityUtils.CheckArgumentNull<DbInsertCommandTree>(commandTree, nameof (commandTree));
      EFOracleVersion storageVersion = EFOracleVersionUtils.GetStorageVersion(this._providerManifest.Token);
      List<OracleParameter> parameters;
      return DmlSqlGenerator.GenerateInsertSql(commandTree, this._providerManifest, storageVersion, out parameters);
    }

    private string GenerateUpdateBodySql(DbUpdateCommandTree commandTree)
    {
      string str = (string) null;
      EntityUtils.CheckArgumentNull<DbUpdateCommandTree>(commandTree, nameof (commandTree));
      return str;
    }

    private string GenerateDeleteBodySql(DbDeleteCommandTree commandTree)
    {
      string str = (string) null;
      EntityUtils.CheckArgumentNull<DbDeleteCommandTree>(commandTree, nameof (commandTree));
      return str;
    }

    protected virtual void Generate(SqlOperation sqlOperation)
    {
      EntityUtils.CheckArgumentNull<SqlOperation>(sqlOperation, nameof (sqlOperation));
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write(sqlOperation.Sql);
        this.AddStatement(writer);
      }
    }

    private void GenerateSequenceCreate(string seqOwner, string seqName)
    {
      EntityUtils.CheckArgumentNull<string>(seqOwner, nameof (seqOwner));
      EntityUtils.CheckArgumentNull<string>(seqName, nameof (seqName));
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("create sequence ");
        writer.Write(SqlGenerator.QuoteIdentifier(seqOwner));
        writer.Write(".");
        writer.Write(SqlGenerator.QuoteIdentifier(seqName));
        this.AddStatement(writer);
      }
    }

    private void GenerateSequenceDrop(string seqOwner, string seqName)
    {
      EntityUtils.CheckArgumentNull<string>(seqOwner, nameof (seqOwner));
      EntityUtils.CheckArgumentNull<string>(seqName, nameof (seqName));
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("begin");
        writer.WriteLine("  execute immediate");
        writer.Write("  'drop sequence ");
        writer.Write(SqlGenerator.QuoteIdentifier(seqOwner));
        writer.Write(".");
        writer.Write(SqlGenerator.QuoteIdentifier(seqName));
        writer.WriteLine("';");
        writer.WriteLine("exception");
        writer.WriteLine("  when others then");
        writer.WriteLine("    if sqlcode <> -2289 then");
        writer.WriteLine("      raise;");
        writer.WriteLine("    end if;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    private void GenerateIdentityTriggerCreate(string trgOwner, string trgName, string tblName, string seqName)
    {
      EntityUtils.CheckArgumentNull<string>(trgOwner, nameof (trgOwner));
      EntityUtils.CheckArgumentNull<string>(trgName, nameof (trgName));
      EntityUtils.CheckArgumentNull<string>(tblName, nameof (tblName));
      EntityUtils.CheckArgumentNull<string>(seqName, nameof (seqName));
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.Write("create or replace trigger ");
        writer.Write(SqlGenerator.QuoteIdentifier(trgOwner));
        writer.Write(".");
        writer.WriteLine(SqlGenerator.QuoteIdentifier(trgName));
        writer.Write("before insert on ");
        writer.Write(SqlGenerator.QuoteIdentifier(trgOwner));
        writer.Write(".");
        writer.WriteLine(SqlGenerator.QuoteIdentifier(tblName));
        writer.WriteLine("for each row");
        writer.WriteLine("begin");
        writer.Write("  select ");
        writer.Write(SqlGenerator.QuoteIdentifier(trgOwner));
        writer.Write(".");
        writer.Write(SqlGenerator.QuoteIdentifier(seqName));
        writer.Write(".nextval into :new.");
        writer.Write(SqlGenerator.QuoteIdentifier(this._identityColumnName));
        writer.WriteLine(" from dual;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    private void GenerateIdentityTriggerDrop(string trgOwner, string trgName)
    {
      EntityUtils.CheckArgumentNull<string>(trgOwner, nameof (trgOwner));
      EntityUtils.CheckArgumentNull<string>(trgName, nameof (trgName));
      using (IndentedTextWriter writer = new IndentedTextWriter((TextWriter) new StringWriter((IFormatProvider) CultureInfo.InvariantCulture)))
      {
        writer.WriteLine("begin");
        writer.WriteLine("  execute immediate");
        writer.Write("  'drop trigger ");
        writer.Write(SqlGenerator.QuoteIdentifier(trgOwner));
        writer.Write(".");
        writer.Write(SqlGenerator.QuoteIdentifier(trgName));
        writer.WriteLine("';");
        writer.WriteLine("exception");
        writer.WriteLine("  when others then");
        writer.WriteLine("    if sqlcode <> -4080 then");
        writer.WriteLine("      raise;");
        writer.WriteLine("    end if;");
        writer.Write("end;");
        this.AddStatement(writer);
      }
    }

    private void WriteColumnType(ColumnModel columnModel, IndentedTextWriter writer)
    {
        string str = columnModel.StoreType;
        TypeUsage storeType = this._providerManifest.GetStoreType(columnModel.TypeUsage);
        if (string.IsNullOrWhiteSpace(str))
            str = storeType.EdmType.Name;
        if (storeType.EdmType.Name.ToLowerInvariant() == "guid raw")
            str = "raw";
        writer.Write(str);
        switch (str)
        {
            case "number":
                IndentedTextWriter indentedTextWriter1 = writer;
                string format1 = "({0}, {1})";
                byte? precision1 = columnModel.Precision;
                // ISSUE: variable of a boxed type
                byte local1 = (byte)(precision1.HasValue ? (int)precision1.GetValueOrDefault() : (int)storeType.GetPrecision());
                byte? scale = columnModel.Scale;
                // ISSUE: variable of a boxed type
                byte local2 = (byte)(scale.HasValue ? (int)scale.GetValueOrDefault() : (int)storeType.GetScale());
                indentedTextWriter1.Write(format1, (object)local1, (object)local2);
                break;
            case "float":
                IndentedTextWriter indentedTextWriter2 = writer;
                string format2 = "({0})";
                byte? precision2 = columnModel.Precision;
                // ISSUE: variable of a boxed type
               byte local3 = (byte)(precision2.HasValue ? (int)precision2.GetValueOrDefault() : (int)storeType.GetPrecision());
                indentedTextWriter2.Write(format2, (object)local3);
                break;
            case "char":
            case "varchar2":
                writer.Write("({0} CHAR)", (object)storeType.GetMaxLength());
                break;
            case "nchar":
            case "nvarchar2":
            case "raw":
                writer.Write("({0})", (object)storeType.GetMaxLength());
                break;
        }
    }

    private void WriteColumnDefault(ColumnModel columnModel, IndentedTextWriter writer)
    {
      TypeUsage storeType = this._providerManifest.GetStoreType(columnModel.TypeUsage);
      string str = !(storeType.EdmType.Name != "guid raw") ? "raw" : storeType.EdmType.Name;
      writer.Write(" default ");
      switch (str)
      {
        case "char":
        case "varchar2":
        case "clob":
        case "raw":
          if (columnModel.DefaultValue != null)
          {
            writer.Write("'");
            writer.Write(columnModel.DefaultValue.ToString());
            writer.Write("'");
            break;
          }
          if (string.IsNullOrEmpty(columnModel.DefaultValueSql))
            break;
          writer.Write(columnModel.DefaultValueSql);
          break;
        case "nchar":
        case "nvarchar2":
        case "nclob":
          if (columnModel.DefaultValue != null)
          {
            writer.Write("N'");
            writer.Write(columnModel.DefaultValue.ToString());
            writer.Write("'");
            break;
          }
          if (string.IsNullOrEmpty(columnModel.DefaultValueSql))
            break;
          writer.Write(columnModel.DefaultValueSql);
          break;
        default:
          if (columnModel.DefaultValue != null)
          {
            writer.Write(columnModel.DefaultValue.ToString());
            break;
          }
          if (string.IsNullOrEmpty(columnModel.DefaultValueSql))
            break;
          writer.Write(columnModel.DefaultValueSql);
          break;
      }
    }

    private void AddStatement(IndentedTextWriter writer)
    {
      this._migrationStatements.Add(new MigrationStatement()
      {
        Sql = writer.InnerWriter.ToString(),
        SuppressTransaction = false,
        BatchTerminator = OracleMigrationSqlGenerator.BatchTerminator
      });
    }

    private void ReplaceParameters(ref string sqlText, List<OracleParameter> parameters)
    {
      foreach (OracleParameter parameter in parameters)
        this.ReplaceParameter(ref sqlText, parameter, true, true, "");
    }

    private void ReplaceParameter(ref string sqlText, OracleParameter Parameter, bool QuoteValue = true, bool UseParameterValue = true, string NewParameterValue = "")
    {
      switch (Parameter.DbType)
      {
        case DbType.Object:
          if (QuoteValue)
          {
            if (UseParameterValue)
            {
              sqlText = sqlText.Replace(Parameter.ParameterName, "'" + BitConverter.ToString(Parameter.Value as byte[]).Replace("-", string.Empty) + "'");
              break;
            }
            sqlText = sqlText.Replace(Parameter.ParameterName, "'" + NewParameterValue + "'");
            break;
          }
          if (UseParameterValue)
          {
            sqlText = sqlText.Replace(Parameter.ParameterName, BitConverter.ToString(Parameter.Value as byte[]).Replace("-", string.Empty));
            break;
          }
          sqlText = sqlText.Replace(Parameter.ParameterName, NewParameterValue);
          break;
        case DbType.String:
          if (QuoteValue)
          {
            if (UseParameterValue)
            {
              sqlText = sqlText.Replace(Parameter.ParameterName, "'" + Parameter.Value.ToString() + "'");
              break;
            }
            sqlText = sqlText.Replace(Parameter.ParameterName, "'" + NewParameterValue + "'");
            break;
          }
          if (UseParameterValue)
          {
            sqlText = sqlText.Replace(Parameter.ParameterName, Parameter.Value.ToString());
            break;
          }
          sqlText = sqlText.Replace(Parameter.ParameterName, NewParameterValue);
          break;
        default:
          sqlText = sqlText.Replace(Parameter.ParameterName, Parameter.Value.ToString());
          break;
      }
    }

    internal static string DeriveObjectName(string Prefix, string BaseName, int MaxLengthBytes = 30)
    {
      int num1 = 0;
      char[] chArray1 = (char[]) null;
      if (Prefix != null)
      {
        num1 = Encoding.UTF8.GetByteCount(Prefix);
        chArray1 = Prefix.ToCharArray();
      }
      char[] chArray2 = (char[]) null;
      int num2 = 0;
      char[] chArray3 = (char[]) null;
      if (BaseName != null)
      {
        BaseName = BaseName.Replace("\"", string.Empty).Replace('.', '_');
        Encoding.UTF8.GetByteCount(BaseName);
        chArray2 = BaseName.ToCharArray();
        string s = Math.Abs(BaseName.GetHashCode()).ToString();
        num2 = Encoding.UTF8.GetByteCount(s);
        chArray3 = s.ToCharArray();
      }
      int byteCount1 = Encoding.UTF8.GetByteCount(OracleMigrationSqlGenerator.NameSeparator);
      char[] charArray = OracleMigrationSqlGenerator.NameSeparator.ToCharArray();
      int num3 = MaxLengthBytes - num1 - num2 - (num1 > 0 ? byteCount1 * 2 : byteCount1);
      if (num3 < 0)
        num3 = 0;
      int num4 = 0;
      StringBuilder stringBuilder = new StringBuilder();
      if (num1 > 0)
      {
        foreach (char ch in chArray1)
        {
          int byteCount2 = Encoding.UTF8.GetByteCount(ch.ToString());
          if (num4 + byteCount2 <= MaxLengthBytes)
          {
            stringBuilder.Append(ch);
            num4 += byteCount2;
          }
          else
            break;
        }
        if (num4 < MaxLengthBytes)
        {
          foreach (char ch in charArray)
          {
            int byteCount2 = Encoding.UTF8.GetByteCount(ch.ToString());
            if (num4 + byteCount2 <= MaxLengthBytes)
            {
              stringBuilder.Append(ch);
              num4 += byteCount2;
            }
            else
              break;
          }
        }
      }
      if (num3 > 0 && num4 < MaxLengthBytes)
      {
        int num5 = 0;
        foreach (char ch in chArray2)
        {
          int byteCount2 = Encoding.UTF8.GetByteCount(ch.ToString());
          if (num5 + byteCount2 <= num3)
          {
            stringBuilder.Append(ch);
            num4 += byteCount2;
            num5 += byteCount2;
          }
          else
            break;
        }
        if (num4 < MaxLengthBytes)
        {
          foreach (char ch in charArray)
          {
            int byteCount2 = Encoding.UTF8.GetByteCount(ch.ToString());
            if (num4 + byteCount2 <= MaxLengthBytes)
            {
              stringBuilder.Append(ch);
              num4 += byteCount2;
            }
            else
              break;
          }
        }
      }
      if (num2 > 0 && num4 < MaxLengthBytes)
      {
        foreach (char ch in chArray3)
        {
          int byteCount2 = Encoding.UTF8.GetByteCount(ch.ToString());
          if (num4 + byteCount2 <= MaxLengthBytes)
          {
            stringBuilder.Append(ch);
            num4 += byteCount2;
          }
          else
            break;
        }
      }
      return stringBuilder.ToString();
    }

    private string Escape(string inputText)
    {
      EntityUtils.CheckArgumentNull<string>(inputText, nameof (inputText));
      return inputText.Replace("'", "''");
    }

    private int GetFirstBlobByteLength(List<OracleParameter> Parameters)
    {
      int num = 0;
      foreach (OracleParameter parameter in Parameters)
      {
        if (parameter.OracleDbType == OracleDbType.Blob && parameter.Value is byte[] && parameter.Value != null)
        {
          num = (parameter.Value as byte[]).Length;
          break;
        }
      }
      return num;
    }
  }
}
