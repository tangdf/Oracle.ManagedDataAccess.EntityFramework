// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.DmlSqlGenerator
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common.Utils;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Globalization;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal static class DmlSqlGenerator
  {
    private const int s_commandTextBuilderInitialCapacity = 256;

    internal static string GenerateUpdateSql(DbUpdateCommandTree tree, EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion, out List<OracleParameter> parameters)
    {
      StringBuilder commandText = new StringBuilder(256);
      DmlSqlGenerator.ExpressionTranslator translator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, null != tree.Returning, sqlVersion);
      int count = tree.SetClauses.Count;
      commandText.Append("update ");
      tree.Target.Expression.Accept((DbExpressionVisitor) translator);
      commandText.AppendLine();
      bool flag = true;
      commandText.Append("set ");
      foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
      {
        if (flag)
          flag = false;
        else
          commandText.Append(", ");
        setClause.Property.Accept((DbExpressionVisitor) translator);
        commandText.Append(" = ");
        setClause.Value.Accept((DbExpressionVisitor) translator);
      }
      if (flag)
        commandText.Append("[place_holder] ");
      commandText.AppendLine();
      commandText.Append("where ");
      tree.Predicate.Accept((DbExpressionVisitor) translator);
      if (flag)
      {
        string str = commandText.ToString();
        int num = str.IndexOf("where ");
        string newValue = str.Substring(num + "where ".Length).Replace("(", "").Replace(")", "").Replace(" and ", " ,");
        commandText.Replace("[place_holder]", newValue);
      }
      commandText.AppendLine();
      DmlSqlGenerator.GenerateReturningSql(commandText, (DbModificationCommandTree) tree, translator, tree.Returning, providerManifest, sqlVersion, true);
      parameters = translator.Parameters;
      return commandText.ToString();
    }

    internal static string GenerateDeleteSql(DbDeleteCommandTree tree, EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion, out List<OracleParameter> parameters)
    {
      StringBuilder commandText = new StringBuilder(256);
      DmlSqlGenerator.ExpressionTranslator expressionTranslator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, false, sqlVersion);
      commandText.Append("delete ");
      tree.Target.Expression.Accept((DbExpressionVisitor) expressionTranslator);
      commandText.AppendLine();
      commandText.Append("where ");
      tree.Predicate.Accept((DbExpressionVisitor) expressionTranslator);
      parameters = expressionTranslator.Parameters;
      return commandText.ToString();
    }

    internal static string GenerateInsertSql(DbInsertCommandTree tree, EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion, out List<OracleParameter> parameters)
    {
      StringBuilder commandText = new StringBuilder(256);
      DmlSqlGenerator.ExpressionTranslator translator = new DmlSqlGenerator.ExpressionTranslator(commandText, (DbModificationCommandTree) tree, null != tree.Returning, sqlVersion);
      commandText.Append("insert into ");
      tree.Target.Expression.Accept((DbExpressionVisitor) translator);
      if (0 < tree.SetClauses.Count)
      {
        commandText.Append("(");
        bool flag1 = true;
        foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
        {
          if (flag1)
            flag1 = false;
          else
            commandText.Append(", ");
          setClause.Property.Accept((DbExpressionVisitor) translator);
        }
        commandText.AppendLine(")");
        bool flag2 = true;
        commandText.Append("values (");
        foreach (DbSetClause setClause in (IEnumerable<DbModificationClause>) tree.SetClauses)
        {
          if (flag2)
            flag2 = false;
          else
            commandText.Append(", ");
          setClause.Value.Accept((DbExpressionVisitor) translator);
          translator.RegisterMemberValue(setClause.Property, setClause.Value);
        }
        commandText.AppendLine(")");
      }
      else
        commandText.AppendLine().AppendLine(" values (default)");
      DmlSqlGenerator.GenerateReturningSql(commandText, (DbModificationCommandTree) tree, translator, tree.Returning, providerManifest, sqlVersion, false);
      parameters = translator.Parameters;
      return commandText.ToString();
    }

    private static string GenerateMemberTSql(EdmMember member)
    {
      return SqlGenerator.QuoteIdentifier(member.Name);
    }

    private static void GenerateReturningSql(StringBuilder commandText, DbModificationCommandTree tree, DmlSqlGenerator.ExpressionTranslator translator, DbExpression returning, EFOracleProviderManifest providerManifest, EFOracleVersion sqlVersion, bool isUpdate)
    {
      if (returning == null)
        return;
      EntitySetBase target = ((DbScanExpression) tree.Target.Expression).Target;
      StringBuilder stringBuilder = new StringBuilder(50);
      stringBuilder.Append("declare\n");
      Dictionary<EdmMember, string> dictionary = new Dictionary<EdmMember, string>();
      foreach (EdmMember member in target.ElementType.Members)
      {
        ReadOnlyMetadataCollection<Facet> facets = ((TypeUsage) member.MetadataProperties["TypeUsage"].Value).Facets;
        string empty = string.Empty;
        if (facets.Contains("StoreGeneratedPattern"))
        {
          string str = facets["StoreGeneratedPattern"].Value.ToString();
          if (!string.IsNullOrEmpty(str))
          {
            if (isUpdate && str.ToUpperInvariant() == "COMPUTED")
              dictionary[member] = str;
            else if (!isUpdate && (str.ToUpperInvariant() == "COMPUTED" || str.ToUpperInvariant() == "IDENTITY"))
              dictionary[member] = str;
          }
        }
        if (dictionary.ContainsKey(member))
        {
          stringBuilder.Append(DmlSqlGenerator.GenerateMemberTSql(member));
          stringBuilder.Append(" ");
          stringBuilder.Append(SqlGenerator.GetSqlPrimitiveType((DbProviderManifest) providerManifest, sqlVersion, member.TypeUsage));
          stringBuilder.Append(";\n");
        }
      }
      stringBuilder.Append("begin\n");
      commandText.Insert(0, stringBuilder.ToString());
      OracleParameter parameter = translator.CreateParameter(OracleDbType.RefCursor, ParameterDirection.Output);
      commandText.Append("returning\n");
      string str1 = string.Empty;
      foreach (EdmMember member in target.ElementType.Members)
      {
        if (dictionary.ContainsKey(member))
        {
          commandText.Append(str1);
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(member));
          str1 = ", ";
        }
      }
      commandText.Append(" into\n");
      string str2 = string.Empty;
      foreach (EdmMember member in target.ElementType.Members)
      {
        if (dictionary.ContainsKey(member))
        {
          commandText.Append(str2);
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(member));
          str2 = ", ";
        }
      }
      commandText.Append(";\n");
      commandText.Append("open ");
      commandText.Append(parameter.ParameterName);
      commandText.Append(" for select\n");
      string str3 = string.Empty;
      foreach (EdmMember member in target.ElementType.Members)
      {
        if (dictionary.ContainsKey(member))
        {
          commandText.Append(str3);
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(member));
          commandText.Append(" as ");
          commandText.Append(DmlSqlGenerator.GenerateMemberTSql(member));
          str3 = ", ";
        }
      }
      commandText.Append(" from dual;\n");
      commandText.Append("end;");
    }

    private static bool IsValidIdentityColumnType(TypeUsage typeUsage)
    {
      if (typeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.PrimitiveType)
        return false;
      string name = typeUsage.EdmType.Name;
      if (name == "tinyint" || name == "smallint" || (name == "int" || name == "bigint"))
        return true;
      Facet facet;
      if ((name == "decimal" || name == "numeric") && typeUsage.Facets.TryGetValue("Scale", false, out facet))
        return Convert.ToInt32(facet.Value, (IFormatProvider) CultureInfo.InvariantCulture) == 0;
      return false;
    }

    private class ExpressionTranslator : System.Data.Entity.Core.Common.CommandTrees.BasicExpressionVisitor
    {
      private readonly StringBuilder _commandText;
      private readonly DbModificationCommandTree _commandTree;
      private readonly List<OracleParameter> _parameters;
      private readonly Dictionary<EdmMember, OracleParameter> _memberValues;
      private readonly EFOracleVersion _version;
      private int parameterNameCount;

      internal ExpressionTranslator(StringBuilder commandText, DbModificationCommandTree commandTree, bool preserveMemberValues, EFOracleVersion version)
      {
        this._commandText = commandText;
        this._commandTree = commandTree;
        this._version = version;
        this._parameters = new List<OracleParameter>();
        this._memberValues = preserveMemberValues ? new Dictionary<EdmMember, OracleParameter>() : (Dictionary<EdmMember, OracleParameter>) null;
      }

      internal List<OracleParameter> Parameters
      {
        get
        {
          return this._parameters;
        }
      }

      internal Dictionary<EdmMember, OracleParameter> MemberValues
      {
        get
        {
          return this._memberValues;
        }
      }

      internal OracleParameter CreateParameter(OracleDbType oracleType, ParameterDirection direction)
      {
        OracleParameter oracleParameter = new OracleParameter();
        oracleParameter.ParameterName = this.NextName();
        oracleParameter.OracleDbType = oracleType;
        oracleParameter.Direction = direction;
        this._parameters.Add(oracleParameter);
        return oracleParameter;
      }

      internal OracleParameter CreateParameter(object value, TypeUsage type)
      {
        OracleParameter oracleParameter = EFOracleProviderServices.CreateOracleParameter(this.NextName(), type, ParameterMode.In, value, this._version);
        this._parameters.Add(oracleParameter);
        return oracleParameter;
      }

      private string NextName()
      {
        string str = ":p" + this.parameterNameCount.ToString((IFormatProvider) CultureInfo.InvariantCulture);
        ++this.parameterNameCount;
        return str;
      }

      public override void Visit(DbAndExpression expression)
      {
        this.VisitBinary((DbBinaryExpression) expression, " and ");
      }

      public override void Visit(DbOrExpression expression)
      {
        this.VisitBinary((DbBinaryExpression) expression, " or ");
      }

      public override void Visit(DbComparisonExpression expression)
      {
        this.VisitBinary((DbBinaryExpression) expression, " = ");
        this.RegisterMemberValue(expression.Left, expression.Right);
      }

      internal void RegisterMemberValue(DbExpression propertyExpression, DbExpression value)
      {
        if (this._memberValues == null)
          return;
        EdmMember property = ((DbPropertyExpression) propertyExpression).Property;
        if (value.ExpressionKind == DbExpressionKind.Null)
          return;
        this._memberValues[property] = this._parameters[this._parameters.Count - 1];
      }

      public override void Visit(DbIsNullExpression expression)
      {
        expression.Argument.Accept((DbExpressionVisitor) this);
        this._commandText.Append(" is null");
      }

      public override void Visit(DbNotExpression expression)
      {
        this._commandText.Append("not (");
        expression.Accept((DbExpressionVisitor) this);
        this._commandText.Append(")");
      }

      public override void Visit(DbConstantExpression expression)
      {
        this._commandText.Append(this.CreateParameter(expression.Value, expression.ResultType).ParameterName);
      }

      public override void Visit(DbScanExpression expression)
      {
        if (EF6MetadataHelpers.GetMetadataProperty<string>((MetadataItem) expression.Target, "DefiningQuery") != null)
          throw new InvalidOperationException(EFProviderSettings.Instance.GetErrorMessage(-5001));
        this._commandText.Append(SqlGenerator.GetTargetTSql(expression.Target));
      }

      public override void Visit(DbPropertyExpression expression)
      {
        this._commandText.Append(DmlSqlGenerator.GenerateMemberTSql(expression.Property));
      }

      public override void Visit(DbNullExpression expression)
      {
        this._commandText.Append("null");
      }

      public override void Visit(DbNewInstanceExpression expression)
      {
        bool flag = true;
        foreach (DbExpression dbExpression in (IEnumerable<DbExpression>) expression.Arguments)
        {
          if (flag)
            flag = false;
          else
            this._commandText.Append(", ");
          dbExpression.Accept((DbExpressionVisitor) this);
        }
      }

      private void VisitBinary(DbBinaryExpression expression, string separator)
      {
        this._commandText.Append("(");
        expression.Left.Accept((DbExpressionVisitor) this);
        this._commandText.Append(separator);
        expression.Right.Accept((DbExpressionVisitor) this);
        this._commandText.Append(")");
      }
    }
  }
}
