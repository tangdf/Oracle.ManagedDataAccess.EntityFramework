// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SqlSelectStatement
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal sealed class SqlSelectStatement : ISqlFragment
  {
    private SqlBuilder select = new SqlBuilder();
    private SqlBuilder from = new SqlBuilder();
    private bool outputColumnsRenamed;
    private Dictionary<string, Symbol> outputColumns;
    private bool isDistinct;
    private List<Symbol> allJoinExtents;
    private List<Symbol> fromExtents;
    private Dictionary<Symbol, bool> outerExtents;
    private TopClause top;
    private SqlBuilder where;
    private SqlBuilder groupBy;
    private SqlBuilder orderBy;
    private static TopClause top_s;
    private bool isTopMost;

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

    internal Dictionary<string, Symbol> OutputColumns
    {
      get
      {
        return this.outputColumns;
      }
      set
      {
        this.outputColumns = value;
      }
    }

    internal bool IsDistinct
    {
      get
      {
        return this.isDistinct;
      }
      set
      {
        this.isDistinct = value;
      }
    }

    internal List<Symbol> AllJoinExtents
    {
      get
      {
        return this.allJoinExtents;
      }
      set
      {
        this.allJoinExtents = value;
      }
    }

    internal List<Symbol> FromExtents
    {
      get
      {
        if (this.fromExtents == null)
          this.fromExtents = new List<Symbol>();
        return this.fromExtents;
      }
    }

    internal Dictionary<Symbol, bool> OuterExtents
    {
      get
      {
        if (this.outerExtents == null)
          this.outerExtents = new Dictionary<Symbol, bool>();
        return this.outerExtents;
      }
    }

    internal TopClause Top
    {
      get
      {
        return this.top;
      }
      set
      {
        this.top = value;
      }
    }

    internal SqlBuilder Select
    {
      get
      {
        return this.select;
      }
    }

    internal SqlBuilder From
    {
      get
      {
        return this.from;
      }
    }

    internal SqlBuilder Where
    {
      get
      {
        if (this.where == null)
          this.where = new SqlBuilder();
        return this.where;
      }
    }

    internal SqlBuilder GroupBy
    {
      get
      {
        if (this.groupBy == null)
          this.groupBy = new SqlBuilder();
        return this.groupBy;
      }
    }

    public SqlBuilder OrderBy
    {
      get
      {
        if (this.orderBy == null)
          this.orderBy = new SqlBuilder();
        return this.orderBy;
      }
    }

    internal static TopClause Top_s
    {
      get
      {
        return SqlSelectStatement.top_s;
      }
      set
      {
        SqlSelectStatement.top_s = value;
      }
    }

    internal bool IsTopMost
    {
      get
      {
        return this.isTopMost;
      }
      set
      {
        this.isTopMost = value;
      }
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      List<string> stringList = (List<string>) null;
      if (this.outerExtents != null && 0 < this.outerExtents.Count)
      {
        foreach (Symbol key in this.outerExtents.Keys)
        {
          JoinSymbol joinSymbol = key as JoinSymbol;
          if (joinSymbol != null)
          {
            foreach (Symbol flattenedExtent in joinSymbol.FlattenedExtentList)
            {
              if (stringList == null)
                stringList = new List<string>();
              stringList.Add(flattenedExtent.NewName);
            }
          }
          else
          {
            if (stringList == null)
              stringList = new List<string>();
            stringList.Add(key.NewName);
          }
        }
      }
      List<Symbol> symbolList = this.AllJoinExtents ?? this.fromExtents;
      if (symbolList != null)
      {
        foreach (Symbol symbol in symbolList)
        {
          if (stringList != null && stringList.Contains(symbol.Name))
          {
            int allExtentName = sqlGenerator.AllExtentNames[symbol.Name];
            string key;
            do
            {
              ++allExtentName;
              key = symbol.Name + allExtentName.ToString((IFormatProvider) CultureInfo.InvariantCulture);
            }
            while (sqlGenerator.AllExtentNames.ContainsKey(key));
            sqlGenerator.AllExtentNames[symbol.Name] = allExtentName;
            symbol.NewName = key;
            sqlGenerator.AllExtentNames[key] = 0;
          }
          if (stringList == null)
            stringList = new List<string>();
          stringList.Add(symbol.NewName);
        }
      }
      ++writer.Indent;
      if (this.IsTopMost && this.Top != null && (this.orderBy != null && !this.OrderBy.IsEmpty) || !this.IsTopMost && this.orderBy != null && !this.OrderBy.IsEmpty)
      {
        writer.Write("SELECT * ");
        writer.WriteLine();
        writer.Write("FROM ( ");
        writer.WriteLine();
      }
      writer.Write("SELECT ");
      if (this.IsDistinct)
        writer.Write("DISTINCT ");
      if (this.select == null || this.Select.IsEmpty)
        writer.Write("*");
      else
        this.Select.WriteSql(writer, sqlGenerator);
      writer.WriteLine();
      writer.Write("FROM ");
      this.From.WriteSql(writer, sqlGenerator);
      if (this.where != null && !this.Where.IsEmpty)
      {
        writer.WriteLine();
        writer.Write("WHERE (");
        this.Where.WriteSql(writer, sqlGenerator);
        writer.Write(")");
        if (this.Top != null && (this.orderBy == null || this.orderBy != null && this.OrderBy.IsEmpty))
        {
          writer.Write(" AND (");
          this.Top.WriteSql(writer, sqlGenerator);
          writer.Write(")");
        }
      }
      else if (this.Top != null && (this.orderBy == null || this.orderBy != null && this.OrderBy.IsEmpty))
      {
        writer.WriteLine();
        writer.Write("WHERE (");
        this.Top.WriteSql(writer, sqlGenerator);
        writer.Write(")");
      }
      if (this.groupBy != null && !this.GroupBy.IsEmpty)
      {
        writer.WriteLine();
        writer.Write("GROUP BY ");
        this.GroupBy.WriteSql(writer, sqlGenerator);
      }
      if (this.orderBy != null && !this.OrderBy.IsEmpty && (this.IsTopMost || this.Top != null))
      {
        writer.WriteLine();
        writer.Write("ORDER BY ");
        this.OrderBy.WriteSql(writer, sqlGenerator);
      }
      if (this.Top != null && this.orderBy != null && !this.OrderBy.IsEmpty)
        SqlSelectStatement.Top_s = this.Top;
      if (this.IsTopMost || this.orderBy != null && !this.OrderBy.IsEmpty)
      {
        if (this.IsTopMost && this.Top != null && (this.orderBy != null && !this.OrderBy.IsEmpty) || !this.IsTopMost && this.orderBy != null && !this.OrderBy.IsEmpty)
        {
          writer.WriteLine();
          writer.Write(")");
        }
        if (SqlSelectStatement.Top_s != null)
        {
          writer.WriteLine();
          writer.Write("WHERE (");
          SqlSelectStatement.Top_s.WriteSql(writer, sqlGenerator);
          writer.Write(")");
          SqlSelectStatement.Top_s = (TopClause) null;
        }
      }
      --writer.Indent;
    }
  }
}
