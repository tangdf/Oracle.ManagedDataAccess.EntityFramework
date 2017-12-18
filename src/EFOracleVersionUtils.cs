// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EFOracleVersionUtils
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Oracle.ManagedDataAccess.Client;
using System;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal static class EFOracleVersionUtils
  {
    internal static EFOracleVersion GetStorageVersion(OracleConnection connection)
    {
      string serverVersion = connection.ServerVersion;
      if (serverVersion.StartsWith("9.2"))
        return EFOracleVersion.Oracle9iR2;
      if (serverVersion.StartsWith("10.1"))
        return EFOracleVersion.Oracle10gR1;
      if (serverVersion.StartsWith("10.2"))
        return EFOracleVersion.Oracle10gR2;
      if (serverVersion.StartsWith("11.1"))
        return EFOracleVersion.Oracle11gR1;
      if (serverVersion.StartsWith("11.2"))
        return EFOracleVersion.Oracle11gR2;
      if (serverVersion.StartsWith("12.1"))
        return EFOracleVersion.Oracle12cR1;
      if (serverVersion.StartsWith("12.2"))
        return EFOracleVersion.Oracle12cR2;
      throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1703, "Oracle Data Provider for .NET", "Oracle " + serverVersion));
    }

    internal static string GetVersionHint(EFOracleVersion version)
    {
      switch (version)
      {
        case EFOracleVersion.Oracle11gR1:
          return "11.1";
        case EFOracleVersion.Oracle11gR2:
          return "11.2";
        case EFOracleVersion.Oracle12cR1:
          return "12.1";
        case EFOracleVersion.Oracle12cR2:
          return "12.2";
        case EFOracleVersion.Oracle9iR2:
          return "9.2";
        case EFOracleVersion.Oracle10gR1:
          return "10.1";
        case EFOracleVersion.Oracle10gR2:
          return "10.2";
        default:
          throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, "ProviderManifestToken"));
      }
    }

    internal static EFOracleVersion GetStorageVersion(string versionHint)
    {
      if (!string.IsNullOrEmpty(versionHint))
      {
        switch (versionHint)
        {
          case "9.2":
            return EFOracleVersion.Oracle9iR2;
          case "10.1":
            return EFOracleVersion.Oracle10gR1;
          case "10.2":
            return EFOracleVersion.Oracle10gR2;
          case "11.1":
            return EFOracleVersion.Oracle11gR1;
          case "11.2":
            return EFOracleVersion.Oracle11gR2;
          case "12.1":
            return EFOracleVersion.Oracle12cR1;
          case "12.2":
            return EFOracleVersion.Oracle12cR2;
        }
      }
      throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, "ProviderManifestToken"));
    }

    internal static bool IsVersionX(EFOracleVersion storageVersion)
    {
      if (storageVersion != EFOracleVersion.Oracle10gR1 && storageVersion != EFOracleVersion.Oracle10gR2 && (storageVersion != EFOracleVersion.Oracle11gR1 && storageVersion != EFOracleVersion.Oracle11gR2) && (storageVersion != EFOracleVersion.Oracle12cR1 && storageVersion != EFOracleVersion.Oracle12cR2))
        return storageVersion == EFOracleVersion.Oracle9iR2;
      return true;
    }
  }
}
