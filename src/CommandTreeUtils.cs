// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.CommandTreeUtils
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal class CommandTreeUtils
  {
    private static readonly HashSet<DbExpressionKind> _associativeExpressionKinds = new HashSet<DbExpressionKind>((IEnumerable<DbExpressionKind>) new DbExpressionKind[4]
    {
      DbExpressionKind.Or,
      DbExpressionKind.And,
      DbExpressionKind.Plus,
      DbExpressionKind.Multiply
    });

    internal static IEnumerable<DbExpression> FlattenAssociativeExpression(DbExpression expression)
    {
      return CommandTreeUtils.FlattenAssociativeExpression(expression.ExpressionKind, expression);
    }

    internal static IEnumerable<DbExpression> FlattenAssociativeExpression(DbExpressionKind expressionKind, params DbExpression[] arguments)
    {
      if (!CommandTreeUtils._associativeExpressionKinds.Contains(expressionKind))
        return (IEnumerable<DbExpression>) arguments;
      List<DbExpression> argumentList = new List<DbExpression>();
      foreach (DbExpression expression in arguments)
        CommandTreeUtils.ExtractAssociativeArguments(expressionKind, argumentList, expression);
      return (IEnumerable<DbExpression>) argumentList;
    }

    private static void ExtractAssociativeArguments(DbExpressionKind expressionKind, List<DbExpression> argumentList, DbExpression expression)
    {
      if (expression.ExpressionKind != expressionKind)
      {
        argumentList.Add(expression);
      }
      else
      {
        DbBinaryExpression binaryExpression = expression as DbBinaryExpression;
        if (binaryExpression != null)
        {
          CommandTreeUtils.ExtractAssociativeArguments(expressionKind, argumentList, binaryExpression.Left);
          CommandTreeUtils.ExtractAssociativeArguments(expressionKind, argumentList, binaryExpression.Right);
        }
        else
        {
          DbArithmeticExpression arithmeticExpression = (DbArithmeticExpression) expression;
          CommandTreeUtils.ExtractAssociativeArguments(expressionKind, argumentList, arithmeticExpression.Arguments[0]);
          CommandTreeUtils.ExtractAssociativeArguments(expressionKind, argumentList, arithmeticExpression.Arguments[1]);
        }
      }
    }
  }
}
