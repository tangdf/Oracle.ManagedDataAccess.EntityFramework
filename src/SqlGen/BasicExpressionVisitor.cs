// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.SqlGen.BasicExpressionVisitor
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;

namespace Oracle.ManagedDataAccess.EntityFramework.SqlGen
{
  internal abstract class BasicExpressionVisitor : DbExpressionVisitor
  {
    protected virtual void VisitUnaryExpression(DbUnaryExpression expression)
    {
      this.VisitExpression(EntityUtils.CheckArgumentNull<DbUnaryExpression>(expression, nameof (expression)).Argument);
    }

    protected virtual void VisitBinaryExpression(DbBinaryExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbBinaryExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Left);
      this.VisitExpression(expression.Right);
    }

    protected virtual void VisitExpressionBindingPre(DbExpressionBinding binding)
    {
      EntityUtils.CheckArgumentNull<DbExpressionBinding>(binding, nameof (binding));
      this.VisitExpression(binding.Expression);
    }

    protected virtual void VisitExpressionBindingPost(DbExpressionBinding binding)
    {
    }

    protected virtual void VisitGroupExpressionBindingPre(DbGroupExpressionBinding binding)
    {
      EntityUtils.CheckArgumentNull<DbGroupExpressionBinding>(binding, nameof (binding));
      this.VisitExpression(binding.Expression);
    }

    protected virtual void VisitGroupExpressionBindingMid(DbGroupExpressionBinding binding)
    {
    }

    protected virtual void VisitGroupExpressionBindingPost(DbGroupExpressionBinding binding)
    {
    }

    protected virtual void VisitLambdaFunctionPre(EdmFunction function, DbExpression body)
    {
      EntityUtils.CheckArgumentNull<EdmFunction>(function, nameof (function));
      EntityUtils.CheckArgumentNull<DbExpression>(body, nameof (body));
    }

    protected virtual void VisitLambdaFunctionPost(EdmFunction function, DbExpression body)
    {
    }

    public virtual void VisitExpression(DbExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbExpression>(expression, nameof (expression)).Accept((DbExpressionVisitor) this);
    }

    public virtual void VisitExpressionList(IList<DbExpression> expressionList)
    {
      EntityUtils.CheckArgumentNull<IList<DbExpression>>(expressionList, nameof (expressionList));
      for (int index = 0; index < expressionList.Count; ++index)
        this.VisitExpression(expressionList[index]);
    }

    public virtual void VisitAggregateList(IList<DbAggregate> aggregates)
    {
      EntityUtils.CheckArgumentNull<IList<DbAggregate>>(aggregates, nameof (aggregates));
      for (int index = 0; index < aggregates.Count; ++index)
        this.VisitAggregate(aggregates[index]);
    }

    public virtual void VisitAggregate(DbAggregate aggregate)
    {
      this.VisitExpressionList(EntityUtils.CheckArgumentNull<DbAggregate>(aggregate, nameof (aggregate)).Arguments);
    }

    public override void Visit(DbExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbExpression>(expression, nameof (expression));
      throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", expression.GetType().FullName));
    }

    public override void Visit(DbConstantExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbConstantExpression>(expression, nameof (expression));
    }

    public override void Visit(DbNullExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbNullExpression>(expression, nameof (expression));
    }

    public override void Visit(DbVariableReferenceExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbVariableReferenceExpression>(expression, nameof (expression));
    }

    public override void Visit(DbParameterReferenceExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbParameterReferenceExpression>(expression, nameof (expression));
    }

