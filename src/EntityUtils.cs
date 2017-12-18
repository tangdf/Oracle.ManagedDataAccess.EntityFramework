// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EntityUtils
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal static class EntityUtils
  {
    internal static T CheckArgumentNull<T>(T value, string parameterName) where T : class
    {
      if ((object) value == null)
        throw new ArgumentNullException(parameterName);
      return value;
    }

    internal static string CheckArgumentEmpty(string value, string parameterName)
    {
      if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException(string.Format("ArgumentIsNullOrWhitespace: {0}", (object) parameterName));
      return value;
    }

    public static void Each<T>(this IEnumerable<T> ts, Action<T> action)
    {
      foreach (T t in ts)
        action(t);
    }
  }
}
