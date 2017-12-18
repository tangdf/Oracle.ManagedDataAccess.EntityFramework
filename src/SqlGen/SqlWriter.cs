// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SqlWriter
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal class SqlWriter : StringWriter
  {
    private int indent = -1;
    private bool atBeginningOfLine = true;

    internal int Indent
    {
      get
      {
        return this.indent;
      }
      set
      {
        this.indent = value;
      }
    }

    public SqlWriter(StringBuilder b)
      : base(b, (IFormatProvider) CultureInfo.InvariantCulture)
    {
    }

    public override void Write(string value)
    {
      if (value == "\r\n")
      {
        base.WriteLine();
        this.atBeginningOfLine = true;
      }
      else
      {
        if (this.atBeginningOfLine)
        {
          if (this.indent > 0)
            base.Write(new string('\t', this.indent));
          this.atBeginningOfLine = false;
        }
        base.Write(value);
      }
    }

    public override void WriteLine()
    {
      base.WriteLine();
      this.atBeginningOfLine = true;
    }
  }
}
