// Decompiled with JetBrains decompiler
// Type: System.Data.Common.Utils.EF6MetadataHelpers
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System.Collections.Generic;
using System.Data.Entity.Core.Metadata.Edm;

namespace System.Data.Common.Utils
{
  internal static class EF6MetadataHelpers
  {
    internal const string MaxLengthFacetName = "MaxLength";
    internal const string UnicodeFacetName = "Unicode";
    internal const string FixedLengthFacetName = "FixedLength";
    internal const string PrecisionFacetName = "Precision";
    internal const string ScaleFacetName = "Scale";
    internal const string NullableFacetName = "Nullable";
    internal const string DefaultValueFacetName = "DefaultValue";
    internal const string TableMetadata = "Table";
    internal const string SchemaMetadata = "Schema";
    internal const string DefiningQueryMetadata = "DefiningQuery";
    internal const string CommandTextMetadata = "CommandTextAttribute";
    internal const string StoreFunctionNameMetadata = "StoreFunctionNameAttribute";
    internal const string BuiltInMetadata = "BuiltInAttribute";
    internal const string NiladicFunctionMetadata = "NiladicFunctionAttribute";
    internal const string OracleCursorParameterNameMetadata = "EFOracleProviderExtensions:CursorParameterName";
    internal const string EdmNamespaceName = "Edm";

    internal static FacetDescription GetFacet(IEnumerable<FacetDescription> facetCollection, string facetName)
    {
      foreach (FacetDescription facet in facetCollection)
      {
        if (facet.FacetName == facetName)
          return facet;
      }
      return (FacetDescription) null;
    }

    internal static bool TryGetBooleanFacetValue(TypeUsage type, string facetName, out bool boolValue)
    {
      boolValue = false;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null)
        return false;
      boolValue = (bool) facet.Value;
      return true;
    }

