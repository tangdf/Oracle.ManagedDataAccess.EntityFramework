// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EFProviderSettings
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal static class EFProviderSettings
  {
    internal const int ODP_NOT_SUPPORTED = -1703;
    internal const int ODP_INVALID_VALUE = -1202;
    internal const int EF_NILADIC_FUNCTION = -5000;
    internal const int EF_READ_ONLY_ENTITY = -5001;
    internal const int EF_CODE_FIRST_MUST_NOT_BE_NULL = -7000;
    internal const int EF_CODE_FIRST_MISSING_FROM_CONN_STRING = -7000;
    internal static EFProviderSettings.IEFProviderSettings Instance;
    internal static bool s_tracingEnabled;

    internal static void InitializeProviderSettings<T>() where T : EFProviderSettings.IEFProviderSettings, new()
    {
      if (EFProviderSettings.Instance == null)
        EFProviderSettings.Instance = (EFProviderSettings.IEFProviderSettings) new T();
      EFProviderSettings.s_tracingEnabled = EFProviderSettings.Instance.TracingEnabled;
    }

    internal interface IEFProviderSettings
    {
      EFProviderSettings.EFOracleProviderType ThickOrThin { get; }

      int InitialLONGFetchSize { get; }

      int InitialLOBFetchSize { get; }

      bool TracingEnabled { get; }

      void Trace(EFProviderSettings.EFTraceLevel level, string message);

      int GetMaxPrecision(string typeName, bool isEF6OrHigher);

      string GetErrorMessage(int errorCode, params string[] args);
    }

    internal enum EFTraceLevel : byte
    {
      None = 0,
      Entry = 1,
      Exit = 1,
    }

    internal enum EFOracleProviderType : byte
    {
      Thick,
      Thin,
    }
  }
}