    public override void Visit(DbFunctionExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbFunctionExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.Arguments);
    }

    public override void Visit(DbPropertyExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbPropertyExpression>(expression, nameof (expression));
      if (expression.Instance == null)
        return;
      this.VisitExpression(expression.Instance);
    }

    public override void Visit(DbComparisonExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbLikeExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbLikeExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Argument);
      this.VisitExpression(expression.Pattern);
      this.VisitExpression(expression.Escape);
    }

    public override void Visit(DbLimitExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbLimitExpression>(expression, nameof (expression));
      this.VisitExpression(expression.Argument);
      this.VisitExpression(expression.Limit);
    }

    public override void Visit(DbIsNullExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbArithmeticExpression expression)
    {
      this.VisitExpressionList(EntityUtils.CheckArgumentNull<DbArithmeticExpression>(expression, nameof (expression)).Arguments);
    }

    public override void Visit(DbAndExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbOrExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbNotExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbDistinctExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbElementExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbIsEmptyExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbUnionAllExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbIntersectExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbExceptExpression expression)
    {
      this.VisitBinaryExpression((DbBinaryExpression) expression);
    }

    public override void Visit(DbOfTypeExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbTreatExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbCastExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbIsOfExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbCaseExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbCaseExpression>(expression, nameof (expression));
      this.VisitExpressionList(expression.When);
      this.VisitExpressionList(expression.Then);
      this.VisitExpression(expression.Else);
    }

    public override void Visit(DbRefExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbRelationshipNavigationExpression expression)
    {
      this.VisitExpression(EntityUtils.CheckArgumentNull<DbRelationshipNavigationExpression>(expression, nameof (expression)).NavigationSource);
    }

    public override void Visit(DbDerefExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbRefKeyExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbEntityRefExpression expression)
    {
      this.VisitUnaryExpression((DbUnaryExpression) expression);
    }

    public override void Visit(DbScanExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbScanExpression>(expression, nameof (expression));
    }

    public override void Visit(DbFilterExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbFilterExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Predicate);
      this.VisitExpressionBindingPost(expression.Input);
    }

    public override void Visit(DbProjectExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbProjectExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Projection);
      this.VisitExpressionBindingPost(expression.Input);
    }

    public override void Visit(DbCrossJoinExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbCrossJoinExpression>(expression, nameof (expression));
      foreach (DbExpressionBinding input in (IEnumerable<DbExpressionBinding>) expression.Inputs)
        this.VisitExpressionBindingPre(input);
      foreach (DbExpressionBinding input in (IEnumerable<DbExpressionBinding>) expression.Inputs)
        this.VisitExpressionBindingPost(input);
    }

    public override void Visit(DbJoinExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbJoinExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Left);
      this.VisitExpressionBindingPre(expression.Right);
      this.VisitExpression(expression.JoinCondition);
      this.VisitExpressionBindingPost(expression.Left);
      this.VisitExpressionBindingPost(expression.Right);
    }

    public override void Visit(DbApplyExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbApplyExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      if (expression.Apply != null)
        this.VisitExpression(expression.Apply.Expression);
      this.VisitExpressionBindingPost(expression.Input);
    }

    public override void Visit(DbGroupByExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbGroupByExpression>(expression, nameof (expression));
      this.VisitGroupExpressionBindingPre(expression.Input);
      this.VisitExpressionList(expression.Keys);
      this.VisitGroupExpressionBindingMid(expression.Input);
      this.VisitAggregateList(expression.Aggregates);
      this.VisitGroupExpressionBindingPost(expression.Input);
    }

    public override void Visit(DbSkipExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbSkipExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      foreach (DbSortClause dbSortClause in (IEnumerable<DbSortClause>) expression.SortOrder)
        this.VisitExpression(dbSortClause.Expression);
      this.VisitExpressionBindingPost(expression.Input);
      this.VisitExpression(expression.Count);
    }

    public override void Visit(DbSortExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbSortExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      for (int index = 0; index < expression.SortOrder.Count; ++index)
        this.VisitExpression(expression.SortOrder[index].Expression);
      this.VisitExpressionBindingPost(expression.Input);
    }

    public override void Visit(DbQuantifierExpression expression)
    {
      EntityUtils.CheckArgumentNull<DbQuantifierExpression>(expression, nameof (expression));
      this.VisitExpressionBindingPre(expression.Input);
      this.VisitExpression(expression.Predicate);
      this.VisitExpressionBindingPost(expression.Input);
    }
  }
}