    internal static bool TryGetByteFacetValue(TypeUsage type, string facetName, out byte byteValue)
    {
      byteValue = (byte) 0;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null || facet.IsUnbounded)
        return false;
      byteValue = (byte) facet.Value;
      return true;
    }

    internal static bool TryGetIntFacetValue(TypeUsage type, string facetName, out int intValue)
    {
      intValue = 0;
      Facet facet;
      if (!type.Facets.TryGetValue(facetName, false, out facet) || facet.Value == null || facet.IsUnbounded)
        return false;
      intValue = (int) facet.Value;
      return true;
    }

    internal static bool TryGetIsFixedLength(TypeUsage type, out bool isFixedLength)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.String) || EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
        return EF6MetadataHelpers.TryGetBooleanFacetValue(type, "FixedLength", out isFixedLength);
      isFixedLength = false;
      return false;
    }

    internal static bool TryGetIsUnicode(TypeUsage type, out bool isUnicode)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.String))
        return EF6MetadataHelpers.TryGetBooleanFacetValue(type, "Unicode", out isUnicode);
      isUnicode = false;
      return false;
    }

    internal static bool IsFacetValueConstant(TypeUsage type, string facetName)
    {
      return EF6MetadataHelpers.GetFacet((IEnumerable<FacetDescription>) ((PrimitiveType) type.EdmType).FacetDescriptions, facetName).IsConstant;
    }

    internal static bool TryGetMaxLength(TypeUsage type, out int maxLength)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.String) || EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.Binary))
        return EF6MetadataHelpers.TryGetIntFacetValue(type, "MaxLength", out maxLength);
      maxLength = 0;
      return false;
    }

    internal static bool TryGetPrecision(TypeUsage type, out byte precision)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
        return EF6MetadataHelpers.TryGetByteFacetValue(type, "Precision", out precision);
      precision = (byte) 0;
      return false;
    }

    internal static bool TryGetScale(TypeUsage type, out byte scale)
    {
      if (EF6MetadataHelpers.IsPrimitiveType(type, PrimitiveTypeKind.Decimal))
        return EF6MetadataHelpers.TryGetByteFacetValue(type, "Scale", out scale);
      scale = (byte) 0;
      return false;
    }

    internal static bool TryGetPrimitiveTypeKind(TypeUsage type, out PrimitiveTypeKind typeKind)
    {
      if (type != null && type.EdmType != null && type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
      {
        typeKind = ((PrimitiveType) type.EdmType).PrimitiveTypeKind;
        return true;
      }
      typeKind = PrimitiveTypeKind.Binary;
      return false;
    }

    internal static PrimitiveTypeKind GetPrimitiveTypeKind(TypeUsage typeUsage)
    {
      return ((PrimitiveType) typeUsage.EdmType).PrimitiveTypeKind;
    }

    internal static bool IsPrimitiveType(EdmType type)
    {
      return BuiltInTypeKind.PrimitiveType == type.BuiltInTypeKind;
    }

    internal static bool IsPrimitiveType(TypeUsage type, PrimitiveTypeKind primitiveTypeKind)
    {
      PrimitiveTypeKind typeKind;
      if (EF6MetadataHelpers.TryGetPrimitiveTypeKind(type, out typeKind))
        return typeKind == primitiveTypeKind;
      return false;
    }

    internal static bool IsNullable(TypeUsage type)
    {
      Facet facet;
      if (type.Facets.TryGetValue("Nullable", false, out facet))
        return (bool) facet.Value;
      return true;
    }

    internal static bool IsReferenceType(GlobalItem item)
    {
      return BuiltInTypeKind.RefType == item.BuiltInTypeKind;
    }

    internal static bool IsRowType(GlobalItem item)
    {
      return BuiltInTypeKind.RowType == item.BuiltInTypeKind;
    }

    internal static bool IsCollectionType(GlobalItem item)
    {
      return BuiltInTypeKind.CollectionType == item.BuiltInTypeKind;
    }

    internal static TypeUsage GetElementTypeUsage(TypeUsage type)
    {
      if (EF6MetadataHelpers.IsCollectionType((GlobalItem) type.EdmType))
        return ((CollectionType) type.EdmType).TypeUsage;
      if (EF6MetadataHelpers.IsReferenceType((GlobalItem) type.EdmType))
        return TypeUsage.CreateDefaultTypeUsage((EdmType) ((RefType) type.EdmType).ElementType);
      return (TypeUsage) null;
    }

    internal static TEdmType GetEdmType<TEdmType>(TypeUsage typeUsage) where TEdmType : EdmType
    {
      return (TEdmType) typeUsage.EdmType;
    }

    internal static bool IsCanonicalFunction(EdmFunction function)
    {
      return function.NamespaceName.Equals("Edm", StringComparison.InvariantCulture);
    }

    internal static IList<EdmProperty> GetProperties(TypeUsage typeUsage)
    {
      return EF6MetadataHelpers.GetProperties(typeUsage.EdmType);
    }

    internal static IList<EdmProperty> GetProperties(EdmType edmType)
    {
      switch (edmType.BuiltInTypeKind)
      {
        case BuiltInTypeKind.ComplexType:
          return (IList<EdmProperty>) ((ComplexType) edmType).Properties;
        case BuiltInTypeKind.EntityType:
          return (IList<EdmProperty>) ((EntityType) edmType).Properties;
        case BuiltInTypeKind.RowType:
          return (IList<EdmProperty>) ((RowType) edmType).Properties;
        default:
          return (IList<EdmProperty>) new List<EdmProperty>();
      }
    }

    internal static TypeUsage CopyTypeUsageAndSetUnicodeFacetToFalse(TypeUsage typeUsage)
    {
      bool isFixedLength = false;
      int maxLength = 0;
      EF6MetadataHelpers.TryGetIsFixedLength(typeUsage, out isFixedLength);
      EF6MetadataHelpers.TryGetMaxLength(typeUsage, out maxLength);
      if (maxLength > 0)
        return TypeUsage.CreateStringTypeUsage((PrimitiveType) typeUsage.EdmType, false, isFixedLength, maxLength);
      return TypeUsage.CreateStringTypeUsage((PrimitiveType) typeUsage.EdmType, false, isFixedLength);
    }

    internal static T GetMetadataProperty<T>(MetadataItem item, string propertyName)
    {
      MetadataProperty metadataProperty;
      if (!item.MetadataProperties.TryGetValue(propertyName, true, out metadataProperty) || !(metadataProperty.Value is T))
        return default (T);
      return (T) metadataProperty.Value;
    }

    internal static ParameterDirection ParameterModeToParameterDirection(ParameterMode mode)
    {
      switch (mode)
      {
        case ParameterMode.In:
          return ParameterDirection.Input;
        case ParameterMode.Out:
          return ParameterDirection.Output;
        case ParameterMode.InOut:
          return ParameterDirection.InputOutput;
        case ParameterMode.ReturnValue:
          return ParameterDirection.ReturnValue;
        default:
          return (ParameterDirection) 0;
      }
    }
  }
}
