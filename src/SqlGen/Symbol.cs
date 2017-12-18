// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.Symbol
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal class Symbol : ISqlFragment
  {
    private Dictionary<string, Symbol> columns;
    private bool needsRenaming;
    private bool outputColumnsRenamed;
    private string name;
    private string newName;
    private TypeUsage type;

    internal Dictionary<string, Symbol> Columns
    {
      get
      {
        if (this.columns == null)
          this.columns = new Dictionary<string, Symbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        return this.columns;
      }
    }

    internal bool NeedsRenaming
    {
      get
      {
        return this.needsRenaming;
      }
      set
      {
        this.needsRenaming = value;
      }
    }

    internal bool OutputColumnsRenamed
    {
      get
      {
        return this.outputColumnsRenamed;
      }
      set
      {
        this.outputColumnsRenamed = value;
      }
    }

    public string Name
    {
      get
      {
        return this.name;
      }
    }

    public string NewName
    {
      get
      {
        return this.newName;
      }
      set
      {
        this.newName = value;
      }
    }

    internal TypeUsage Type
    {
      get
      {
        return this.type;
      }
      set
      {
        this.type = value;
      }
    }

    public Symbol(string name, TypeUsage type)
    {
      this.name = name;
      this.newName = name;
      this.Type = type;
    }

    public Symbol(string name, TypeUsage type, Dictionary<string, Symbol> columns)
    {
      this.name = name;
      this.newName = name;
      this.Type = type;
      this.columns = columns;
      this.OutputColumnsRenamed = true;
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      if (this.NeedsRenaming)
      {
        int num;
        if (sqlGenerator.AllColumnNames.TryGetValue(this.NewName, out num))
        {
          string key;
          do
          {
            ++num;
            key = this.NewName + num.ToString((IFormatProvider) CultureInfo.InvariantCulture);
          }
          while (sqlGenerator.AllColumnNames.ContainsKey(key));
          sqlGenerator.AllColumnNames[this.NewName] = num;
          this.NewName = key;
        }
        sqlGenerator.AllColumnNames[this.NewName] = 0;
        this.NeedsRenaming = false;
      }
      writer.Write(SqlGenerator.QuoteIdentifier(this.NewName));
    }
  }
}
