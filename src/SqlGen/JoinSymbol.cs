// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.JoinSymbol
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal sealed class JoinSymbol : Symbol
  {
    private List<Symbol> columnList;
    private List<Symbol> extentList;
    private List<Symbol> flattenedExtentList;
    private Dictionary<string, Symbol> nameToExtent;
    private bool isNestedJoin;

    internal List<Symbol> ColumnList
    {
      get
      {
        if (this.columnList == null)
          this.columnList = new List<Symbol>();
        return this.columnList;
      }
      set
      {
        this.columnList = value;
      }
    }

    internal List<Symbol> ExtentList
    {
      get
      {
        return this.extentList;
      }
    }

    internal List<Symbol> FlattenedExtentList
    {
      get
      {
        if (this.flattenedExtentList == null)
          this.flattenedExtentList = new List<Symbol>();
        return this.flattenedExtentList;
      }
      set
      {
        this.flattenedExtentList = value;
      }
    }

    internal Dictionary<string, Symbol> NameToExtent
    {
      get
      {
        return this.nameToExtent;
      }
    }

    internal bool IsNestedJoin
    {
      get
      {
        return this.isNestedJoin;
      }
      set
      {
        this.isNestedJoin = value;
      }
    }

    public JoinSymbol(string name, TypeUsage type, List<Symbol> extents)
      : base(name, type)
    {
      this.extentList = new List<Symbol>(extents.Count);
      this.nameToExtent = new Dictionary<string, Symbol>(extents.Count, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Symbol extent in extents)
      {
        this.nameToExtent[extent.Name] = extent;
        this.ExtentList.Add(extent);
      }
    }
  }
}
