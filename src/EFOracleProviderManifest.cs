// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EFOracleProviderManifest
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using OracleInternal.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common.Utils;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Metadata.Edm;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal class EFOracleProviderManifest : DbXmlEnabledProviderManifest
  {
    internal static bool m_bMapNumberToBoolean = false;
    internal static bool m_bMapNumberToByte = false;
    internal static int m_edmMappingMaxBOOL = 1;
    internal static int m_edmMappingMaxBYTE = 3;
    internal static int m_edmMappingMaxINT16 = 5;
    internal static int m_edmMappingMaxINT32 = 10;
    internal static int m_edmMappingMaxINT64 = 19;
    internal static Dictionary<DbType, PrimitiveTypeKind> s_DbTypeToPrimitiveTypeKind = new Dictionary<DbType, PrimitiveTypeKind>()
    {
      {
        DbType.Boolean,
        PrimitiveTypeKind.Boolean
      },
      {
        DbType.Byte,
        PrimitiveTypeKind.Byte
      },
      {
        DbType.Int16,
        PrimitiveTypeKind.Int16
      },
      {
        DbType.Int32,
        PrimitiveTypeKind.Int32
      },
      {
        DbType.Int64,
        PrimitiveTypeKind.Int64
      },
      {
        DbType.Decimal,
        PrimitiveTypeKind.Decimal
      }
    };
    private EFOracleVersion _version = EFOracleVersion.Oracle11gR2;
    private string _token = "11.2";
    private int Nvarchar2MaxSize_12c = (int) short.MaxValue;
    private int Varchar2MaxSize_12c = (int) short.MaxValue;
    private int BinaryMaxSize_12c = (int) short.MaxValue;
    internal const string TokenOracle9iR2 = "9.2";
    internal const string TokenOracle10gR1 = "10.1";
    internal const string TokenOracle10gR2 = "10.2";
    internal const string TokenOracle11gR1 = "11.1";
    internal const string TokenOracle11gR2 = "11.2";
    internal const string TokenOracle12cR1 = "12.1";
    internal const string TokenOracle12cR2 = "12.2";
    internal const char LikeEscapeChar = '\\';
    internal const string LikeEscapeCharToString = "\\";
    private const int BinaryMaxSize = 2000;
    private const int Nvarchar2MaxSize = 4000;
    private const int NcharMaxSize = 2000;
    private const int CharMaxSize = 2000;
    private const int Varchar2MaxSize = 4000;
    private ReadOnlyCollection<PrimitiveType> _primitiveTypes;
    private ReadOnlyCollection<EdmFunction> _functions;

    public EFOracleProviderManifest(string manifestToken)
      : base(EFOracleProviderManifest.GetProviderManifest())
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderManifest::EFOracleProviderManifest()\n");
      this._version = EFOracleVersionUtils.GetStorageVersion(manifestToken);
      this._token = manifestToken;
      ODTSettings.FireEdmInUseEvent();
      EFOracleProviderManifest.m_bMapNumberToBoolean = false;
      int maxPrecision1;
      if ((maxPrecision1 = EFProviderSettings.Instance.GetMaxPrecision("BOOL", true)) > 0)
      {
        EFOracleProviderManifest.m_edmMappingMaxBOOL = maxPrecision1;
        EFOracleProviderManifest.m_bMapNumberToBoolean = true;
      }
      else
        EFOracleProviderManifest.m_edmMappingMaxBOOL = 1;
      EFOracleProviderManifest.m_bMapNumberToByte = false;
      int maxPrecision2;
      if ((maxPrecision2 = EFProviderSettings.Instance.GetMaxPrecision("BYTE", true)) > 0)
      {
        EFOracleProviderManifest.m_edmMappingMaxBYTE = maxPrecision2;
        EFOracleProviderManifest.m_bMapNumberToByte = true;
      }
      else
        EFOracleProviderManifest.m_edmMappingMaxBYTE = 3;
      int maxPrecision3;
      EFOracleProviderManifest.m_edmMappingMaxINT16 = (maxPrecision3 = EFProviderSettings.Instance.GetMaxPrecision("INT16", true)) <= 0 ? 5 : maxPrecision3;
      int maxPrecision4;
      EFOracleProviderManifest.m_edmMappingMaxINT32 = (maxPrecision4 = EFProviderSettings.Instance.GetMaxPrecision("INT32", true)) <= 0 ? 10 : maxPrecision4;
      int maxPrecision5;
      EFOracleProviderManifest.m_edmMappingMaxINT64 = (maxPrecision5 = EFProviderSettings.Instance.GetMaxPrecision("INT64", true)) <= 0 ? 19 : maxPrecision5;
      if (!EFProviderSettings.s_tracingEnabled)
        return;
      EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT)  EFOracleProviderManifest::EFOracleProviderManifest()\n");
    }

    internal string Token
    {
      get
      {
        return this._token;
      }
    }

    private static XmlReader GetProviderManifest()
    {
      if (EFOracleVersionUtils.GetStorageVersion(EFOracleProviderServices.versionHint_static) >= EFOracleVersion.Oracle12cR1)
        return EFOracleProviderManifest.GetXmlResource("Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleProviderManifest_12c_or_later.xml");
      return EFOracleProviderManifest.GetXmlResource("Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleProviderManifest.xml");
    }

    internal static string EscapeLikeText(string text, bool alwaysEscapeEscapeChar, out bool usedEscapeChar)
    {
      usedEscapeChar = false;
      if (!text.Contains("%") && !text.Contains("_") && (!alwaysEscapeEscapeChar || !text.Contains("\\")))
        return text;
      StringBuilder stringBuilder = new StringBuilder(text.Length);
      foreach (char ch in text)
      {
        switch (ch)
        {
          case '%':
          case '_':
          case '\\':
            stringBuilder.Append('\\');
            usedEscapeChar = true;
            break;
        }
        stringBuilder.Append(ch);
      }
      return stringBuilder.ToString();
    }

    protected override XmlReader GetDbInformation(string informationType)
    {
      if (informationType == "StoreSchemaDefinition" || informationType == "StoreSchemaDefinitionVersion3")
        return this.GetStoreSchemaDescription(informationType);
      if (informationType == "StoreSchemaMapping" || informationType == "StoreSchemaMappingVersion3")
        return this.GetStoreSchemaMapping(informationType);
      if (informationType == "ConceptualSchemaDefinition" || informationType == "ConceptualSchemaDefinitionVersion3")
        return (XmlReader) null;
      throw new ProviderIncompatibleException(EFProviderSettings.Instance.GetErrorMessage(-1202, informationType));
    }

    public override ReadOnlyCollection<PrimitiveType> GetStoreTypes()
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderManifest::GetStoreTypes()\n");
      if (this._primitiveTypes == null && EFOracleVersionUtils.IsVersionX(this._version))
      {
        if (this._version < EFOracleVersion.Oracle10gR1)
        {
          List<PrimitiveType> primitiveTypeList = new List<PrimitiveType>((IEnumerable<PrimitiveType>) base.GetStoreTypes());
          primitiveTypeList.RemoveAll((Predicate<PrimitiveType>) (primitiveType =>
          {
            string lowerInvariant = primitiveType.Name.ToLowerInvariant();
            if (!lowerInvariant.Equals("binary_float", StringComparison.Ordinal))
              return lowerInvariant.Equals("binary_double", StringComparison.Ordinal);
            return true;
          }));
          this._primitiveTypes = primitiveTypeList.AsReadOnly();
        }
        else
          this._primitiveTypes = base.GetStoreTypes();
      }
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT)  EFOracleProviderManifest::GetStoreTypes()\n");
      return this._primitiveTypes;
    }

    public override ReadOnlyCollection<EdmFunction> GetStoreFunctions()
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderManifest::GetStoreFunctions()\n");
      if (this._functions == null && EFOracleVersionUtils.IsVersionX(this._version))
        this._functions = base.GetStoreFunctions();
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderManifest::GetStoreFunctions()\n");
      return this._functions;
    }

    public override TypeUsage GetEdmType(TypeUsage storeType)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderManifest::GetEdmType()\n");
      EntityUtils.CheckArgumentNull<TypeUsage>(storeType, nameof (storeType));
      string lowerInvariant = storeType.EdmType.Name.ToLowerInvariant();
      if (!this.StoreTypeNameToEdmPrimitiveType.ContainsKey(lowerInvariant))
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", lowerInvariant));
      PrimitiveType primitiveType = this.StoreTypeNameToEdmPrimitiveType[lowerInvariant];
      int maxLength = 0;
      bool isUnicode = true;
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderManifest::GetEdmType()\n");
      PrimitiveTypeKind primitiveTypeKind1;
      bool flag;
      bool isFixedLength;
      switch (lowerInvariant)
      {
        case "int":
        case "smallint":
        case "binary_integer":
        case "pl/sql boolean":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) primitiveType);
        case "mlslabel":
          return TypeUsage.CreateBinaryTypeUsage(primitiveType, true, 12345);
        case "varchar2":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isUnicode = false;
          isFixedLength = false;
          break;
        case "char":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isUnicode = false;
          isFixedLength = true;
          break;
        case "nvarchar2":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isUnicode = true;
          isFixedLength = false;
          break;
        case "nchar":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isUnicode = true;
          isFixedLength = true;
          break;
        case "clob":
        case "long":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = true;
          isUnicode = false;
          isFixedLength = false;
          break;
        case "xmltype":
        case "nclob":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = true;
          isUnicode = true;
          isFixedLength = false;
          break;
        case "blob":
        case "bfile":
          primitiveTypeKind1 = PrimitiveTypeKind.Binary;
          flag = true;
          isFixedLength = false;
          break;
        case "raw":
          primitiveTypeKind1 = PrimitiveTypeKind.Binary;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isFixedLength = false;
          if (maxLength == 16)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Guid));
          break;
        case "long raw":
          primitiveTypeKind1 = PrimitiveTypeKind.Binary;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isFixedLength = false;
          break;
        case "guid raw":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Guid));
        case "guid":
        case "binary_float":
        case "binary_double":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) primitiveType);
        case "rowid":
        case "urowid":
          primitiveTypeKind1 = PrimitiveTypeKind.String;
          flag = !EF6MetadataHelpers.TryGetMaxLength(storeType, out maxLength);
          isUnicode = false;
          isFixedLength = false;
          break;
        case "float":
          byte precision1;
          byte scale1;
          if (!EF6MetadataHelpers.TryGetPrecision(storeType, out precision1) || !EF6MetadataHelpers.TryGetScale(storeType, out scale1))
            return TypeUsage.CreateDecimalTypeUsage(primitiveType);
          byte precision2 = byte.Parse(((int) ((double) Convert.ToInt32(precision1) * 0.30103 + 1.0)).ToString());
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, precision2, scale1);
        case "odp_internal_use_type":
          return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Boolean));
        case "number":
          byte precision3;
          byte scale2;
          if (!EF6MetadataHelpers.TryGetPrecision(storeType, out precision3) || !EF6MetadataHelpers.TryGetScale(storeType, out scale2))
            return TypeUsage.CreateDecimalTypeUsage(primitiveType);
          if (!ConfigBaseClass.s_bLegacyEdmMappingPresent && (int) scale2 == 0)
          {
            if ((int) precision3 < 0 || (int) precision3 > 38)
              precision3 = (byte) 0;
            DbType key = ConfigBaseClass.s_edmPrecisonMapping[(int) precision3];
            if (EFOracleProviderManifest.s_DbTypeToPrimitiveTypeKind.ContainsKey(key))
            {
              PrimitiveTypeKind primitiveTypeKind2;
              EFOracleProviderManifest.s_DbTypeToPrimitiveTypeKind.TryGetValue(key, out primitiveTypeKind2);
              if (primitiveTypeKind2 == PrimitiveTypeKind.Boolean || primitiveTypeKind2 == PrimitiveTypeKind.Byte || (primitiveTypeKind2 == PrimitiveTypeKind.Int16 || primitiveTypeKind2 == PrimitiveTypeKind.Int32) || primitiveTypeKind2 == PrimitiveTypeKind.Int64)
                return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(primitiveTypeKind2));
              if (primitiveTypeKind2 == PrimitiveTypeKind.Decimal)
                return TypeUsage.CreateDecimalTypeUsage(primitiveType, precision3, scale2);
            }
          }
          if ((int) precision3 == 1 && (int) scale2 == 0)
          {
            if (EFOracleProviderManifest.m_bMapNumberToBoolean && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxBOOL)
              return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Boolean));
            if (EFOracleProviderManifest.m_bMapNumberToByte && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxBYTE)
              return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Byte));
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int16));
          }
          if (EFOracleProviderManifest.m_bMapNumberToByte && (int) scale2 == 0 && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxBYTE)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Byte));
          if ((int) scale2 == 0 && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxINT16)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int16));
          if ((int) scale2 == 0 && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxINT32)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int32));
          if ((int) scale2 == 0 && (int) precision3 <= (int) (byte) EFOracleProviderManifest.m_edmMappingMaxINT64)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.Int64));
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, precision3, scale2);
        case "date":
          return TypeUsage.CreateDateTimeTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTime), new byte?());
        case "timestamp":
          byte byteValue;
          if (EF6MetadataHelpers.TryGetByteFacetValue(storeType, "Precision", out byteValue))
            return TypeUsage.CreateDateTimeTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTime), new byte?(byteValue));
          return TypeUsage.CreateDateTimeTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTime), new byte?((byte) 9));
        case "timestamp with time zone":
          return TypeUsage.CreateDateTimeOffsetTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTimeOffset), new byte?((byte) 9));
        case "timestamp with local time zone":
          return TypeUsage.CreateDateTimeTypeUsage(PrimitiveType.GetEdmPrimitiveType(PrimitiveTypeKind.DateTime), new byte?(byte.MaxValue));
        case "interval year to month":
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, (byte) 250, (byte) 0);
        case "interval day to second":
          return TypeUsage.CreateDecimalTypeUsage(primitiveType, (byte) 251, (byte) 0);
        default:
          throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", lowerInvariant));
      }
      switch (primitiveTypeKind1)
      {
        case PrimitiveTypeKind.Binary:
          if (!flag)
            return TypeUsage.CreateBinaryTypeUsage(primitiveType, isFixedLength, maxLength);
          return TypeUsage.CreateBinaryTypeUsage(primitiveType, isFixedLength);
        case PrimitiveTypeKind.String:
          if (!flag)
            return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, isFixedLength, maxLength);
          return TypeUsage.CreateStringTypeUsage(primitiveType, isUnicode, isFixedLength);
        default:
          throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", lowerInvariant));
      }
    }

    public override TypeUsage GetStoreType(TypeUsage edmType)
    {
      bool bUse32DataTypes = ODTSettings.m_bUse32DataTypes;
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderManifest::GetStoreType() \n");
      EntityUtils.CheckArgumentNull<TypeUsage>(edmType, nameof (edmType));
      PrimitiveType edmType1 = edmType.EdmType as PrimitiveType;
      if (edmType1 == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", edmType.EdmType.FullName));
      ReadOnlyMetadataCollection<Facet> facets = edmType.Facets;
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderManifest::GetStoreType() \n");
      switch (edmType1.PrimitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          bool flag1 = facets["FixedLength"].Value != null && (bool) facets["FixedLength"].Value;
          Facet facet1 = facets["MaxLength"];
          bool flag2 = !bUse32DataTypes ? facet1.IsUnbounded || facet1.Value == null || (int) facet1.Value > 2000 : facet1.IsUnbounded || facet1.Value == null || (int) facet1.Value > this.BinaryMaxSize_12c;
          int maxLength1 = !flag2 ? (int) facet1.Value : int.MinValue;
          return !flag1 ? (!flag2 ? TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["raw"], false, maxLength1) : TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["blob"], false)) : (!bUse32DataTypes ? TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["raw"], true, flag2 ? 2000 : maxLength1) : TypeUsage.CreateBinaryTypeUsage(this.StoreTypeNameToStorePrimitiveType["raw"], true, flag2 ? this.BinaryMaxSize_12c : maxLength1));
        case PrimitiveTypeKind.Boolean:
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) EFOracleProviderManifest.m_edmMappingMaxBOOL, (byte) 0);
        case PrimitiveTypeKind.Byte:
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) EFOracleProviderManifest.m_edmMappingMaxBYTE, (byte) 0);
        case PrimitiveTypeKind.DateTime:
          if (facets == null || facets["Precision"].Value == null)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["date"]);
          if ((int) (byte) facets["Precision"].Value > 9)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["timestamp with local time zone"]);
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["timestamp"]);
        case PrimitiveTypeKind.Decimal:
          byte precision;
          byte scale;
          if (EF6MetadataHelpers.TryGetPrecision(edmType, out precision) && EF6MetadataHelpers.TryGetScale(edmType, out scale))
          {
            if ((int) precision == 250 && (int) scale == 0)
              return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["interval year to month"], (byte) 9, (byte) 0);
            if ((int) precision == 251 && (int) scale == 0)
              return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["interval day to second"], (byte) 9, (byte) 0);
            return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], precision, scale);
          }
          if (EF6MetadataHelpers.TryGetPrecision(edmType, out precision))
            return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], precision, (byte) 0);
          if (EF6MetadataHelpers.TryGetScale(edmType, out scale))
            return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) 38, scale);
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["number"]);
        case PrimitiveTypeKind.Double:
          if (this._version < EFOracleVersion.Oracle10gR1)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["number"]);
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["binary_double"]);
        case PrimitiveTypeKind.Guid:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["guid raw"]);
        case PrimitiveTypeKind.Single:
          if (this._version < EFOracleVersion.Oracle10gR1)
            return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["number"]);
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["binary_float"]);
        case PrimitiveTypeKind.Int16:
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) EFOracleProviderManifest.m_edmMappingMaxINT16, (byte) 0);
        case PrimitiveTypeKind.Int32:
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) EFOracleProviderManifest.m_edmMappingMaxINT32, (byte) 0);
        case PrimitiveTypeKind.Int64:
          return TypeUsage.CreateDecimalTypeUsage(this.StoreTypeNameToStorePrimitiveType["number"], (byte) EFOracleProviderManifest.m_edmMappingMaxINT64, (byte) 0);
        case PrimitiveTypeKind.String:
          bool flag3 = facets["Unicode"].Value == null || (bool) facets["Unicode"].Value;
          bool flag4 = facets["FixedLength"].Value != null && (bool) facets["FixedLength"].Value;
          Facet facet2 = facets["MaxLength"];
          bool flag5 = !bUse32DataTypes ? facet2.IsUnbounded || facet2.Value == null || (int) facet2.Value > (flag3 ? 4000 : 4000) : facet2.IsUnbounded || facet2.Value == null || (int) facet2.Value > (flag3 ? this.Nvarchar2MaxSize_12c : this.Varchar2MaxSize_12c);
          int maxLength2 = !flag5 ? (int) facet2.Value : int.MinValue;
          return !flag3 ? (!flag4 ? (!flag5 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["varchar2"], false, false, maxLength2) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["clob"], false, false)) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["char"], false, true, flag5 ? 2000 : maxLength2)) : (!flag4 ? (!flag5 ? TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nvarchar2"], true, false, maxLength2) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nclob"], true, false)) : TypeUsage.CreateStringTypeUsage(this.StoreTypeNameToStorePrimitiveType["nchar"], true, true, flag5 ? 2000 : maxLength2));
        case PrimitiveTypeKind.DateTimeOffset:
          return TypeUsage.CreateDefaultTypeUsage((EdmType) this.StoreTypeNameToStorePrimitiveType["timestamp with time zone"]);
        default:
          throw new NotSupportedException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", edmType1.PrimitiveTypeKind.ToString()));
      }
    }

    public override bool SupportsEscapingLikeArgument(out char escapeCharacter)
    {
      escapeCharacter = '\\';
      return true;
    }

    public override string EscapeLikeArgument(string argument)
    {
      bool usedEscapeChar;
      return EFOracleProviderManifest.EscapeLikeText(argument, true, out usedEscapeChar);
    }

    private XmlReader GetStoreSchemaMapping(string informationType)
    {
      string resourceName = (string) null;
      if (informationType == "StoreSchemaMapping")
        resourceName = "Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleStoreSchemaMapping.msl";
      else if (informationType == "StoreSchemaMappingVersion3")
        resourceName = "Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleStoreSchemaMappingVersion3.msl";
      return EFOracleProviderManifest.GetXmlResource(resourceName);
    }

    private XmlReader GetStoreSchemaDescription(string informationType)
    {
      if (EFOracleVersionUtils.IsVersionX(this._version))
        return EFOracleProviderManifest.GetOMDPStoreSchemaDescription(informationType);
      return (XmlReader) null;
    }

    internal static XmlReader GetXmlResource(string resourceName)
    {
      return XmlReader.Create(Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName), (XmlReaderSettings) null, resourceName);
    }

    internal static XmlReader GetOMDPStoreSchemaDescription(string informationType)
    {
      string str = (string) null;
      if (informationType == "StoreSchemaDefinition")
        str = "Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleStoreSchemaDefinition.ssdl";
      else if (informationType == "StoreSchemaDefinitionVersion3")
        str = "Oracle.ManagedDataAccess.EntityFramework.Resources.EFOracleStoreSchemaDefinitionVersion3.ssdl";
      Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(str);
      MemoryStream memoryStream = new MemoryStream();
      XmlDocument xmlDocument = new XmlDocument();
      xmlDocument.Load(manifestResourceStream);
      xmlDocument.DocumentElement.Attributes["Provider"].Value = "Oracle.ManagedDataAccess.Client";
      xmlDocument.Save((Stream) memoryStream);
      memoryStream.Seek(0L, SeekOrigin.Begin);
      manifestResourceStream.Dispose();
      return XmlReader.Create((Stream) memoryStream, (XmlReaderSettings) null, str);
    }
  }
}
