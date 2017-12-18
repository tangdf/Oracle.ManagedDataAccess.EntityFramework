//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Collections.Specialized;
//using System.Data;
//using System.Diagnostics;
//using System.Linq;
//using System.Security.Permissions;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;
//using Microsoft.Win32;
//using Oracle.ManagedDataAccess.Client;
//
//namespace OracleInternal.Common
//{
//    internal static class ODTSettings
//    {
//        public static void FireEdmInUseEvent()
//        {
//            
//        }
//
//        public static bool m_bUse32DataTypes
//        {
//            get { return false; }
//        }
//
//        public static Hashtable m_schemaFilterHashtable
//        {
//            get { return null; }
//        }
//
//        public static bool EdmEventHasSubscribers()
//        {
//            return false; 
//        }
//
//        public static bool m_bUseLongIdentifiers
//        {
//            get { return false; }
//        }
//    }
//
//    internal enum OracleTraceTag
//    {
//        None = 0,
//        Environment = 1,
//        Version = 2,
//        Config = 4,
//        Sqlnet = 8,
//        Tnsnames = 16,
//        Entry = 256,
//        Exit = 512,
//        SQL = 1024,
//        CP = 2048,
//        MTS = 4096,
//        EDM = 8192,
//        REFCursor = 16384,
//        EF = 32768,
//        SelfTuning = 65536,
//        TTC = 131072,
//        SvcObj = 262144,
//        RLB = 524288,
//        HA = 1048576,
//        ONS = 2097152,
//        BUF = 4194304,
//        XML = 8388608,
//        Send = 16777216,
//        Receive = 33554432,
//        Prm = 67108864,
//        Error = 268435456,
//    }
//
//
//    public static class OracleConnetionExtension
//    {
//        public static bool m_isDb10gR2OrHigher(this OracleConnection oracleConnection)
//        {
//            return false;
//        }
//        public static  int m_majorVersion(this OracleConnection oracleConnection)
//        {
//            return 1;
//        }
//    }
//
//    public static class OracleParameterExtension
//    {
//        public static void m_bReturnDateTimeOffset(this OracleParameter oracleParameter,bool value)
//        {
//            
//        }
//    }
//
//    internal  static  class Trace
//    {
//        public static void Write(OracleTraceLevel traceLevel, OracleTraceTag traceTag, params string[] args)
//        {
//            
//        }
//    }
//    public static class OracleStringResourceManager
//    {
//        public static string GetErrorMesg(int errorCode, params string[] args)
//        {
//            return string.Empty;
//        }
//    }
//    internal class ConfigBaseClass
//    {
//         
//        protected ConfigBaseClass()
//        {
//        }
//
//        internal static int GetMaxPrecision(string edmType, bool isEF6OrHigher = false)
//        {
//            throw new NotImplementedException();
//        }
//
//
//        public static int m_TraceLevel;
//
//        public static int m_InitialLOBFetchSize;
//        public static int m_InitialLONGFetchSize;
//
//        public static bool s_bLegacyEdmMappingPresent = false;
//        internal static DbType[] s_edmPrecisonMapping = new DbType[39];
//
//
//    }
//
//    internal enum OracleTraceLevel
//    {
//        None = 0,
//        Public = 1,
//        Private = 2,
//        Network = 4,
//        Config = 268435456,
//    }
//
//
//    internal static class OracleCommandExtension
//    {
//        public static void ExpectedColumnTypes(this OracleCommand command, Type[] expectedColumnTypes)
//        {
//            
//        }
//
//        public static void m_isFromEF(this OracleCommand command, bool isFromEF)
//        {
//            
//        }
//    }
//}
