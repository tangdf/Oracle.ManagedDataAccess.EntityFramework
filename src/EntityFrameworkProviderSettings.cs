// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EntityFrameworkProviderSettings
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Oracle.ManagedDataAccess.Client;
using OracleInternal.Common;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  internal class EntityFrameworkProviderSettings : EFProviderSettings.IEFProviderSettings
  {
    EFProviderSettings.EFOracleProviderType EFProviderSettings.IEFProviderSettings.ThickOrThin
    {
      get
      {
        return EFProviderSettings.EFOracleProviderType.Thin;
      }
    }

    int EFProviderSettings.IEFProviderSettings.InitialLONGFetchSize
    {
      get
      {
        return ConfigBaseClass.m_InitialLONGFetchSize;
      }
    }

    int EFProviderSettings.IEFProviderSettings.InitialLOBFetchSize
    {
      get
      {
        return ConfigBaseClass.m_InitialLOBFetchSize;
      }
    }

    bool EFProviderSettings.IEFProviderSettings.TracingEnabled
    {
      get
      {
        return ConfigBaseClass.m_TraceLevel > 0;
      }
    }

    void EFProviderSettings.IEFProviderSettings.Trace(EFProviderSettings.EFTraceLevel level, string message)
    {
      OracleTraceTag traceTag = OracleTraceTag.Entry;
      if (level == EFProviderSettings.EFTraceLevel.Entry)
        traceTag = OracleTraceTag.Exit;
      Trace.Write(OracleTraceLevel.Public, traceTag, message);
    }

    int EFProviderSettings.IEFProviderSettings.GetMaxPrecision(string typeName, bool isEF6OrHigher)
    {
      return ConfigBaseClass.GetMaxPrecision(typeName, isEF6OrHigher);
    }

    string EFProviderSettings.IEFProviderSettings.GetErrorMessage(int errorCode, params string[] args)
    {
      return OracleStringResourceManager.GetErrorMesg(errorCode, args);
    }
  }
}
