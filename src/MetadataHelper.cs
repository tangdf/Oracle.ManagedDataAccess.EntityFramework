// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.MetadataHelper
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using System.Data.Entity.Core.Metadata.Edm;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal static class MetadataHelper
  {
    internal static byte GetPrecision(this TypeUsage type)
    {
      return type.GetFacetValue<byte>("Precision");
    }

    internal static byte GetScale(this TypeUsage type)
    {
      return type.GetFacetValue<byte>("Scale");
    }

    internal static int GetMaxLength(this TypeUsage type)
    {
      return type.GetFacetValue<int>("MaxLength");
    }

    internal static T GetFacetValue<T>(this TypeUsage type, string facetName)
    {
      return (T) type.Facets[facetName].Value;
    }
  }
}
