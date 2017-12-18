// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SqlBuilder
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal sealed class SqlBuilder : ISqlFragment
  {
    private List<object> _sqlFragments;

    internal List<object> sqlFragments
    {
      get
      {
        if (this._sqlFragments == null)
          this._sqlFragments = new List<object>();
        return this._sqlFragments;
      }
    }

    public void Append(object s)
    {
      this.sqlFragments.Add(s);
    }

    public void AppendLine()
    {
      this.sqlFragments.Add((object) "\r\n");
    }

    public bool IsEmpty
    {
      get
      {
        if (this._sqlFragments != null)
          return 0 == this._sqlFragments.Count;
        return true;
      }
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
      if (this._sqlFragments == null)
        return;
      foreach (object sqlFragment1 in this._sqlFragments)
      {
        string str = sqlFragment1 as string;
        if (str != null)
        {
          writer.Write(str);
        }
        else
        {
          ISqlFragment sqlFragment2 = sqlFragment1 as ISqlFragment;
          if (sqlFragment2 == null)
            throw new InvalidOperationException();
          sqlFragment2.WriteSql(writer, sqlGenerator);
        }
      }
    }
  }
}
