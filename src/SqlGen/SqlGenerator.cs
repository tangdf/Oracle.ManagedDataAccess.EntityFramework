// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.SqlGenerator
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common.Utils;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal class SqlGenerator : DbExpressionVisitor<ISqlFragment>
  {
    private static readonly Dictionary<string, SqlGenerator.FunctionHandler> _canonicalFunctionHandlers = SqlGenerator.InitializeCanonicalFunctionHandlers();
    private static readonly char[] hexDigits = new char[16]
    {
      '0',
      '1',
      '2',
      '3',
      '4',
      '5',
      '6',
      '7',
      '8',
      '9',
      'A',
      'B',
      'C',
      'D',
      'E',
      'F'
    };
    private readonly SymbolTable symbolTable = new SymbolTable();
    private readonly Dictionary<string, bool> ListOfParamsForNonUnicode = new Dictionary<string, bool>();
    private const byte defaultDecimalPrecision = 38;
    private Stack<SqlSelectStatement> selectStatementStack;
    private Stack<bool> isParentAJoinStack;
    private Dictionary<string, int> allExtentNames;
    private Dictionary<string, int> allColumnNames;
    private bool isVarRefSingle;
    private bool _bNeedToMakeUnicodeFalse;
    private bool _bIgnoreMakingUnicodeFalse;
    private EFOracleVersion _sqlVersion;
    [ThreadStatic]
    internal static List<string> TargetOwner;
    [ThreadStatic]
    internal static List<string> TargetTable;
    private EFOracleProviderManifest _providerManifest;

    private SqlSelectStatement CurrentSelectStatement
    {
      get
      {
        return this.selectStatementStack.Peek();
      }
    }

    private bool IsParentAJoin
    {
      get
      {
        if (this.isParentAJoinStack.Count != 0)
          return this.isParentAJoinStack.Peek();
        return false;
      }
    }

    internal Dictionary<string, int> AllExtentNames
    {
      get
      {
        return this.allExtentNames;
      }
    }

    internal Dictionary<string, int> AllColumnNames
    {
      get
      {
        return this.allColumnNames;
      }
    }

    private static Dictionary<string, SqlGenerator.FunctionHandler> InitializeCanonicalFunctionHandlers()
    {
      return new Dictionary<string, SqlGenerator.FunctionHandler>(53, (IEqualityComparer<string>) StringComparer.Ordinal)
      {
        {
          "Left",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionLeft)
        },
        {
          "Right",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionRight)
        },
        {
          "IndexOf",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionIndexOf2)
        },
        {
          "Substring",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionSubstring)
        },
        {
          "Length",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionLength)
        },
        {
          "NewGuid",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionNewGuid)
        },
        {
          "Round",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionRound)
        },
        {
          "ToLower",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionToLower)
        },
        {
          "ToUpper",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionToUpper)
        },
        {
          "Ceiling",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCeiling)
        },
        {
          "Trim",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionTrim)
        },
        {
          "Year",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Month",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Day",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Hour",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Minute",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Second",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "Millisecond",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "CurrentDateTime",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
        },
        {
          "CurrentUtcDateTime",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
        },
        {
          "CurrentDateTimeOffset",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
        },
        {
          "GetTotalOffsetMinutes",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionGetTotalOffsetMinutes)
        },
        {
          "Concat",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleConcatFunction)
        },
        {
          "BitwiseAnd",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
        },
        {
          "BitwiseNot",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
        },
        {
          "BitwiseOr",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
        },
        {
          "BitwiseXor",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionBitwise)
        },
        {
          "Truncate",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionTruncate)
        },
        {
          "TruncateTime",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionTruncate)
        },
        {
          "DayOfYear",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepart)
        },
        {
          "AddNanoseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddMicroseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddMilliseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddSeconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddMinutes",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddHours",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddDays",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddMonths",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "AddYears",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartAdd)
        },
        {
          "CreateDateTime",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
        },
        {
          "CreateDateTimeOffset",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionCurrentDateTime)
        },
        {
          "DiffNanoseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffMilliseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffMicroseconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffSeconds",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffMinutes",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffHours",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffDays",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffMonths",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "DiffYears",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionDatepartDiff)
        },
        {
          "Contains",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionIndexOf2)
        },
        {
          "EndsWith",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionIndexOf2)
        },
        {
          "StartsWith",
          new SqlGenerator.FunctionHandler(SqlGenerator.HandleCanonicalFunctionIndexOf2)
        }
      };
    }

    internal bool IsPre10g
    {
      get
      {
        return this._sqlVersion < EFOracleVersion.Oracle10gR1;
      }
    }

    internal SqlGenerator(EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion)
    {
      this._providerManifest = providerManifest;
      this._sqlVersion = sqlVersion;
    }

    internal static string GenerateSql(DbCommandTree tree, EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion, out List<OracleParameter> parameters, out CommandType commandType, out HashSet<string> ListOfParamsToMakeUnicodeFalse)
    {
      SqlGenerator sqlGenerator = (SqlGenerator) null;
      commandType = CommandType.Text;
      parameters = (List<OracleParameter>) null;
      ListOfParamsToMakeUnicodeFalse = (HashSet<string>) null;
      SqlGenerator.TargetOwner = new List<string>();
      SqlGenerator.TargetTable = new List<string>();
      if (tree is DbQueryCommandTree)
        return new SqlGenerator(providerManifest, sqlVersion).GenerateSql((DbQueryCommandTree) tree, out ListOfParamsToMakeUnicodeFalse);
      if (tree is DbInsertCommandTree)
        return DmlSqlGenerator.GenerateInsertSql((DbInsertCommandTree) tree, providerManifest, sqlVersion, out parameters);
      if (tree is DbDeleteCommandTree)
        return DmlSqlGenerator.GenerateDeleteSql((DbDeleteCommandTree) tree, providerManifest, sqlVersion, out parameters);
      if (tree is DbUpdateCommandTree)
        return DmlSqlGenerator.GenerateUpdateSql((DbUpdateCommandTree) tree, providerManifest, sqlVersion, out parameters);
      if (tree is DbFunctionCommandTree)
      {
        sqlGenerator = new SqlGenerator(providerManifest, sqlVersion);
        return SqlGenerator.GenerateFunctionSql((DbFunctionCommandTree) tree, out commandType, out parameters);
      }
      parameters = (List<OracleParameter>) null;
      return (string) null;
    }

    private static string GenerateFunctionSql(DbFunctionCommandTree tree, out CommandType commandType, out List<OracleParameter> parameters)
    {
      EdmFunction edmFunction = tree.EdmFunction;
      parameters = (List<OracleParameter>) null;
      string metadataProperty1 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) edmFunction, "CommandTextAttribute");
      string metadataProperty2 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) edmFunction, "Schema");
      string metadataProperty3 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) edmFunction, "StoreFunctionNameAttribute");
      string metadataProperty4 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) edmFunction, "EFOracleProviderExtensions:CursorParameterName");
      if (!string.IsNullOrEmpty(metadataProperty4))
      {
        parameters = new List<OracleParameter>();
        OracleParameter oracleParameter = new OracleParameter();
        oracleParameter.OracleDbType = OracleDbType.RefCursor;
        oracleParameter.ParameterName = metadataProperty4;
        oracleParameter.Direction = ParameterDirection.Output;
        parameters.Add(oracleParameter);
      }
      if (string.IsNullOrEmpty(metadataProperty1))
      {
        commandType = CommandType.StoredProcedure;
        string str = SqlGenerator.QuoteIdentifier_storeFunctionName(string.IsNullOrEmpty(metadataProperty3) ? edmFunction.Name : metadataProperty3);
        if (!string.IsNullOrEmpty(metadataProperty2))
          return SqlGenerator.QuoteIdentifier(metadataProperty2) + "." + str;
        return str;
      }
      commandType = CommandType.Text;
      return metadataProperty1;
    }

    private string GenerateSql(DbQueryCommandTree tree, out HashSet<string> ListOfParamsToMakeUnicodeFalse)
    {
      DbQueryCommandTree queryCommandTree = tree;
      this.selectStatementStack = new Stack<SqlSelectStatement>();
      this.isParentAJoinStack = new Stack<bool>();
      this.allExtentNames = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.allColumnNames = new Dictionary<string, int>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      ISqlFragment sqlStatement;
      if (EF6MetadataHelpers.IsCollectionType((GlobalItem) queryCommandTree.Query.ResultType.EdmType))
      {
        SqlSelectStatement sqlSelectStatement = this.VisitExpressionEnsureSqlStatement(queryCommandTree.Query);
        sqlSelectStatement.IsTopMost = true;
        sqlStatement = (ISqlFragment) sqlSelectStatement;
      }
      else
      {
        SqlBuilder sqlBuilder = new SqlBuilder();
        sqlBuilder.Append((object) "SELECT ");
        sqlBuilder.Append((object) queryCommandTree.Query.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        sqlStatement = (ISqlFragment) sqlBuilder;
      }
      if (this.isVarRefSingle)
        throw new NotSupportedException();
      ListOfParamsToMakeUnicodeFalse = new HashSet<string>((IEnumerable<string>) this.ListOfParamsForNonUnicode.Where<KeyValuePair<string, bool>>((Func<KeyValuePair<string, bool>, bool>) (p => p.Value)).Select<KeyValuePair<string, bool>, string>((Func<KeyValuePair<string, bool>, string>) (q => q.Key)).ToList<string>());
      return this.WriteSql(sqlStatement);
    }

    private string WriteSql(ISqlFragment sqlStatement)
    {
      StringBuilder b = new StringBuilder(1024);
      using (SqlWriter writer = new SqlWriter(b))
        sqlStatement.WriteSql(writer, this);
      return b.ToString();
    }

    public override ISqlFragment Visit(DbAndExpression e)
    {
      return (ISqlFragment) this.VisitBinaryExpression(" AND ", DbExpressionKind.And, e.Left, e.Right);
    }

    public override ISqlFragment Visit(DbApplyExpression e)
    {
      List<DbExpressionBinding> expressionBindingList = new List<DbExpressionBinding>();
      expressionBindingList.Add(e.Input);
      expressionBindingList.Add(e.Apply);
      string joinString;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.CrossApply:
          joinString = "CROSS APPLY";
          break;
        case DbExpressionKind.OuterApply:
          joinString = "OUTER APPLY";
          break;
        default:
          throw new InvalidOperationException();
      }
      return this.VisitJoinExpression((IList<DbExpressionBinding>) expressionBindingList, DbExpressionKind.CrossJoin, joinString, (DbExpression) null);
    }

    public override ISqlFragment Visit(DbArithmeticExpression e)
    {
      SqlBuilder sqlBuilder;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.Plus:
          sqlBuilder = this.VisitBinaryExpression(" + ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);
          break;
        case DbExpressionKind.UnaryMinus:
          sqlBuilder = new SqlBuilder();
          sqlBuilder.Append((object) " -(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ")");
          break;
        case DbExpressionKind.Divide:
          sqlBuilder = this.VisitBinaryExpression(" / ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);
          break;
        case DbExpressionKind.Minus:
          sqlBuilder = this.VisitBinaryExpression(" - ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);
          break;
        case DbExpressionKind.Modulo:
          sqlBuilder = new SqlBuilder();
          sqlBuilder.Append((object) "MOD(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ",");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ")");
          break;
        case DbExpressionKind.Multiply:
          sqlBuilder = this.VisitBinaryExpression(" * ", e.ExpressionKind, e.Arguments[0], e.Arguments[1]);
          break;
        default:
          throw new InvalidOperationException(string.Empty);
      }
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbCaseExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "CASE");
      for (int index = 0; index < e.When.Count; ++index)
      {
        sqlBuilder.Append((object) " WHEN (");
        sqlBuilder.Append((object) e.When[index].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        sqlBuilder.Append((object) ") THEN ");
        sqlBuilder.Append((object) e.Then[index].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      }
      if (e.Else != null && !(e.Else is DbNullExpression))
      {
        sqlBuilder.Append((object) " ELSE ");
        sqlBuilder.Append((object) e.Else.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      }
      sqlBuilder.Append((object) " END");
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbCastExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      string sqlPrimitiveType = this.GetSqlPrimitiveType(e.ResultType);
      switch (sqlPrimitiveType.ToLowerInvariant())
      {
        case "nclob":
          sqlBuilder.Append((object) "TO_NCLOB(");
          sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ")");
          break;
        case "clob":
          sqlBuilder.Append((object) "TO_CLOB(");
          sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ")");
          break;
        case "blob":
          sqlBuilder.Append((object) "TO_BLOB(");
          sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) ")");
          break;
        default:
          sqlBuilder.Append((object) " CAST( ");
          sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          sqlBuilder.Append((object) " AS ");
          sqlBuilder.Append((object) sqlPrimitiveType);
          sqlBuilder.Append((object) ")");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbComparisonExpression e)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(e.Left.ResultType, PrimitiveTypeKind.String))
        this._bNeedToMakeUnicodeFalse = this.CheckIfNeedToMakeUnicodeFalse((DbExpression) e);
      SqlBuilder sqlBuilder;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.LessThan:
          sqlBuilder = this.VisitComparisonExpression(" < ", e.Left, e.Right);
          break;
        case DbExpressionKind.LessThanOrEquals:
          sqlBuilder = this.VisitComparisonExpression(" <= ", e.Left, e.Right);
          break;
        case DbExpressionKind.NotEquals:
          sqlBuilder = this.VisitComparisonExpression(" <> ", e.Left, e.Right);
          break;
        case DbExpressionKind.Equals:
          sqlBuilder = this.VisitComparisonExpression(" = ", e.Left, e.Right);
          break;
        case DbExpressionKind.GreaterThan:
          sqlBuilder = this.VisitComparisonExpression(" > ", e.Left, e.Right);
          break;
        case DbExpressionKind.GreaterThanOrEquals:
          sqlBuilder = this.VisitComparisonExpression(" >= ", e.Left, e.Right);
          break;
        default:
          throw new InvalidOperationException(string.Empty);
      }
      this._bNeedToMakeUnicodeFalse = false;
      return (ISqlFragment) sqlBuilder;
    }

    private bool CheckIfNeedToMakeUnicodeFalse(DbExpression e)
    {
      if (this._bNeedToMakeUnicodeFalse)
        throw new NotSupportedException();
      if (e.ExpressionKind == DbExpressionKind.Like)
      {
        DbLikeExpression dbLikeExpression = (DbLikeExpression) e;
        if (SqlGenerator.IsSourceUnicodeFalse(dbLikeExpression.Argument) && this.IsTargetUnicodeNull(dbLikeExpression.Pattern))
          return this.IsTargetUnicodeNull(dbLikeExpression.Escape);
        return false;
      }
      DbComparisonExpression comparisonExpression = (DbComparisonExpression) e;
      DbExpression left = comparisonExpression.Left;
      DbExpression right = comparisonExpression.Right;
      if (SqlGenerator.IsSourceUnicodeFalse(left) && this.IsTargetUnicodeNull(right))
        return true;
      if (SqlGenerator.IsSourceUnicodeFalse(right))
        return this.IsTargetUnicodeNull(left);
      return false;
    }

    internal bool IsTargetUnicodeNull(DbExpression expr)
    {
      if (SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNull(expr))
        return true;
      if (expr.ExpressionKind == DbExpressionKind.Function)
      {
        DbFunctionExpression functionExpression = (DbFunctionExpression) expr;
        EdmFunction function = functionExpression.Function;
        if (!EF6MetadataHelpers.IsCanonicalFunction(function) && !SqlGenerator.IsBuiltInStoreFunction(function))
          return false;
        if ("Edm.Left".Equals(function.FullName, StringComparison.Ordinal) || "Edm.LTrim".Equals(function.FullName, StringComparison.Ordinal) || ("Edm.Reverse".Equals(function.FullName, StringComparison.Ordinal) || "Edm.Right".Equals(function.FullName, StringComparison.Ordinal)) || ("Edm.RTrim".Equals(function.FullName, StringComparison.Ordinal) || "Edm.Substring".Equals(function.FullName, StringComparison.Ordinal) || ("Edm.ToLower".Equals(function.FullName, StringComparison.Ordinal) || "Edm.ToUpper".Equals(function.FullName, StringComparison.Ordinal))) || "Edm.Trim".Equals(function.FullName, StringComparison.Ordinal))
          return this.IsTargetUnicodeNull(functionExpression.Arguments[0]);
        if ("Edm.Concat".Equals(function.FullName, StringComparison.Ordinal))
        {
          if (!this.IsTargetUnicodeNull(functionExpression.Arguments[0]) && !SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(functionExpression.Arguments[0]))
            return false;
          if (!this.IsTargetUnicodeNull(functionExpression.Arguments[1]))
            return SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(functionExpression.Arguments[1]);
          return true;
        }
        if ("Edm.Replace".Equals(function.FullName, StringComparison.Ordinal) && (this.IsTargetUnicodeNull(functionExpression.Arguments[0]) || SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(functionExpression.Arguments[0])) && (this.IsTargetUnicodeNull(functionExpression.Arguments[1]) || SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(functionExpression.Arguments[1])))
        {
          if (!this.IsTargetUnicodeNull(functionExpression.Arguments[2]))
            return SqlGenerator.IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(functionExpression.Arguments[2]);
          return true;
        }
      }
      return false;
    }

    private static bool IsSourceUnicodeFalse(DbExpression argument)
    {
      bool isUnicode;
      if (argument.ExpressionKind == DbExpressionKind.Property && EF6MetadataHelpers.TryGetIsUnicode(argument.ResultType, out isUnicode))
        return !isUnicode;
      return false;
    }

    internal static bool IsConstParamOrNullExpressionAndUnicodeIsNull(DbExpression argument)
    {
      DbExpressionKind expressionKind = argument.ExpressionKind;
      TypeUsage resultType = argument.ResultType;
      if (!EF6MetadataHelpers.IsPrimitiveType(resultType, PrimitiveTypeKind.String) || expressionKind != DbExpressionKind.Constant && expressionKind != DbExpressionKind.ParameterReference && expressionKind != DbExpressionKind.Null)
        return false;
      bool boolValue;
      return !EF6MetadataHelpers.TryGetBooleanFacetValue(resultType, "Unicode", out boolValue);
    }

    internal static bool IsConstParamOrNullExpressionAndUnicodeIsNullOrFalse(DbExpression argument)
    {
      DbExpressionKind expressionKind = argument.ExpressionKind;
      TypeUsage resultType = argument.ResultType;
      if (!EF6MetadataHelpers.IsPrimitiveType(resultType, PrimitiveTypeKind.String) || expressionKind != DbExpressionKind.Constant && expressionKind != DbExpressionKind.ParameterReference && expressionKind != DbExpressionKind.Null)
        return false;
      bool boolValue;
      if (EF6MetadataHelpers.TryGetBooleanFacetValue(resultType, "Unicode", out boolValue))
        return !boolValue;
      return true;
    }

    private ISqlFragment VisitConstant(DbConstantExpression e, bool isCastOptional)
    {
      SqlBuilder result = new SqlBuilder();
      PrimitiveTypeKind typeKind;
      if (!EF6MetadataHelpers.TryGetPrimitiveTypeKind(e.ResultType, out typeKind))
        throw new NotSupportedException();
      switch (typeKind)
      {
        case PrimitiveTypeKind.Binary:
          result.Append((object) " CAST('");
          result.Append((object) SqlGenerator.ByteArrayToBinaryString((byte[]) e.Value));
          result.Append((object) "' AS RAW(");
          result.Append((object) ((byte[]) e.Value).Length.ToString());
          result.Append((object) "))");
          break;
        case PrimitiveTypeKind.Boolean:
          result.Append((bool) e.Value ? (object) "1" : (object) "0");
          break;
        case PrimitiveTypeKind.Byte:
          SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "number(2)", result);
          break;
        case PrimitiveTypeKind.DateTime:
          result.Append((object) "TO_TIMESTAMP(");
          result.Append((object) SqlGenerator.EscapeSingleQuote(((DateTime) e.Value).ToString("yyyy-MM-dd HH:mm:ss.fff", (IFormatProvider) CultureInfo.InvariantCulture), false));
          result.Append((object) ", 'YYYY-MM-DD HH24:MI:SS.FF')");
          break;
        case PrimitiveTypeKind.Decimal:
          string str = ((Decimal) e.Value).ToString((IFormatProvider) CultureInfo.InvariantCulture);
          int num;
          if (-1 == str.IndexOf('.'))
            num = str.TrimStart('-').Length < 20 ? 1 : 0;
          else
            num = 0;
          bool flag = num != 0;
          string typeName = "decimal(" + Math.Max((byte) str.Length, (byte) 38).ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")";
          SqlGenerator.WrapWithCastIfNeeded(false, str, typeName, result);
          break;
        case PrimitiveTypeKind.Double:
          isCastOptional = true;
          if (this.IsPre10g)
          {
            SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, ((double) e.Value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), "number", result);
            break;
          }
          SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, ((double) e.Value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), "binary_double", result);
          break;
        case PrimitiveTypeKind.Guid:
          result.Append((object) " CAST('");
          if (e.Value is Guid)
            result.Append((object) SqlGenerator.ByteArrayToBinaryString(((Guid) e.Value).ToByteArray()));
          else
            result.Append((object) SqlGenerator.ByteArrayToBinaryString((byte[]) e.Value));
          result.Append((object) "' AS RAW(16))");
          break;
        case PrimitiveTypeKind.Single:
          isCastOptional = true;
          if (this.IsPre10g)
          {
            SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, ((float) e.Value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), "number", result);
            break;
          }
          SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, ((float) e.Value).ToString("R", (IFormatProvider) CultureInfo.InvariantCulture), "binary_float", result);
          break;
        case PrimitiveTypeKind.Int16:
          isCastOptional = true;
          SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "number(4)", result);
          break;
        case PrimitiveTypeKind.Int32:
          result.Append((object) e.Value.ToString());
          break;
        case PrimitiveTypeKind.Int64:
          isCastOptional = true;
          SqlGenerator.WrapWithCastIfNeeded(!isCastOptional, e.Value.ToString(), "number(18)", result);
          break;
        case PrimitiveTypeKind.String:
          bool isUnicode;
          if (!EF6MetadataHelpers.TryGetIsUnicode(e.ResultType, out isUnicode))
            isUnicode = false;
          result.Append((object) SqlGenerator.EscapeSingleQuote(e.Value as string, isUnicode));
          break;
        case PrimitiveTypeKind.Time:
          throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", typeKind.ToString()));
        case PrimitiveTypeKind.DateTimeOffset:
          result.Append((object) "TO_TIMESTAMP_TZ(");
          result.Append((object) SqlGenerator.EscapeSingleQuote(((DateTimeOffset) e.Value).ToString("yyyy-MM-dd HH:mm:ss.fff zzz", (IFormatProvider) CultureInfo.InvariantCulture), false));
          result.Append((object) ", 'yyyy-mm-dd HH24:MI:SS.FF3 TZH:TZM')");
          break;
        default:
          throw new NotSupportedException();
      }
      return (ISqlFragment) result;
    }

    private static void WrapWithCastIfNeeded(bool cast, string value, string typeName, SqlBuilder result)
    {
      if (!cast)
      {
        result.Append((object) value);
      }
      else
      {
        result.Append((object) "cast(");
        result.Append((object) value);
        result.Append((object) " as ");
        result.Append((object) typeName);
        result.Append((object) ")");
      }
    }

    public override ISqlFragment Visit(DbConstantExpression e)
    {
      return this.VisitConstant(e, false);
    }

    public override ISqlFragment Visit(DbDerefExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbDistinctExpression e)
    {
      SqlSelectStatement sqlSelectStatement = this.VisitExpressionEnsureSqlStatement(e.Argument);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
      {
        TypeUsage elementTypeUsage = EF6MetadataHelpers.GetElementTypeUsage(e.Argument.ResultType);
        Symbol fromSymbol;
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, "distinct", elementTypeUsage, out fromSymbol);
        this.AddFromSymbol(sqlSelectStatement, "distinct", fromSymbol, false);
      }
      sqlSelectStatement.IsDistinct = true;
      return (ISqlFragment) sqlSelectStatement;
    }

    public override ISqlFragment Visit(DbElementExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "(");
      sqlBuilder.Append((object) this.VisitExpressionEnsureSqlStatement(e.Argument));
      sqlBuilder.Append((object) ")");
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbExceptExpression e)
    {
      return this.VisitSetOpExpression(e.Left, e.Right, "MINUS");
    }

    public override ISqlFragment Visit(DbExpression e)
    {
      throw new InvalidOperationException(string.Empty);
    }

    public override ISqlFragment Visit(DbScanExpression e)
    {
      EntitySetBase target = e.Target;
      if (this.IsParentAJoin)
      {
        SqlBuilder sqlBuilder = new SqlBuilder();
        sqlBuilder.Append((object) SqlGenerator.GetTargetTSql(target));
        if (target.Name == "HistoryRow")
        {
          if (SqlGenerator.TargetOwner != null)
            SqlGenerator.TargetOwner.Add(target.Schema);
          if (SqlGenerator.TargetTable != null)
            SqlGenerator.TargetTable.Add(target.Table);
        }
        return (ISqlFragment) sqlBuilder;
      }
      SqlSelectStatement sqlSelectStatement = new SqlSelectStatement();
      sqlSelectStatement.From.Append((object) SqlGenerator.GetTargetTSql(target));
      if (target.Name == "HistoryRow")
      {
        if (SqlGenerator.TargetOwner != null)
          SqlGenerator.TargetOwner.Add(target.Schema);
        if (SqlGenerator.TargetTable != null)
          SqlGenerator.TargetTable.Add(target.Table);
      }
      return (ISqlFragment) sqlSelectStatement;
    }

    internal static string GetTargetTSql(EntitySetBase entitySetBase)
    {
      string metadataProperty1 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) entitySetBase, "DefiningQuery");
      if (metadataProperty1 != null)
        return "(" + metadataProperty1 + ")";
      string metadataProperty2 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) entitySetBase, "Schema");
      StringBuilder stringBuilder = new StringBuilder(50);
      if (!string.IsNullOrEmpty(metadataProperty2))
      {
        stringBuilder.Append(SqlGenerator.QuoteIdentifier(metadataProperty2));
        stringBuilder.Append(".");
      }
      string metadataProperty3 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) entitySetBase, "Table");
      if (!string.IsNullOrEmpty(metadataProperty3))
        stringBuilder.Append(SqlGenerator.QuoteIdentifier(metadataProperty3));
      else
        stringBuilder.Append(SqlGenerator.QuoteIdentifier(entitySetBase.Name));
      return stringBuilder.ToString();
    }

    public override ISqlFragment Visit(DbFilterExpression e)
    {
      return (ISqlFragment) this.VisitFilterExpression(e.Input, e.Predicate, false);
    }

    public override ISqlFragment Visit(DbFunctionExpression e)
    {
      if (SqlGenerator.IsSpecialCanonicalFunction(e))
        return this.HandleSpecialCanonicalFunction(e);
      return this.HandleFunctionDefault(e);
    }

    public override ISqlFragment Visit(DbLambdaExpression expression)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbEntityRefExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbRefKeyExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbGroupByExpression e)
    {
      Symbol fromSymbol;
      SqlSelectStatement sqlSelectStatement = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      this.selectStatementStack.Push(sqlSelectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(sqlSelectStatement, e.Input.VariableName, fromSymbol);
      this.symbolTable.Add(e.Input.GroupVariableName, fromSymbol);
      RowType edmType = EF6MetadataHelpers.GetEdmType<RowType>(EF6MetadataHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage);
      bool flag = SqlGenerator.GroupByAggregatesNeedInnerQuery(e.Aggregates) || SqlGenerator.GroupByKeysNeedInnerQuery(e.Keys, e.Input.VariableName);
      SqlSelectStatement selectStatement;
      if (flag)
      {
        selectStatement = this.CreateNewSelectStatement(sqlSelectStatement, e.Input.VariableName, e.Input.VariableType, false, out fromSymbol);
        this.AddFromSymbol(selectStatement, e.Input.VariableName, fromSymbol, false);
      }
      else
        selectStatement = sqlSelectStatement;
      using (IEnumerator<EdmProperty> enumerator = (IEnumerator<EdmProperty>) edmType.Properties.GetEnumerator())
      {
        enumerator.MoveNext();
        string str1 = "";
        foreach (DbExpression key in (IEnumerable<DbExpression>) e.Keys)
        {
          string str2 = SqlGenerator.QuoteIdentifier(enumerator.Current.Name);
          selectStatement.GroupBy.Append((object) str1);
          ISqlFragment sqlFragment = key.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
          if (!flag)
          {
            selectStatement.Select.Append((object) str1);
            selectStatement.Select.AppendLine();
            selectStatement.Select.Append((object) sqlFragment);
            selectStatement.Select.Append((object) " AS ");
            selectStatement.Select.Append((object) str2);
            selectStatement.GroupBy.Append((object) sqlFragment);
          }
          else
          {
            sqlSelectStatement.Select.Append((object) str1);
            sqlSelectStatement.Select.AppendLine();
            sqlSelectStatement.Select.Append((object) sqlFragment);
            sqlSelectStatement.Select.Append((object) " AS ");
            sqlSelectStatement.Select.Append((object) str2);
            selectStatement.Select.Append((object) str1);
            selectStatement.Select.AppendLine();
            selectStatement.Select.Append((object) fromSymbol);
            selectStatement.Select.Append((object) ".");
            selectStatement.Select.Append((object) str2);
            selectStatement.Select.Append((object) " AS ");
            selectStatement.Select.Append((object) str2);
            selectStatement.GroupBy.Append((object) str2);
          }
          str1 = ", ";
          enumerator.MoveNext();
        }
        foreach (DbAggregate aggregate in (IEnumerable<DbAggregate>) e.Aggregates)
        {
          string str2 = SqlGenerator.QuoteIdentifier(enumerator.Current.Name);
          ISqlFragment sqlFragment1 = aggregate.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
          object aggregateArgument;
          if (flag)
          {
            SqlBuilder sqlBuilder = new SqlBuilder();
            sqlBuilder.Append((object) fromSymbol);
            sqlBuilder.Append((object) ".");
            sqlBuilder.Append((object) str2);
            aggregateArgument = (object) sqlBuilder;
            sqlSelectStatement.Select.Append((object) str1);
            sqlSelectStatement.Select.AppendLine();
            sqlSelectStatement.Select.Append((object) sqlFragment1);
            sqlSelectStatement.Select.Append((object) " AS ");
            sqlSelectStatement.Select.Append((object) str2);
          }
          else
            aggregateArgument = (object) sqlFragment1;
          ISqlFragment sqlFragment2 = (ISqlFragment) SqlGenerator.VisitAggregate(aggregate, aggregateArgument);
          selectStatement.Select.Append((object) str1);
          selectStatement.Select.AppendLine();
          selectStatement.Select.Append((object) sqlFragment2);
          selectStatement.Select.Append((object) " AS ");
          selectStatement.Select.Append((object) str2);
          str1 = ", ";
          enumerator.MoveNext();
        }
      }
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      return (ISqlFragment) selectStatement;
    }

    public override ISqlFragment Visit(DbIntersectExpression e)
    {
      return this.VisitSetOpExpression(e.Left, e.Right, "INTERSECT");
    }

    public override ISqlFragment Visit(DbIsEmptyExpression e)
    {
      return (ISqlFragment) this.VisitIsEmptyExpression(e, false);
    }

    public override ISqlFragment Visit(DbIsNullExpression e)
    {
      return (ISqlFragment) this.VisitIsNullExpression(e, false);
    }

    public override ISqlFragment Visit(DbIsOfExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbCrossJoinExpression e)
    {
      return this.VisitJoinExpression(e.Inputs, e.ExpressionKind, "CROSS JOIN", (DbExpression) null);
    }

    public override ISqlFragment Visit(DbJoinExpression e)
    {
      string joinString;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.FullOuterJoin:
          joinString = "FULL OUTER JOIN";
          break;
        case DbExpressionKind.InnerJoin:
          joinString = "INNER JOIN";
          break;
        case DbExpressionKind.LeftOuterJoin:
          joinString = "LEFT OUTER JOIN";
          break;
        default:
          joinString = (string) null;
          break;
      }
      return this.VisitJoinExpression((IList<DbExpressionBinding>) new List<DbExpressionBinding>(2)
      {
        e.Left,
        e.Right
      }, e.ExpressionKind, joinString, e.JoinCondition);
    }

    public override ISqlFragment Visit(DbLikeExpression e)
    {
      this._bNeedToMakeUnicodeFalse = this.CheckIfNeedToMakeUnicodeFalse((DbExpression) e);
      SqlBuilder sqlBuilder1 = new SqlBuilder();
      sqlBuilder1.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      sqlBuilder1.Append((object) " LIKE ");
      sqlBuilder1.Append((object) e.Pattern.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      if (e.Escape.ExpressionKind != DbExpressionKind.Null)
      {
        sqlBuilder1.Append((object) " ESCAPE ");
        SqlBuilder sqlBuilder2 = (SqlBuilder) e.Escape.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
        if (!sqlBuilder2.IsEmpty && ((string) sqlBuilder2.sqlFragments[0]).StartsWith("N'"))
          sqlBuilder2.sqlFragments[0] = (object) ((string) sqlBuilder2.sqlFragments[0]).Remove(0, 1);
        sqlBuilder1.Append((object) sqlBuilder2);
      }
      this._bNeedToMakeUnicodeFalse = false;
      return (ISqlFragment) sqlBuilder1;
    }

    public override ISqlFragment Visit(DbLimitExpression e)
    {
      SqlSelectStatement sqlSelectStatement = this.VisitExpressionEnsureSqlStatement(e.Argument, false);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
      {
        TypeUsage elementTypeUsage = EF6MetadataHelpers.GetElementTypeUsage(e.Argument.ResultType);
        Symbol fromSymbol;
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, "top", elementTypeUsage, out fromSymbol);
        this.AddFromSymbol(sqlSelectStatement, "top", fromSymbol, false);
      }
      ISqlFragment topCount = this.HandleCountExpression(e.Limit);
      sqlSelectStatement.Top = new TopClause(topCount, e.WithTies);
      return (ISqlFragment) sqlSelectStatement;
    }

    public override ISqlFragment Visit(DbNewInstanceExpression e)
    {
      if (EF6MetadataHelpers.IsCollectionType((GlobalItem) e.ResultType.EdmType))
        return this.VisitCollectionConstructor(e);
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbNotExpression e)
    {
      DbNotExpression dbNotExpression = e.Argument as DbNotExpression;
      if (dbNotExpression != null)
        return dbNotExpression.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
      DbIsEmptyExpression e1 = e.Argument as DbIsEmptyExpression;
      if (e1 != null)
        return (ISqlFragment) this.VisitIsEmptyExpression(e1, true);
      DbIsNullExpression e2 = e.Argument as DbIsNullExpression;
      if (e2 != null)
        return (ISqlFragment) this.VisitIsNullExpression(e2, true);
      DbComparisonExpression comparisonExpression = e.Argument as DbComparisonExpression;
      if (comparisonExpression != null && comparisonExpression.ExpressionKind == DbExpressionKind.Equals)
      {
        bool makeUnicodeFalse = this._bNeedToMakeUnicodeFalse;
        ISqlFragment sqlFragment = (ISqlFragment) this.VisitBinaryExpression(" <> ", DbExpressionKind.NotEquals, comparisonExpression.Left, comparisonExpression.Right);
        this._bNeedToMakeUnicodeFalse = makeUnicodeFalse;
        return sqlFragment;
      }
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) " NOT (");
      sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      sqlBuilder.Append((object) ")");
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbNullExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "NULL");
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbOfTypeExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbOrExpression e)
    {
      return (ISqlFragment) this.VisitBinaryExpression(" OR ", e.ExpressionKind, e.Left, e.Right);
    }

    public override ISqlFragment Visit(DbParameterReferenceExpression e)
    {
      if (!this._bIgnoreMakingUnicodeFalse)
      {
        if (!this._bNeedToMakeUnicodeFalse)
          this.ListOfParamsForNonUnicode[e.ParameterName] = false;
        else if (!this.ListOfParamsForNonUnicode.ContainsKey(e.ParameterName))
          this.ListOfParamsForNonUnicode[e.ParameterName] = true;
      }
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) (":" + e.ParameterName));
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbProjectExpression e)
    {
      Symbol fromSymbol;
      SqlSelectStatement sqlSelectStatement = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      bool aliasesNeedRenaming = false;
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      this.selectStatementStack.Push(sqlSelectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(sqlSelectStatement, e.Input.VariableName, fromSymbol);
      DbNewInstanceExpression projection = e.Projection as DbNewInstanceExpression;
      if (projection != null)
      {
        Dictionary<string, Symbol> newColumns;
        sqlSelectStatement.Select.Append((object) this.VisitNewInstanceExpression(projection, aliasesNeedRenaming, out newColumns));
        if (aliasesNeedRenaming)
        {
          sqlSelectStatement.OutputColumnsRenamed = true;
          sqlSelectStatement.OutputColumns = newColumns;
        }
      }
      else
        sqlSelectStatement.Select.Append((object) e.Projection.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      return (ISqlFragment) sqlSelectStatement;
    }

    public override ISqlFragment Visit(DbPropertyExpression e)
    {
      ISqlFragment sqlFragment = e.Instance.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
      if (e.Instance is DbVariableReferenceExpression)
        this.isVarRefSingle = false;
      JoinSymbol joinSymbol = sqlFragment as JoinSymbol;
      if (joinSymbol != null)
      {
        if (joinSymbol.IsNestedJoin)
          return (ISqlFragment) new SymbolPair((Symbol) joinSymbol, joinSymbol.NameToExtent[e.Property.Name]);
        return (ISqlFragment) joinSymbol.NameToExtent[e.Property.Name];
      }
      SymbolPair symbolPair = sqlFragment as SymbolPair;
      if (symbolPair != null)
      {
        JoinSymbol column = symbolPair.Column as JoinSymbol;
        if (column != null)
        {
          symbolPair.Column = column.NameToExtent[e.Property.Name];
          return (ISqlFragment) symbolPair;
        }
        if (symbolPair.Column.Columns.ContainsKey(e.Property.Name))
        {
          SqlBuilder sqlBuilder = new SqlBuilder();
          sqlBuilder.Append((object) symbolPair.Source);
          sqlBuilder.Append((object) ".");
          sqlBuilder.Append((object) symbolPair.Column.Columns[e.Property.Name]);
          return (ISqlFragment) sqlBuilder;
        }
      }
      SqlBuilder sqlBuilder1 = new SqlBuilder();
      sqlBuilder1.Append((object) sqlFragment);
      sqlBuilder1.Append((object) ".");
      Symbol symbol = sqlFragment as Symbol;
      if (symbol != null && symbol.OutputColumnsRenamed)
        sqlBuilder1.Append((object) symbol.Columns[e.Property.Name]);
      else
        sqlBuilder1.Append((object) SqlGenerator.QuoteIdentifier(e.Property.Name));
      return (ISqlFragment) sqlBuilder1;
    }

    public override ISqlFragment Visit(DbQuantifierExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      bool negatePredicate = e.ExpressionKind == DbExpressionKind.All;
      if (e.ExpressionKind == DbExpressionKind.Any)
        sqlBuilder.Append((object) "EXISTS (");
      else
        sqlBuilder.Append((object) "NOT EXISTS (");
      SqlSelectStatement selectStatement = this.VisitFilterExpression(e.Input, e.Predicate, negatePredicate);
      if (selectStatement.Select.IsEmpty)
        this.AddDefaultColumns(selectStatement);
      sqlBuilder.Append((object) selectStatement);
      sqlBuilder.Append((object) ")");
      return (ISqlFragment) sqlBuilder;
    }

    public override ISqlFragment Visit(DbRefExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbRelationshipNavigationExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbSkipExpression e)
    {
      Symbol fromSymbol1;
      SqlSelectStatement sqlSelectStatement = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out fromSymbol1);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, e.Input.VariableName, e.Input.VariableType, out fromSymbol1);
      this.selectStatementStack.Push(sqlSelectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(sqlSelectStatement, e.Input.VariableName, fromSymbol1);
      List<Symbol> symbolList = this.AddDefaultColumns(sqlSelectStatement);
      sqlSelectStatement.Select.Append((object) ", row_number() OVER (ORDER BY ");
      this.AddSortKeys(sqlSelectStatement.Select, e.SortOrder);
      sqlSelectStatement.Select.Append((object) ") AS ");
      Symbol symbol = new Symbol("row_number", (TypeUsage) null);
      sqlSelectStatement.Select.Append((object) symbol);
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      SqlSelectStatement selectStatement = new SqlSelectStatement();
      selectStatement.From.Append((object) "( ");
      selectStatement.From.Append((object) sqlSelectStatement);
      selectStatement.From.AppendLine();
      selectStatement.From.Append((object) ") ");
      Symbol fromSymbol2 = (Symbol) null;
      if (sqlSelectStatement.FromExtents.Count == 1)
      {
        JoinSymbol fromExtent = sqlSelectStatement.FromExtents[0] as JoinSymbol;
        if (fromExtent != null)
          fromSymbol2 = (Symbol) new JoinSymbol(e.Input.VariableName, e.Input.VariableType, fromExtent.ExtentList)
          {
            IsNestedJoin = true,
            ColumnList = symbolList,
            FlattenedExtentList = fromExtent.FlattenedExtentList
          };
      }
      if (fromSymbol2 == null)
        fromSymbol2 = new Symbol(e.Input.VariableName, e.Input.VariableType);
      this.selectStatementStack.Push(selectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(selectStatement, e.Input.VariableName, fromSymbol2);
      selectStatement.Where.Append((object) fromSymbol2);
      selectStatement.Where.Append((object) ".");
      selectStatement.Where.Append((object) symbol);
      selectStatement.Where.Append((object) " > ");
      selectStatement.Where.Append((object) this.HandleCountExpression(e.Count));
      this.AddSortKeys(selectStatement.OrderBy, e.SortOrder);
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      return (ISqlFragment) selectStatement;
    }

    public override ISqlFragment Visit(DbSortExpression e)
    {
      Symbol fromSymbol;
      SqlSelectStatement sqlSelectStatement = this.VisitInputExpression(e.Input.Expression, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, e.ExpressionKind))
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, e.Input.VariableName, e.Input.VariableType, out fromSymbol);
      this.selectStatementStack.Push(sqlSelectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(sqlSelectStatement, e.Input.VariableName, fromSymbol);
      this.AddSortKeys(sqlSelectStatement.OrderBy, e.SortOrder);
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      return (ISqlFragment) sqlSelectStatement;
    }

    public override ISqlFragment Visit(DbTreatExpression e)
    {
      throw new NotSupportedException();
    }

    public override ISqlFragment Visit(DbUnionAllExpression e)
    {
      return this.VisitSetOpExpression(e.Left, e.Right, "UNION ALL");
    }

    public override ISqlFragment Visit(DbVariableReferenceExpression e)
    {
      if (this.isVarRefSingle)
        throw new NotSupportedException();
      this.isVarRefSingle = true;
      Symbol index = this.symbolTable.Lookup(e.VariableName);
      if (!this.CurrentSelectStatement.FromExtents.Contains(index))
        this.CurrentSelectStatement.OuterExtents[index] = true;
      return (ISqlFragment) index;
    }

    private static SqlBuilder VisitAggregate(DbAggregate aggregate, object aggregateArgument)
    {
      SqlBuilder result = new SqlBuilder();
      DbFunctionAggregate functionAggregate1 = aggregate as DbFunctionAggregate;
      if (functionAggregate1 == null)
        throw new NotSupportedException();
      SqlGenerator.WriteFunctionName(result, functionAggregate1.Function);
      result.Append((object) "(");
      DbFunctionAggregate functionAggregate2 = functionAggregate1;
      if (functionAggregate2 != null && functionAggregate2.Distinct)
        result.Append((object) "DISTINCT ");
      result.Append(aggregateArgument);
      result.Append((object) ")");
      return result;
    }

    private void ParanthesizeExpressionIfNeeded(DbExpression e, SqlBuilder result)
    {
      if (SqlGenerator.IsComplexExpression(e))
      {
        result.Append((object) "(");
        result.Append((object) e.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        result.Append((object) ")");
      }
      else
        result.Append((object) e.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
    }

    private SqlBuilder VisitBinaryExpression(string op, DbExpressionKind expressionKind, DbExpression left, DbExpression right)
    {
      SqlBuilder result = new SqlBuilder();
      bool flag = true;
      foreach (DbExpression e in Oracle.ManagedDataAccess.EntityFramework.CommandTreeUtils.FlattenAssociativeExpression(expressionKind, left, right))
      {
        if (flag)
          flag = false;
        else
          result.Append((object) op);
        this.ParanthesizeExpressionIfNeeded(e, result);
      }
      return result;
    }

    private SqlBuilder VisitComparisonExpression(string op, DbExpression left, DbExpression right)
    {
      SqlBuilder result = new SqlBuilder();
      bool isCastOptional = left.ResultType.EdmType == right.ResultType.EdmType;
      if (left.ExpressionKind == DbExpressionKind.Constant)
        result.Append((object) this.VisitConstant((DbConstantExpression) left, isCastOptional));
      else
        this.ParanthesizeExpressionIfNeeded(left, result);
      result.Append((object) op);
      if (right.ExpressionKind == DbExpressionKind.Constant)
        result.Append((object) this.VisitConstant((DbConstantExpression) right, isCastOptional));
      else
        this.ParanthesizeExpressionIfNeeded(right, result);
      return result;
    }

    private SqlSelectStatement VisitInputExpression(DbExpression inputExpression, string inputVarName, TypeUsage inputVarType, out Symbol fromSymbol)
    {
      ISqlFragment sqlFragment = inputExpression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
      SqlSelectStatement result = sqlFragment as SqlSelectStatement;
      if (result == null)
      {
        result = new SqlSelectStatement();
        SqlGenerator.WrapNonQueryExtent(result, sqlFragment, inputExpression.ExpressionKind);
      }
      if (result.FromExtents.Count == 0)
        fromSymbol = new Symbol(inputVarName, inputVarType);
      else if (result.FromExtents.Count == 1)
      {
        fromSymbol = result.FromExtents[0];
      }
      else
      {
        fromSymbol = (Symbol) new JoinSymbol(inputVarName, inputVarType, result.FromExtents)
        {
          FlattenedExtentList = result.AllJoinExtents
        };
        result.FromExtents.Clear();
        result.FromExtents.Add(fromSymbol);
      }
      return result;
    }

    private SqlBuilder VisitIsEmptyExpression(DbIsEmptyExpression e, bool negate)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      if (!negate)
        sqlBuilder.Append((object) " NOT");
      sqlBuilder.Append((object) " EXISTS (");
      sqlBuilder.Append((object) this.VisitExpressionEnsureSqlStatement(e.Argument));
      sqlBuilder.AppendLine();
      sqlBuilder.Append((object) ")");
      return sqlBuilder;
    }

    private ISqlFragment VisitCollectionConstructor(DbNewInstanceExpression e)
    {
      if (e.Arguments.Count == 1 && e.Arguments[0].ExpressionKind == DbExpressionKind.Element)
      {
        DbElementExpression elementExpression = e.Arguments[0] as DbElementExpression;
        SqlSelectStatement sqlSelectStatement = this.VisitExpressionEnsureSqlStatement(elementExpression.Argument);
        if (!SqlGenerator.IsCompatible(sqlSelectStatement, DbExpressionKind.Element))
        {
          TypeUsage elementTypeUsage = EF6MetadataHelpers.GetElementTypeUsage(elementExpression.Argument.ResultType);
          Symbol fromSymbol;
          sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, "element", elementTypeUsage, out fromSymbol);
          this.AddFromSymbol(sqlSelectStatement, "element", fromSymbol, false);
        }
        sqlSelectStatement.Top = new TopClause(1, false);
        return (ISqlFragment) sqlSelectStatement;
      }
      CollectionType edmType = EF6MetadataHelpers.GetEdmType<CollectionType>(e.ResultType);
      bool flag = EF6MetadataHelpers.IsPrimitiveType(edmType.TypeUsage.EdmType);
      SqlBuilder sqlBuilder = new SqlBuilder();
      string str = "";
      if (e.Arguments.Count == 0)
      {
        sqlBuilder.Append((object) " SELECT CAST(null as ");
        sqlBuilder.Append((object) this.GetSqlPrimitiveType(edmType.TypeUsage));
        sqlBuilder.Append((object) ") AS X FROM DUAL Y WHERE 1=0");
      }
      foreach (DbExpression dbExpression in (IEnumerable<DbExpression>) e.Arguments)
      {
        sqlBuilder.Append((object) str);
        sqlBuilder.Append((object) " SELECT ");
        sqlBuilder.Append((object) dbExpression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        if (flag)
          sqlBuilder.Append((object) " FROM DUAL ");
        str = " UNION ALL ";
      }
      return (ISqlFragment) sqlBuilder;
    }

    private SqlBuilder VisitIsNullExpression(DbIsNullExpression e, bool negate)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      if (e.Argument.ExpressionKind == DbExpressionKind.ParameterReference)
        this._bIgnoreMakingUnicodeFalse = true;
      sqlBuilder.Append((object) e.Argument.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      this._bIgnoreMakingUnicodeFalse = false;
      if (!negate)
        sqlBuilder.Append((object) " IS NULL");
      else
        sqlBuilder.Append((object) " IS NOT NULL");
      return sqlBuilder;
    }

    private ISqlFragment VisitJoinExpression(IList<DbExpressionBinding> inputs, DbExpressionKind joinKind, string joinString, DbExpression joinCondition)
    {
      SqlSelectStatement result;
      if (!this.IsParentAJoin)
      {
        result = new SqlSelectStatement();
        result.AllJoinExtents = new List<Symbol>();
        this.selectStatementStack.Push(result);
      }
      else
        result = this.CurrentSelectStatement;
      this.symbolTable.EnterScope();
      string str = "";
      bool flag = true;
      int count1 = inputs.Count;
      for (int index = 0; index < count1; ++index)
      {
        DbExpressionBinding input = inputs[index];
        if (str.Length != 0)
          result.From.AppendLine();
        result.From.Append((object) (str + " "));
        this.isParentAJoinStack.Push(input.Expression.ExpressionKind == DbExpressionKind.Scan || flag && (SqlGenerator.IsJoinExpression(input.Expression) || SqlGenerator.IsApplyExpression(input.Expression)));
        int count2 = result.FromExtents.Count;
        ISqlFragment fromExtentFragment = input.Expression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
        this.isParentAJoinStack.Pop();
        this.ProcessJoinInputResult(fromExtentFragment, result, input, count2);
        str = joinString;
        flag = false;
      }
      switch (joinKind)
      {
        case DbExpressionKind.FullOuterJoin:
        case DbExpressionKind.InnerJoin:
        case DbExpressionKind.LeftOuterJoin:
          result.From.Append((object) " ON ");
          this.isParentAJoinStack.Push(false);
          result.From.Append((object) joinCondition.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
          this.isParentAJoinStack.Pop();
          break;
      }
      this.symbolTable.ExitScope();
      if (!this.IsParentAJoin)
        this.selectStatementStack.Pop();
      return (ISqlFragment) result;
    }

    private void ProcessJoinInputResult(ISqlFragment fromExtentFragment, SqlSelectStatement result, DbExpressionBinding input, int fromSymbolStart)
    {
      Symbol fromSymbol = (Symbol) null;
      if (result != fromExtentFragment)
      {
        SqlSelectStatement selectStatement = fromExtentFragment as SqlSelectStatement;
        if (selectStatement != null)
        {
          if (selectStatement.Select.IsEmpty)
          {
            List<Symbol> symbolList = this.AddDefaultColumns(selectStatement);
            if (SqlGenerator.IsJoinExpression(input.Expression) || SqlGenerator.IsApplyExpression(input.Expression))
            {
              List<Symbol> fromExtents = selectStatement.FromExtents;
              fromSymbol = (Symbol) new JoinSymbol(input.VariableName, input.VariableType, fromExtents)
              {
                IsNestedJoin = true,
                ColumnList = symbolList
              };
            }
            else
            {
              JoinSymbol fromExtent = selectStatement.FromExtents[0] as JoinSymbol;
              if (fromExtent != null)
                fromSymbol = (Symbol) new JoinSymbol(input.VariableName, input.VariableType, fromExtent.ExtentList)
                {
                  IsNestedJoin = true,
                  ColumnList = symbolList,
                  FlattenedExtentList = fromExtent.FlattenedExtentList
                };
              else if (selectStatement.FromExtents[0].OutputColumnsRenamed)
                fromSymbol = new Symbol(input.VariableName, input.VariableType, selectStatement.FromExtents[0].Columns);
            }
          }
          else if (selectStatement.OutputColumnsRenamed)
            fromSymbol = new Symbol(input.VariableName, input.VariableType, selectStatement.OutputColumns);
          result.From.Append((object) " (");
          result.From.Append((object) selectStatement);
          result.From.Append((object) " )");
        }
        else if (input.Expression is DbScanExpression)
          result.From.Append((object) fromExtentFragment);
        else
          SqlGenerator.WrapNonQueryExtent(result, fromExtentFragment, input.Expression.ExpressionKind);
        if (fromSymbol == null)
          fromSymbol = new Symbol(input.VariableName, input.VariableType);
        this.AddFromSymbol(result, input.VariableName, fromSymbol);
        result.AllJoinExtents.Add(fromSymbol);
      }
      else
      {
        List<Symbol> extents = new List<Symbol>();
        for (int index = fromSymbolStart; index < result.FromExtents.Count; ++index)
          extents.Add(result.FromExtents[index]);
        result.FromExtents.RemoveRange(fromSymbolStart, result.FromExtents.Count - fromSymbolStart);
        Symbol symbol = (Symbol) new JoinSymbol(input.VariableName, input.VariableType, extents);
        result.FromExtents.Add(symbol);
        this.symbolTable.Add(input.VariableName, symbol);
      }
    }

    private ISqlFragment VisitNewInstanceExpression(DbNewInstanceExpression e, bool aliasesNeedRenaming, out Dictionary<string, Symbol> newColumns)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      RowType edmType = e.ResultType.EdmType as RowType;
      if (edmType == null)
        throw new NotSupportedException();
      newColumns = !aliasesNeedRenaming ? (Dictionary<string, Symbol>) null : new Dictionary<string, Symbol>(e.Arguments.Count);
      ReadOnlyMetadataCollection<EdmProperty> properties = edmType.Properties;
      string str = "";
      for (int index = 0; index < e.Arguments.Count; ++index)
      {
        DbExpression dbExpression = e.Arguments[index];
        if (EF6MetadataHelpers.IsRowType((GlobalItem) dbExpression.ResultType.EdmType))
          throw new NotSupportedException();
        EdmProperty edmProperty = properties[index];
        sqlBuilder.Append((object) str);
        sqlBuilder.AppendLine();
        sqlBuilder.Append((object) dbExpression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        sqlBuilder.Append((object) " AS ");
        if (aliasesNeedRenaming)
        {
          Symbol symbol = new Symbol(edmProperty.Name, edmProperty.TypeUsage);
          symbol.NeedsRenaming = true;
          symbol.NewName = "Internal_" + edmProperty.Name;
          sqlBuilder.Append((object) symbol);
          newColumns.Add(edmProperty.Name, symbol);
        }
        else
          sqlBuilder.Append((object) SqlGenerator.QuoteIdentifier(edmProperty.Name));
        str = ", ";
      }
      return (ISqlFragment) sqlBuilder;
    }

    private ISqlFragment VisitSetOpExpression(DbExpression left, DbExpression right, string separator)
    {
      SqlSelectStatement sqlSelectStatement1 = this.VisitExpressionEnsureSqlStatement(left);
      SqlSelectStatement sqlSelectStatement2 = this.VisitExpressionEnsureSqlStatement(right);
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) sqlSelectStatement1);
      sqlBuilder.AppendLine();
      sqlBuilder.Append((object) separator);
      sqlBuilder.AppendLine();
      sqlBuilder.Append((object) sqlSelectStatement2);
      if (!sqlSelectStatement1.OutputColumnsRenamed)
        return (ISqlFragment) sqlBuilder;
      SqlSelectStatement selectStatement = new SqlSelectStatement();
      selectStatement.From.Append((object) "( ");
      selectStatement.From.Append((object) sqlBuilder);
      selectStatement.From.AppendLine();
      selectStatement.From.Append((object) ") ");
      Symbol fromSymbol = new Symbol("X", left.ResultType, sqlSelectStatement1.OutputColumns);
      this.AddFromSymbol(selectStatement, (string) null, fromSymbol, false);
      return (ISqlFragment) selectStatement;
    }

    private static bool IsSpecialCanonicalFunction(DbFunctionExpression e)
    {
      if (EF6MetadataHelpers.IsCanonicalFunction(e.Function))
        return SqlGenerator._canonicalFunctionHandlers.ContainsKey(e.Function.Name);
      return false;
    }

    private ISqlFragment HandleFunctionDefault(DbFunctionExpression e)
    {
      SqlBuilder result = new SqlBuilder();
      SqlGenerator.WriteFunctionName(result, e.Function);
      this.HandleFunctionArgumentsDefault(e, result);
      return (ISqlFragment) result;
    }

    private ISqlFragment HandleFunctionDefaultGivenName(DbFunctionExpression e, string functionName)
    {
      SqlBuilder result = new SqlBuilder();
      result.Append((object) functionName);
      this.HandleFunctionArgumentsDefault(e, result);
      return (ISqlFragment) result;
    }

    private void HandleFunctionArgumentsDefault(DbFunctionExpression e, SqlBuilder result)
    {
      bool metadataProperty = EF6MetadataHelpers.GetMetadataProperty<bool>((MetadataItem) e.Function, "NiladicFunctionAttribute");
      if (metadataProperty && e.Arguments.Count > 0)
        throw new MetadataException(EFProviderSettings.Instance.GetErrorMessage(-5000));
      if (metadataProperty)
        return;
      result.Append((object) "(");
      string str = "";
      foreach (DbExpression dbExpression in (IEnumerable<DbExpression>) e.Arguments)
      {
        result.Append((object) str);
        result.Append((object) dbExpression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        str = ", ";
      }
      result.Append((object) ")");
    }

    private ISqlFragment HandleSpecialCanonicalFunction(DbFunctionExpression e)
    {
      return this.HandleSpecialFunction(SqlGenerator._canonicalFunctionHandlers, e);
    }

    private ISqlFragment HandleSpecialFunction(Dictionary<string, SqlGenerator.FunctionHandler> handlers, DbFunctionExpression e)
    {
      return handlers[e.Function.Name](this, e);
    }

    private static ISqlFragment HandleCanonicalFunctionSubstring(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "SUBSTR");
    }

    private static ISqlFragment HandleCanonicalFunctionLeft(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "SUBSTR (");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ",1,");
      sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionRight(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "(CASE WHEN LENGTH(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ") >= (");
      sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ") THEN ");
      sqlBuilder.Append((object) "SUBSTR (");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ",-(");
      sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) "),");
      sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")");
      sqlBuilder.Append((object) " ELSE ");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) " END)");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleConcatFunction(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "((");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")||(");
      sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) "))");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionBitwise(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      switch (e.Function.Name.ToUpperInvariant())
      {
        case "BITWISEAND":
          sqlBuilder.Append((object) "BITAND(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ",");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ")");
          break;
        case "BITWISEOR":
          sqlBuilder.Append((object) "((");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ")+(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ")-BITAND(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ",");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "))");
          break;
        case "BITWISEXOR":
          sqlBuilder.Append((object) "((");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ")+(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ")-2*BITAND(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ",");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "))");
          break;
        case "BITWISENOT":
          sqlBuilder.Append((object) "((0 - ");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ") - 1)");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionDatepart(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      if (e.Function.Name.ToUpperInvariant() == "MILLISECOND")
      {
        sqlBuilder.Append((object) " NVL(TO_NUMBER(SUBSTR(TO_CHAR(CAST(");
        sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
        sqlBuilder.Append((object) " AS TIMESTAMP(3))");
        sqlBuilder.Append((object) " , 'DD-MON-RR HH24:MI:SSXFF'), 20, 3)), 0) ");
        return (ISqlFragment) sqlBuilder;
      }
      if (e.Function.Name.ToUpperInvariant() == "DAYOFYEAR")
      {
        sqlBuilder.Append((object) " TO_NUMBER(TO_CHAR(");
        sqlBuilder.Append((object) "CAST(");
        sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
        sqlBuilder.Append((object) " AS TIMESTAMP)");
        sqlBuilder.Append((object) ", 'DDD')) ");
        return (ISqlFragment) sqlBuilder;
      }
      sqlBuilder.Append((object) "EXTRACT (");
      sqlBuilder.Append((object) e.Function.Name.ToUpperInvariant());
      sqlBuilder.Append((object) " FROM (");
      sqlBuilder.Append((object) " CAST(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) " AS TIMESTAMP)");
      sqlBuilder.Append((object) "))");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionDatepartAdd(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      PrimitiveTypeKind typeKind;
      EF6MetadataHelpers.TryGetPrimitiveTypeKind(e.Arguments[0].ResultType, out typeKind);
      if (typeKind != PrimitiveTypeKind.DateTimeOffset)
        sqlBuilder.Append((object) " CAST(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      if (typeKind != PrimitiveTypeKind.DateTimeOffset)
        sqlBuilder.Append((object) " AS TIMESTAMP(9))");
      sqlBuilder.Append((object) " + ");
      switch (e.Function.Name.ToUpperInvariant())
      {
        case "ADDYEARS":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' YEAR(9) ");
          break;
        case "ADDMONTHS":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' MONTH(9) ");
          break;
        case "ADDDAYS":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' DAY(9) ");
          break;
        case "ADDHOURS":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' HOUR(9) ");
          break;
        case "ADDMINUTES":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' MINUTE(9) ");
          break;
        case "ADDSECONDS":
          sqlBuilder.Append((object) " INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' SECOND(9) ");
          break;
        case "ADDMILLISECONDS":
          sqlBuilder.Append((object) " (INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' SECOND(9) / 1000) ");
          break;
        case "ADDMICROSECONDS":
          sqlBuilder.Append((object) " (INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' SECOND(9) / (1000 * 1000)) ");
          break;
        case "ADDNANOSECONDS":
          sqlBuilder.Append((object) " (INTERVAL '");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "' SECOND(9) / (1000 * 1000 * 1000)) ");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionDatepartDiff(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      PrimitiveTypeKind typeKind;
      EF6MetadataHelpers.TryGetPrimitiveTypeKind(e.Arguments[1].ResultType, out typeKind);
      SqlBuilder sqlBuilder = new SqlBuilder();
      switch (e.Function.Name.ToUpperInvariant())
      {
        case "DIFFYEARS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " TRUNC(EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) / 365) ");
            break;
          }
          sqlBuilder.Append((object) " TRUNC(EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) / 365) ");
          break;
        case "DIFFMONTHS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " TRUNC(EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) / 31) ");
            break;
          }
          sqlBuilder.Append((object) " TRUNC(EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) / 31) ");
          break;
        case "DIFFDAYS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) ");
          break;
        case "DIFFHOURS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*24 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) ");
          break;
        case "DIFFMINUTES":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24*60 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " MINUTE FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*24*60 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " MINUTE FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) ");
          break;
        case "DIFFSECONDS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24*60*60 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*60 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " MINUTE FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " SECOND FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) ")) ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*24*60*60 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60*60 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " MINUTE FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " SECOND FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) ")) ");
          break;
        case "DIFFMILLISECONDS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24*60*60*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*60*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " MINUTE FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " SECOND FROM (");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*1000 ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*24*60*60*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9)) ");
          sqlBuilder.Append((object) "))*60*60*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " MINUTE FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9)) ");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9)) ");
          sqlBuilder.Append((object) "))*60*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " SECOND FROM (");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*1000 ");
          break;
        case "DIFFMICROSECONDS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24*60*60*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*60*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " MINUTE FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " SECOND FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*1000*1000 ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*24*60*60*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60*60*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " MINUTE FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " SECOND FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*1000*1000 ");
          break;
        case "DIFFNANOSECONDS":
          if (typeKind == PrimitiveTypeKind.DateTimeOffset)
          {
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " DAY FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*24*60*60*1000*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " HOUR FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*60*1000*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " MINUTE FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*60*1000*1000*1000 + ");
            sqlBuilder.Append((object) " EXTRACT(");
            sqlBuilder.Append((object) " SECOND FROM(");
            sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) " - ");
            sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
            sqlBuilder.Append((object) "))*1000*1000*1000 ");
            break;
          }
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " DAY FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9)) ");
          sqlBuilder.Append((object) "))*24*60*60*1000*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " HOUR FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60*60*1000*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " MINUTE FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*60*1000*1000*1000 + ");
          sqlBuilder.Append((object) " EXTRACT(");
          sqlBuilder.Append((object) " SECOND FROM(");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) " - ");
          sqlBuilder.Append((object) " CAST(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " AS TIMESTAMP(9))");
          sqlBuilder.Append((object) "))*1000*1000*1000 ");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionGetTotalOffsetMinutes(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "(EXTRACT (TIMEZONE_HOUR FROM (");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")) * 60 + EXTRACT (TIMEZONE_MINUTE FROM (");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")))");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionCurrentDateTime(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      switch (e.Function.Name.ToUpperInvariant())
      {
        case "CURRENTDATETIME":
          sqlBuilder.Append((object) "LOCALTIMESTAMP");
          break;
        case "CURRENTUTCDATETIME":
          sqlBuilder.Append((object) "SYS_EXTRACT_UTC(LOCALTIMESTAMP)");
          break;
        case "CURRENTDATETIMEOFFSET":
          sqlBuilder.Append((object) "SYSTIMESTAMP");
          break;
        case "CREATEDATETIME":
          sqlBuilder.Append((object) "TO_TIMESTAMP(");
          sqlBuilder.Append((object) "'");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "-");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "-");
          sqlBuilder.Append((object) e.Arguments[2].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " ");
          sqlBuilder.Append((object) e.Arguments[3].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ":");
          sqlBuilder.Append((object) e.Arguments[4].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ":");
          sqlBuilder.Append((object) ((DbConstantExpression) e.Arguments[5]).Value.ToString());
          sqlBuilder.Append((object) "'");
          sqlBuilder.Append((object) ", 'YYYY-MM-DD HH24:MI:SS.FF')");
          break;
        case "CREATEDATETIMEOFFSET":
          sqlBuilder.Append((object) "TO_TIMESTAMP_TZ(");
          sqlBuilder.Append((object) "'");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "-");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "-");
          sqlBuilder.Append((object) e.Arguments[2].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) " ");
          sqlBuilder.Append((object) e.Arguments[3].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ":");
          sqlBuilder.Append((object) e.Arguments[4].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ":");
          sqlBuilder.Append((object) string.Format("{0:00.000000000}", ((DbConstantExpression) e.Arguments[5]).Value));
          sqlBuilder.Append((object) " ");
          sqlBuilder.Append((object) e.Arguments[6].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "'");
          sqlBuilder.Append((object) ", 'yyyy-mm-dd HH24:MI:SS.FF TZH:TZM')");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionIndexOf(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "INSTR");
    }

    private static ISqlFragment HandleCanonicalFunctionIndexOf2(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      switch (e.Function.Name.ToUpperInvariant())
      {
        case "INDEXOF":
          sqlBuilder.Append((object) " NVL(INSTR(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ", ");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "), 0) ");
          break;
        case "CONTAINS":
          sqlBuilder.Append((object) " CASE WHEN ");
          sqlBuilder.Append((object) " NVL(INSTR(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ", ");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "), 0) ");
          sqlBuilder.Append((object) " != 0 THEN 1 ELSE 0 END ");
          break;
        case "STARTSWITH":
          sqlBuilder.Append((object) " CASE WHEN ");
          sqlBuilder.Append((object) " NVL(INSTR(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ", ");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) "), 0) ");
          sqlBuilder.Append((object) " = 1 THEN 1 ELSE 0 END ");
          break;
        case "ENDSWITH":
          sqlBuilder.Append((object) " CASE WHEN NVL(INSTR(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ", ");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ", LENGTH(");
          sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ") - LENGTH(");
          sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
          sqlBuilder.Append((object) ") + 1, 1 ), 0) > 0 THEN 1 ELSE 0 END ");
          break;
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionNewGuid(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "SYS_GUID");
    }

    private static ISqlFragment HandleCanonicalFunctionLength(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "LENGTH(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) ")");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionRound(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "ROUND(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      if (e.Arguments.Count == 1)
      {
        sqlBuilder.Append((object) ", 0)");
      }
      else
      {
        sqlBuilder.Append((object) ", ");
        sqlBuilder.Append((object) e.Arguments[1].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
        sqlBuilder.Append((object) ")");
      }
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionTrim(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      SqlBuilder sqlBuilder = new SqlBuilder();
      sqlBuilder.Append((object) "LTRIM(RTRIM(");
      sqlBuilder.Append((object) e.Arguments[0].Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) sqlgen));
      sqlBuilder.Append((object) "))");
      return (ISqlFragment) sqlBuilder;
    }

    private static ISqlFragment HandleCanonicalFunctionToLower(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "LOWER");
    }

    private static ISqlFragment HandleCanonicalFunctionToUpper(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "UPPER");
    }

    private static ISqlFragment HandleCanonicalFunctionCeiling(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "CEIL");
    }

    private static ISqlFragment HandleCanonicalFunctionTruncate(SqlGenerator sqlgen, DbFunctionExpression e)
    {
      return sqlgen.HandleFunctionDefaultGivenName(e, "TRUNC");
    }

    private static void WriteFunctionName(SqlBuilder result, EdmFunction function)
    {
      string metadataProperty1 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) function, "StoreFunctionNameAttribute");
      string name = metadataProperty1 == null ? function.Name : metadataProperty1;
      if (EF6MetadataHelpers.IsCanonicalFunction(function))
      {
        if (name.ToUpperInvariant() == "STDEV")
          result.Append((object) "STDDEV");
        else if (name.ToUpperInvariant() == "STDEVP")
          result.Append((object) "STDDEV_POP");
        else if (name.ToUpperInvariant() == "VAR")
          result.Append((object) "VARIANCE");
        else if (name.ToUpperInvariant() == "VARP")
          result.Append((object) "VAR_POP");
        else if (name.ToUpperInvariant() == "BIGCOUNT")
          result.Append((object) "COUNT");
        else
          result.Append((object) name.ToUpperInvariant());
      }
      else if (SqlGenerator.IsBuiltInStoreFunction(function))
      {
        result.Append((object) name);
      }
      else
      {
        string metadataProperty2 = EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) function, "Schema");
        if (string.IsNullOrEmpty(metadataProperty2))
          result.Append((object) SqlGenerator.QuoteIdentifier(function.NamespaceName));
        else
          result.Append((object) SqlGenerator.QuoteIdentifier(metadataProperty2));
        result.Append((object) ".");
        result.Append((object) SqlGenerator.QuoteIdentifier_storeFunctionName(name));
      }
    }

    private void AddColumns(SqlSelectStatement selectStatement, Symbol symbol, List<Symbol> columnList, Dictionary<string, Symbol> columnDictionary, ref string separator)
    {
      JoinSymbol joinSymbol = symbol as JoinSymbol;
      if (joinSymbol != null)
      {
        if (!joinSymbol.IsNestedJoin)
        {
          foreach (Symbol extent in joinSymbol.ExtentList)
          {
            if (extent.Type != null && !EF6MetadataHelpers.IsPrimitiveType(extent.Type.EdmType))
              this.AddColumns(selectStatement, extent, columnList, columnDictionary, ref separator);
          }
        }
        else
        {
          foreach (Symbol column in joinSymbol.ColumnList)
          {
            selectStatement.Select.Append((object) separator);
            selectStatement.Select.Append((object) symbol);
            selectStatement.Select.Append((object) ".");
            selectStatement.Select.Append((object) column);
            if (columnDictionary.ContainsKey(column.Name))
            {
              columnDictionary[column.Name].NeedsRenaming = true;
              column.NeedsRenaming = true;
            }
            else
              columnDictionary[column.Name] = column;
            columnList.Add(column);
            separator = ", ";
          }
        }
      }
      else
      {
        if (symbol.OutputColumnsRenamed)
        {
          selectStatement.OutputColumnsRenamed = true;
          selectStatement.OutputColumns = new Dictionary<string, Symbol>();
        }
        if (symbol.Type == null || EF6MetadataHelpers.IsPrimitiveType(symbol.Type.EdmType))
        {
          this.AddColumn(selectStatement, symbol, columnList, columnDictionary, ref separator, "X");
        }
        else
        {
          foreach (EdmProperty property in (IEnumerable<EdmProperty>) EF6MetadataHelpers.GetProperties(symbol.Type))
            this.AddColumn(selectStatement, symbol, columnList, columnDictionary, ref separator, property.Name);
        }
      }
    }

    private void AddColumn(SqlSelectStatement selectStatement, Symbol symbol, List<Symbol> columnList, Dictionary<string, Symbol> columnDictionary, ref string separator, string columnName)
    {
      this.allColumnNames[columnName] = 0;
      Symbol symbol1;
      if (!symbol.Columns.TryGetValue(columnName, out symbol1))
      {
        symbol1 = new Symbol(columnName, (TypeUsage) null);
        symbol.Columns.Add(columnName, symbol1);
      }
      selectStatement.Select.Append((object) separator);
      selectStatement.Select.Append((object) symbol);
      selectStatement.Select.Append((object) ".");
      if (symbol.OutputColumnsRenamed)
      {
        selectStatement.Select.Append((object) symbol1);
        selectStatement.OutputColumns.Add(symbol1.Name, symbol1);
      }
      else
        selectStatement.Select.Append((object) SqlGenerator.QuoteIdentifier(columnName));
      selectStatement.Select.Append((object) " AS ");
      selectStatement.Select.Append((object) symbol1);
      if (columnDictionary.ContainsKey(columnName))
      {
        columnDictionary[columnName].NeedsRenaming = true;
        symbol1.NeedsRenaming = true;
      }
      else
        columnDictionary[columnName] = symbol.Columns[columnName];
      columnList.Add(symbol1);
      separator = ", ";
    }

    private List<Symbol> AddDefaultColumns(SqlSelectStatement selectStatement)
    {
      List<Symbol> columnList = new List<Symbol>();
      Dictionary<string, Symbol> columnDictionary = new Dictionary<string, Symbol>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      string separator = "";
      if (!selectStatement.Select.IsEmpty)
        separator = ", ";
      foreach (Symbol fromExtent in selectStatement.FromExtents)
        this.AddColumns(selectStatement, fromExtent, columnList, columnDictionary, ref separator);
      return columnList;
    }

    private void AddFromSymbol(SqlSelectStatement selectStatement, string inputVarName, Symbol fromSymbol)
    {
      this.AddFromSymbol(selectStatement, inputVarName, fromSymbol, true);
    }

    private void AddFromSymbol(SqlSelectStatement selectStatement, string inputVarName, Symbol fromSymbol, bool addToSymbolTable)
    {
      if (selectStatement.FromExtents.Count == 0 || fromSymbol != selectStatement.FromExtents[0])
      {
        selectStatement.FromExtents.Add(fromSymbol);
        selectStatement.From.Append((object) " ");
        selectStatement.From.Append((object) fromSymbol);
        this.allExtentNames[fromSymbol.Name] = 0;
      }
      if (!addToSymbolTable)
        return;
      this.symbolTable.Add(inputVarName, fromSymbol);
    }

    private void AddSortKeys(SqlBuilder orderByClause, IList<DbSortClause> sortKeys)
    {
      string str = "";
      foreach (DbSortClause sortKey in (IEnumerable<DbSortClause>) sortKeys)
      {
        orderByClause.Append((object) str);
        orderByClause.Append((object) sortKey.Expression.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
        if (!string.IsNullOrEmpty(sortKey.Collation))
        {
          orderByClause.Append((object) " COLLATE ");
          orderByClause.Append((object) sortKey.Collation);
        }
        orderByClause.Append(sortKey.Ascending ? (object) " ASC" : (object) " DESC");
        str = ", ";
      }
    }

    private SqlSelectStatement CreateNewSelectStatement(SqlSelectStatement oldStatement, string inputVarName, TypeUsage inputVarType, out Symbol fromSymbol)
    {
      return this.CreateNewSelectStatement(oldStatement, inputVarName, inputVarType, true, out fromSymbol);
    }

    private SqlSelectStatement CreateNewSelectStatement(SqlSelectStatement oldStatement, string inputVarName, TypeUsage inputVarType, bool finalizeOldStatement, out Symbol fromSymbol)
    {
      fromSymbol = (Symbol) null;
      if (finalizeOldStatement && oldStatement.Select.IsEmpty)
      {
        List<Symbol> symbolList = this.AddDefaultColumns(oldStatement);
        JoinSymbol fromExtent = oldStatement.FromExtents[0] as JoinSymbol;
        if (fromExtent != null)
          fromSymbol = (Symbol) new JoinSymbol(inputVarName, inputVarType, fromExtent.ExtentList)
          {
            IsNestedJoin = true,
            ColumnList = symbolList,
            FlattenedExtentList = fromExtent.FlattenedExtentList
          };
      }
      if (fromSymbol == null)
        fromSymbol = !oldStatement.OutputColumnsRenamed ? new Symbol(inputVarName, inputVarType) : new Symbol(inputVarName, inputVarType, oldStatement.OutputColumns);
      SqlSelectStatement sqlSelectStatement = new SqlSelectStatement();
      sqlSelectStatement.From.Append((object) "( ");
      sqlSelectStatement.From.Append((object) oldStatement);
      sqlSelectStatement.From.AppendLine();
      sqlSelectStatement.From.Append((object) ") ");
      return sqlSelectStatement;
    }

    private static string EscapeSingleQuote(string s, bool isUnicode)
    {
      return (isUnicode ? "N'" : "'") + s.Replace("'", "''") + "'";
    }

    internal string GetSqlPrimitiveType(TypeUsage type)
    {
      return SqlGenerator.GetSqlPrimitiveType((DbProviderManifest) this._providerManifest, this._sqlVersion, type);
    }

    internal static string GetSqlPrimitiveType(DbProviderManifest providerManifest, EFOracleVersion sqlVersion, TypeUsage type)
    {
      TypeUsage storeType = providerManifest.GetStoreType(type);
      string str = storeType.EdmType.Name;
      int maxLength = 0;
      byte precision = 0;
      byte scale = 0;
      switch (((PrimitiveType) storeType.EdmType).PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          if (!EF6MetadataHelpers.IsFacetValueConstant(storeType, "MaxLength"))
          {
            EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
            str = str + "(" + maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")";
            break;
          }
          break;
        case PrimitiveTypeKind.Boolean:
          str = "number(1,0)";
          break;
        case PrimitiveTypeKind.DateTime:
        case PrimitiveTypeKind.Time:
          byte byteValue;
          str = !EF6MetadataHelpers.TryGetByteFacetValue(type, "Precision", out byteValue) ? "date" : ((int) byteValue <= 9 ? "timestamp" : "timestamp with local time zone");
          break;
        case PrimitiveTypeKind.Decimal:
          if (!EF6MetadataHelpers.IsFacetValueConstant(storeType, "Precision"))
          {
            EF6MetadataHelpers.TryGetPrecision(storeType, out precision);
            EF6MetadataHelpers.TryGetScale(storeType, out scale);
            str = str + "(" + (object) precision + "," + (object) scale + ")";
            break;
          }
          break;
        case PrimitiveTypeKind.Double:
          str = sqlVersion >= EFOracleVersion.Oracle10gR1 ? "binary_double" : "number";
          break;
        case PrimitiveTypeKind.Guid:
          str = "raw(16)";
          break;
        case PrimitiveTypeKind.Single:
          str = sqlVersion >= EFOracleVersion.Oracle10gR1 ? "binary_float" : "number";
          break;
        case PrimitiveTypeKind.String:
          if (!EF6MetadataHelpers.IsFacetValueConstant(storeType, "MaxLength"))
          {
            EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
            str = str + "(" + maxLength.ToString((IFormatProvider) CultureInfo.InvariantCulture) + ")";
            break;
          }
          break;
        case PrimitiveTypeKind.DateTimeOffset:
          str = "timestamp with time zone";
          break;
      }
      return str;
    }

    private ISqlFragment HandleCountExpression(DbExpression e)
    {
      ISqlFragment sqlFragment;
      if (e.ExpressionKind == DbExpressionKind.Constant)
      {
        SqlBuilder sqlBuilder = new SqlBuilder();
        sqlBuilder.Append((object) ((DbConstantExpression) e).Value.ToString());
        sqlFragment = (ISqlFragment) sqlBuilder;
      }
      else
        sqlFragment = e.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this);
      return sqlFragment;
    }

    private static bool IsApplyExpression(DbExpression e)
    {
      if (DbExpressionKind.CrossApply != e.ExpressionKind)
        return DbExpressionKind.OuterApply == e.ExpressionKind;
      return true;
    }

    private static bool IsJoinExpression(DbExpression e)
    {
      if (DbExpressionKind.CrossJoin != e.ExpressionKind && DbExpressionKind.FullOuterJoin != e.ExpressionKind && DbExpressionKind.InnerJoin != e.ExpressionKind)
        return DbExpressionKind.LeftOuterJoin == e.ExpressionKind;
      return true;
    }

    private static bool IsComplexExpression(DbExpression e)
    {
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.Constant:
        case DbExpressionKind.ParameterReference:
        case DbExpressionKind.Property:
          return false;
        default:
          return true;
      }
    }

    private static bool IsCompatible(SqlSelectStatement result, DbExpressionKind expressionKind)
    {
      switch (expressionKind)
      {
        case DbExpressionKind.Limit:
        case DbExpressionKind.Element:
          return result.Top == null;
        case DbExpressionKind.Project:
          if (result.Select.IsEmpty && result.GroupBy.IsEmpty)
            return !result.IsDistinct;
          return false;
        case DbExpressionKind.Skip:
          if (result.Select.IsEmpty && result.GroupBy.IsEmpty && result.OrderBy.IsEmpty)
            return !result.IsDistinct;
          return false;
        case DbExpressionKind.Sort:
          if (result.Select.IsEmpty && result.GroupBy.IsEmpty && result.OrderBy.IsEmpty)
            return !result.IsDistinct;
          return false;
        case DbExpressionKind.Distinct:
          if (result.Top == null)
            return result.OrderBy.IsEmpty;
          return false;
        case DbExpressionKind.Filter:
          if (result.Select.IsEmpty && result.Where.IsEmpty && result.GroupBy.IsEmpty)
            return result.Top == null;
          return false;
        case DbExpressionKind.GroupBy:
          if (result.Select.IsEmpty && result.GroupBy.IsEmpty && result.OrderBy.IsEmpty)
            return result.Top == null;
          return false;
        default:
          throw new InvalidOperationException(string.Empty);
      }
    }

    internal static string QuoteIdentifier(string name)
    {
      return "\"" + name.Replace("\"", "\"\"") + "\"";
    }

    internal static string QuoteIdentifier_storeFunctionName(string name)
    {
      if (!name.Contains("."))
        return "\"" + name + "\"";
      int num = 0;
      for (int index = name.IndexOf("."); index != -1; index = name.IndexOf(".", index + 1))
        ++num;
      if (num == 1)
        return "\"" + name.Replace(".", "\".\"") + "\"";
      int length = name.LastIndexOf(".");
      return "\"" + name.Substring(0, length) + "\".\"" + name.Substring(length + 1) + "\"";
    }

    private SqlSelectStatement VisitExpressionEnsureSqlStatement(DbExpression e)
    {
      return this.VisitExpressionEnsureSqlStatement(e, true);
    }

    private SqlSelectStatement VisitExpressionEnsureSqlStatement(DbExpression e, bool addDefaultColumns)
    {
      SqlSelectStatement selectStatement;
      switch (e.ExpressionKind)
      {
        case DbExpressionKind.Project:
        case DbExpressionKind.Sort:
        case DbExpressionKind.Filter:
        case DbExpressionKind.GroupBy:
          selectStatement = e.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this) as SqlSelectStatement;
          break;
        default:
          string inputVarName = "c";
          this.symbolTable.EnterScope();
          TypeUsage inputVarType;
          switch (e.ExpressionKind)
          {
            case DbExpressionKind.LeftOuterJoin:
            case DbExpressionKind.OuterApply:
            case DbExpressionKind.Scan:
            case DbExpressionKind.CrossApply:
            case DbExpressionKind.CrossJoin:
            case DbExpressionKind.FullOuterJoin:
            case DbExpressionKind.InnerJoin:
              inputVarType = EF6MetadataHelpers.GetElementTypeUsage(e.ResultType);
              break;
            default:
              inputVarType = EF6MetadataHelpers.GetEdmType<CollectionType>(e.ResultType).TypeUsage;
              break;
          }
          Symbol fromSymbol;
          selectStatement = this.VisitInputExpression(e, inputVarName, inputVarType, out fromSymbol);
          this.AddFromSymbol(selectStatement, inputVarName, fromSymbol);
          this.symbolTable.ExitScope();
          break;
      }
      if (addDefaultColumns && selectStatement.Select.IsEmpty)
        this.AddDefaultColumns(selectStatement);
      return selectStatement;
    }

    private SqlSelectStatement VisitFilterExpression(DbExpressionBinding input, DbExpression predicate, bool negatePredicate)
    {
      Symbol fromSymbol;
      SqlSelectStatement sqlSelectStatement = this.VisitInputExpression(input.Expression, input.VariableName, input.VariableType, out fromSymbol);
      if (!SqlGenerator.IsCompatible(sqlSelectStatement, DbExpressionKind.Filter))
        sqlSelectStatement = this.CreateNewSelectStatement(sqlSelectStatement, input.VariableName, input.VariableType, out fromSymbol);
      this.selectStatementStack.Push(sqlSelectStatement);
      this.symbolTable.EnterScope();
      this.AddFromSymbol(sqlSelectStatement, input.VariableName, fromSymbol);
      if (negatePredicate)
        sqlSelectStatement.Where.Append((object) "NOT (");
      sqlSelectStatement.Where.Append((object) predicate.Accept<ISqlFragment>((DbExpressionVisitor<ISqlFragment>) this));
      if (negatePredicate)
        sqlSelectStatement.Where.Append((object) ")");
      this.symbolTable.ExitScope();
      this.selectStatementStack.Pop();
      return sqlSelectStatement;
    }

    private static void WrapNonQueryExtent(SqlSelectStatement result, ISqlFragment sqlFragment, DbExpressionKind expressionKind)
    {
      if (expressionKind == DbExpressionKind.Function)
      {
        result.From.Append((object) sqlFragment);
      }
      else
      {
        result.From.Append((object) " (");
        result.From.Append((object) sqlFragment);
        result.From.Append((object) ")");
      }
    }

    private static bool IsBuiltInStoreFunction(EdmFunction function)
    {
      if (EF6MetadataHelpers.GetMetadataProperty<bool>((MetadataItem) function, "BuiltInAttribute"))
        return !EF6MetadataHelpers.IsCanonicalFunction(function);
      return false;
    }

    private static string ByteArrayToBinaryString(byte[] binaryArray)
    {
      StringBuilder stringBuilder = new StringBuilder(binaryArray.Length * 2);
      for (int index = 0; index < binaryArray.Length; ++index)
        stringBuilder.Append(SqlGenerator.hexDigits[((int) binaryArray[index] & 240) >> 4]).Append(SqlGenerator.hexDigits[(int) binaryArray[index] & 15]);
      return stringBuilder.ToString();
    }

    private static bool GroupByAggregatesNeedInnerQuery(IList<DbAggregate> aggregates)
    {
      foreach (DbAggregate aggregate in (IEnumerable<DbAggregate>) aggregates)
      {
        if (SqlGenerator.GroupByAggregateNeedsInnerQuery(aggregate.Arguments[0]))
          return true;
      }
      return false;
    }

    private static bool GroupByAggregateNeedsInnerQuery(DbExpression expression)
    {
      if (expression.ExpressionKind == DbExpressionKind.Constant)
        return false;
      if (expression.ExpressionKind == DbExpressionKind.Cast)
        return SqlGenerator.GroupByAggregateNeedsInnerQuery(((DbUnaryExpression) expression).Argument);
      if (expression.ExpressionKind == DbExpressionKind.Property)
        return SqlGenerator.GroupByAggregateNeedsInnerQuery(((DbPropertyExpression) expression).Instance);
      return expression.ExpressionKind != DbExpressionKind.VariableReference;
    }

    private static bool GroupByKeysNeedInnerQuery(IList<DbExpression> keys, string inputVarRefName)
    {
      foreach (DbExpression key in (IEnumerable<DbExpression>) keys)
      {
        if (SqlGenerator.GroupByKeyNeedsInnerQuery(key, inputVarRefName))
          return true;
      }
      return false;
    }

    private static bool GroupByKeyNeedsInnerQuery(DbExpression expression, string inputVarRefName)
    {
      if (expression.ExpressionKind == DbExpressionKind.Cast)
        return SqlGenerator.GroupByKeyNeedsInnerQuery(((DbUnaryExpression) expression).Argument, inputVarRefName);
      if (expression.ExpressionKind == DbExpressionKind.Property)
        return SqlGenerator.GroupByKeyNeedsInnerQuery(((DbPropertyExpression) expression).Instance, inputVarRefName);
      if (expression.ExpressionKind == DbExpressionKind.VariableReference)
        return !(expression as DbVariableReferenceExpression).VariableName.Equals(inputVarRefName);
      return true;
    }

    private void Assert10gOrNewer(PrimitiveTypeKind primitiveTypeKind)
    {
      SqlGenerator.Assert10gOrNewer(this._sqlVersion, primitiveTypeKind);
    }

    private static void Assert10gOrNewer(EFOracleVersion _sqlVersion, PrimitiveTypeKind primitiveTypeKind)
    {
      if (_sqlVersion < EFOracleVersion.Oracle10gR1)
        throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1202, primitiveTypeKind.ToString()));
    }

    private void Assert10gOrNewer(DbFunctionExpression e)
    {
      if (this.IsPre10g)
        throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1202, e.Function.Name));
    }

    private delegate ISqlFragment FunctionHandler(SqlGenerator sqlgen, DbFunctionExpression functionExpr);
  }
}
