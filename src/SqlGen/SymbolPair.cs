// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SymbolPair
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal class SymbolPair : ISqlFragment
  {
    public Symbol Source;
    public Symbol Column;

    public SymbolPair(Symbol source, Symbol column)
    {
      this.Source = source;
      this.Column = column;
    }

    public void WriteSql(SqlWriter writer, SqlGenerator sqlGenerator)
    {
    }
  }
}
