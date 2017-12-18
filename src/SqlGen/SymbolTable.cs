// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SymbolTable
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal sealed class SymbolTable
  {
    private List<Dictionary<string, Symbol>> symbols = new List<Dictionary<string, Symbol>>();

    internal void EnterScope()
    {
      this.symbols.Add(new Dictionary<string, Symbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase));
    }

    internal void ExitScope()
    {
      this.symbols.RemoveAt(this.symbols.Count - 1);
    }

    internal void Add(string name, Symbol value)
    {
      this.symbols[this.symbols.Count - 1][name] = value;
    }

    internal Symbol Lookup(string name)
    {
      for (int index = this.symbols.Count - 1; index >= 0; --index)
      {
        if (this.symbols[index].ContainsKey(name))
          return this.symbols[index][name];
      }
      return (Symbol) null;
    }
  }
}
