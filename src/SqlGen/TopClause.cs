// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.TopClause
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Globalization;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal class TopClause : ISqlFragment
  {
    private ISqlFragment topCount;
    private bool withTies;

    internal bool WithTies
    {
      get
      {
        return this.withTies;
      }
    }

    internal ISqlFragment TopCount
    {
      get
      {
        return this.topCount;
      }
    }

    internal TopClause(ISqlFragment topCount, bool withTies)
    {
      this.topCount = topCount;
      this.withTies = withTies;
    }

    internal TopClause(int topCount, bool withTies)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) topCount.ToString((IFormatProvider) CultureInfo.InvariantCulture));
      this.topCount = (ISqlFragment) sqlBuilder;
      this.withTies = withTies;
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      writer.Write("ROWNUM <= (");
      this.TopCount.WriteSql(writer, sqlGenerator);
      writer.Write(")");
      writer.Write(" ");
      if (!this.WithTies)
        return;
      writer.Write("WITH TIES ");
    }
  }
}
