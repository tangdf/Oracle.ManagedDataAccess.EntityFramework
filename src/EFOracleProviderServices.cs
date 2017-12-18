// Decompiled with JetBrains decompiler
// Type: Oracle.ManagedDataAccess.EntityFramework.EFOracleProviderServices
// Assembly: Oracle.ManagedDataAccess.EntityFramework, Version=6.121.2.0, Culture=neutral, PublicKeyToken=89b483f429c47342
// MVID: 849B74E4-F18B-4D59-8C09-680963D97393
// Assembly location: D:\tfs\母子健康手册\trunk\MACHMSystem\packages\Oracle.ManagedDataAccess.EntityFramework.12.1.2400\lib\net45\Oracle.ManagedDataAccess.EntityFramework.dll

using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.EntityFramework.SqlGen;
using OracleInternal.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Common.Utils;
using System.Data.Entity.Core.Common;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.DependencyResolution;
using System.Data.Entity.Migrations.Sql;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace Oracle.ManagedDataAccess.EntityFramework
{
  public sealed class EFOracleProviderServices : DbProviderServices
  {
    private static readonly EFOracleProviderServices m_providerInstance = new EFOracleProviderServices();
    internal static string m_conStr = (string) null;
    internal static string m_filteringStrs = (string) null;
    internal static string[] m_filteringStrArray = (string[]) null;
    internal static EFOracleVersion version = EFOracleVersion.Oracle11gR2;
    internal static bool m_10202_or_later_version = false;
    internal static string versionHint_static = "12.1";
    internal static bool m_GetDbProviderManifestTokenWasCalled = false;
    internal static bool m_LightSwitch_Wizard_Mode = false;
    internal readonly string ProviderInvariantName = "Oracle.ManagedDataAccess.Client";
    private string query3_StartsWith = "SELECT \r\n\"Project3\".\"C4\" AS \"C1\", \r\n\"Project3\".\"C1\" AS \"C2\", \r\n\"Project3\".\"C2\" AS \"C3\", \r\n\"Project3\".\"C3\" AS \"C4\"\r\nFROM ( SELECT \r\n";
    private string query3_var2_StartsWith = "SELECT \r\n\"Project5\".\"C4\" AS \"C1\", \r\n\"Project5\".\"C1\" AS \"C2\", \r\n\"Project5\".\"C2\" AS \"C3\", \r\n\"Project5\".\"C3\" AS \"C4\"\r\nFROM ( SELECT \r\n";
    private string Oracle_own_query3 = "select  \r\n\t1,  \r\n\tNULL,  \r\n\textent1.owner,  \r\n\tId  \r\nfrom  \r\n(  \r\nselect  \r\n\tap.owner,  \r\n\tobject_id,  \r\n\tsubprogram_id,  \r\n\t(CASE WHEN ap.procedure_name IS NULL THEN ap.object_name ELSE ap.object_name || '.' || ap.procedure_name END) Id, \r\n\toverload \r\nfrom \r\n\tall_procedures ap  \r\nwhere  \r\n\tap.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t(ap.object_type = 'PROCEDURE' or  \r\n         (ap.object_type = 'FUNCTION') or  \r\n         (ap.object_type='PACKAGE' and ap.procedure_name IS NOT NULL)  \r\n        ) and \r\n\t(OVERLOAD IS NULL OR OVERLOAD = '1') and \r\n\t(INSTR(object_name, '.') = 0) \r\n) extent1 \r\nleft outer join \r\n( \r\nselect  \r\n\towner, \r\n\tobject_id, \r\n\tsubprogram_id, \r\n\tdata_type, \r\n\tposition \r\nfrom  \r\n\tall_arguments aa  \r\nwhere  \r\n\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\tposition = 0 and  \r\n\tdata_type != 'REF CURSOR' \r\n) extent2 \r\non \r\n\textent1.object_id = extent2.object_id and \r\n\textent1.subprogram_id = extent2.subprogram_id \r\norder by 3 asc, 4 asc";
    private string query4_5_StartsWith = "SELECT \r\n\"Project6\".\"C2\" AS \"C1\", \r\n\"Project6\".\"CatalogName\" AS \"CatalogName\", \r\n\"Project6\".\"SchemaName\" AS \"SchemaName\", \r\n\"Project6\".\"Name\" AS \"Name\", \r\n\"Project6\".\"C1\" AS \"C2\", \r\n\"Project6\".\"C3\" AS \"C3\", \r\n\"Project6\".\"C4\" AS \"C4\", \r\n\"Project6\".\"C5\" AS \"C5\", \r\n\"Project6\".\"C6\" AS \"C6\", \r\n\"Project6\".\"C7\" AS \"C7\", \r\n\"Project6\".\"C8\" AS \"C8\", \r\n\"Project6\".\"C9\" AS \"C9\", \r\n\"Project6\".\"C10\" AS \"C10\", \r\n\"Project6\".\"C11\" AS \"C11\"\r\nFROM ( SELECT ";
    private string query4_5_framework_sp1_StartsWith = "SELECT \r\n\"UnionAll1\".\"Ordinal\" AS \"C1\", \r\n\"Extent1\".\"CatalogName\" AS \"CatalogName\", \r\n\"Extent1\".\"SchemaName\" AS \"SchemaName\", \r\n\"Extent1\".\"Name\" AS \"Name\", \r\n\"UnionAll1\".\"Name\" AS \"C2\", \r\n\"UnionAll1\".\"IsNullable\" AS \"C3\", \r\n\"UnionAll1\".\"TypeName\" AS \"C4\", \r\n\"UnionAll1\".\"MaxLength\" AS \"C5\", \r\n\"UnionAll1\".\"Precision\" AS \"C6\", \r\n\"UnionAll1\".\"DateTimePrecision\" AS \"C7\", \r\n\"UnionAll1\".\"Scale\" AS \"C8\", \r\n\"UnionAll1\".\"IsIdentity\" AS \"C9\", \r\n\"UnionAll1\".\"IsStoreGenerated\" AS \"C10\", \r\nCASE WHEN (\"Project5\".\"C2\" IS NULL) THEN 0 ELSE \"Project5\".\"C2\" END AS \"C11\"\r\nFROM   (";
    private string Oracle_own_query4_5 = "select \r\n     extent1.column_id, \r\n     NULL Catalog, \r\n     extent1.owner, \r\n     extent1.table_name, \r\n     extent1.column_name, \r\n     extent1.\"IsNullable\", \r\n     extent1.\"TypeName\", \r\n     extent1.\"data_length\", \r\n     extent1.\"Precision\", \r\n     extent1.\"DateTimePrecision\", \r\n     extent1.data_scale, \r\n     extent1.IsIdentity, \r\n     extent1.IsStoreGenerated, \r\n     (CASE \r\n         WHEN extent2.constraint_type = 'P' THEN 1 \r\n         ELSE 0 \r\n     END) constraint_type \r\nfrom \r\n( \r\n     select \r\n         atc.column_id, \r\n         atc.owner, \r\n         atc.table_name, \r\n         atc.column_name, \r\n         CASE atc.NULLABLE WHEN 'Y' THEN 1 ELSE 0 END \"IsNullable\", \r\n                 (CASE \r\n             WHEN INSTR(atc.data_type, 'INTERVAL YEAR') != 0 THEN  \r\n'interval year to month' \r\n                     WHEN INSTR(atc.data_type, 'INTERVAL DAY')  != 0 \r\nTHEN 'interval day to second' \r\n                     WHEN INSTR(atc.data_type, 'WITH TIME ZONE')  != 0 \r\nTHEN 'timestamp with time zone' \r\n                     WHEN INSTR(atc.data_type, 'WITH LOCAL TIME ZONE') \r\n!= 0 THEN 'timestamp with local time zone' \r\n                     WHEN INSTR(atc.data_type, 'TIMESTAMP') != 0 AND  \r\nINSTR(atc.data_type, 'TIME ZONE') = 0 THEN 'timestamp' \r\n                     WHEN atc.data_type = 'RAW' AND atc.data_length = 16 \r\nTHEN 'guid raw' \r\n                     ELSE LOWER(atc.data_type) \r\n         END) \"TypeName\", \r\n          CASE atc.data_type  \r\n          WHEN 'CHAR' THEN atc.char_length \r\n          WHEN 'VARCHAR2' THEN atc.char_length \r\n          WHEN 'NCHAR' THEN atc.char_length \r\n          WHEN 'NVARCHAR2' THEN atc.char_length  \r\n          ELSE atc.data_length END \"data_length\", \r\n         (CASE \r\n             WHEN INSTR(atc.data_type, 'INTERVAL YEAR') != 0 AND  \r\natc.DATA_PRECISION < atc.DATA_SCALE THEN atc.DATA_SCALE \r\n                 WHEN INSTR(atc.data_type, 'INTERVAL DAY') != 0 AND  \r\natc.DATA_PRECISION < atc.DATA_SCALE THEN atc.DATA_SCALE \r\n                 ELSE atc.DATA_PRECISION \r\n         END) \"Precision\", \r\n                 atc.DATA_PRECISION      \"DateTimePrecision\", \r\n         atc.data_scale, \r\n         CASE IDENTITY_COLUMN WHEN 'YES' THEN 1 ELSE 0 END  IsIdentity, \r\n                 0 IsStoreGenerated \r\n     from \r\n         all_tab_columns atc \r\n     where \r\n         ( owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n           [WHERECLAUSE1]  \r\n         ) \r\n) extent1 \r\nleft outer join \r\n( \r\n     select \r\n         acc.owner, \r\n         acc.table_name, \r\n         acc.column_name, \r\n         ac.constraint_type \r\n     from \r\n         all_cons_columns acc, \r\n         all_constraints ac \r\n     where \r\n         acc.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n         acc.owner = ac.owner and \r\n         acc.table_name = ac.table_name and \r\n         acc.constraint_name = ac.constraint_name and \r\n         ac.constraint_type = 'P' and  \r\n         ( \r\n           [WHERECLAUSE2]  \r\n         ) \r\n) extent2 \r\non \r\n     extent1.owner = extent2.owner and \r\n     extent1.table_name = extent2.table_name and \r\n     extent1.column_name = extent2.column_name \r\norder by extent1.owner asc, extent1.table_name asc, extent1.column_id asc \r\n";
    private string Oracle_own_query4_5_single_schema = "select \r\n     extent1.column_id, \r\n     NULL Catalog, \r\n     extent1.owner, \r\n     extent1.table_name, \r\n     extent1.column_name, \r\n     extent1.\"IsNullable\", \r\n     extent1.\"TypeName\", \r\n     extent1.\"data_length\", \r\n     extent1.\"Precision\", \r\n     extent1.\"DateTimePrecision\", \r\n     extent1.data_scale, \r\n     extent1.IsIdentity, \r\n     extent1.IsStoreGenerated, \r\n     (CASE \r\n         WHEN extent2.constraint_type = 'P' THEN 1 \r\n         ELSE 0 \r\n     END) constraint_type \r\nfrom \r\n( \r\n     select \r\n         atc.column_id, \r\n         SYS_CONTEXT('USERENV', 'CURRENT_USER') owner, \r\n         atc.table_name, \r\n         atc.column_name, \r\n         CASE atc.NULLABLE WHEN 'Y' THEN 1 ELSE 0 END \"IsNullable\", \r\n                 (CASE \r\n             WHEN INSTR(atc.data_type, 'INTERVAL YEAR') != 0 THEN  \r\n'interval year to month' \r\n                     WHEN INSTR(atc.data_type, 'INTERVAL DAY')  != 0 \r\nTHEN 'interval day to second' \r\n                     WHEN INSTR(atc.data_type, 'WITH TIME ZONE')  != 0 \r\nTHEN 'timestamp with time zone' \r\n                     WHEN INSTR(atc.data_type, 'WITH LOCAL TIME ZONE') \r\n!= 0 THEN 'timestamp with local time zone' \r\n                     WHEN INSTR(atc.data_type, 'TIMESTAMP') != 0 AND  \r\nINSTR(atc.data_type, 'TIME ZONE') = 0 THEN 'timestamp' \r\n                     WHEN atc.data_type = 'RAW' AND atc.data_length = 16 \r\nTHEN 'guid raw' \r\n                     ELSE LOWER(atc.data_type) \r\n         END) \"TypeName\", \r\n          CASE atc.data_type  \r\n          WHEN 'CHAR' THEN atc.char_length \r\n          WHEN 'VARCHAR2' THEN atc.char_length \r\n          WHEN 'NCHAR' THEN atc.char_length \r\n          WHEN 'NVARCHAR2' THEN atc.char_length  \r\n          ELSE atc.data_length END \"data_length\", \r\n         (CASE \r\n             WHEN INSTR(atc.data_type, 'INTERVAL YEAR') != 0 AND  \r\natc.DATA_PRECISION < atc.DATA_SCALE THEN atc.DATA_SCALE \r\n                 WHEN INSTR(atc.data_type, 'INTERVAL DAY') != 0 AND  \r\natc.DATA_PRECISION < atc.DATA_SCALE THEN atc.DATA_SCALE \r\n                 ELSE atc.DATA_PRECISION \r\n         END) \"Precision\", \r\n                 atc.DATA_PRECISION      \"DateTimePrecision\", \r\n         atc.data_scale, \r\n         CASE IDENTITY_COLUMN WHEN 'YES' THEN 1 ELSE 0 END  IsIdentity, \r\n                 0 IsStoreGenerated \r\n     from \r\n         user_tab_columns atc \r\n     where \r\n         ( \r\n           [WHERECLAUSE1]  \r\n         ) \r\n) extent1 \r\nleft outer join \r\n( \r\n     select \r\n         SYS_CONTEXT('USERENV', 'CURRENT_USER') owner, \r\n         acc.table_name, \r\n         acc.column_name, \r\n         ac.constraint_type \r\n     from \r\n         user_cons_columns acc, \r\n         user_constraints ac \r\n     where \r\n         acc.table_name = ac.table_name and \r\n         acc.constraint_name = ac.constraint_name and \r\n         ac.constraint_type = 'P' and  \r\n         ( \r\n           [WHERECLAUSE2]  \r\n         ) \r\n) extent2 \r\non \r\n     extent1.owner = extent2.owner and \r\n     extent1.table_name = extent2.table_name and \r\n     extent1.column_name = extent2.column_name \r\norder by extent1.owner asc, extent1.table_name asc, extent1.column_id asc \r\n";
    private string query6_StartsWith = "SELECT \r\n\"Project11\".\"C1\" AS \"C1\", \r\n\"Project11\".\"C5\" AS \"C2\", \r\n\"Project11\".\"C6\" AS \"C3\", \r\n\"Project11\".\"C4\" AS \"C4\", \r\n\"Project11\".\"C2\" AS \"C5\", \r\n\"Project11\".\"C8\" AS \"C6\", \r\n\"Project11\".\"C9\" AS \"C7\", \r\n\"Project11\".\"C7\" AS \"C8\", \r\n\"Project11\".\"C3\" AS \"C9\", \r\n\"Project11\".\"Name\" AS \"Name\", \r\n\"Project11\".\"Id\" AS \"Id\", \r\n\"Project11\".\"C10\" AS \"C10\"\r\nFROM ( SELECT ";
    private string query6_framework_sp1_StartsWith = "SELECT \r\n\"Join5\".\"Ordinal\" AS \"C1\", \r\n\"UnionAll4\".\"CatalogName\" AS \"C2\", \r\n\"UnionAll4\".\"SchemaName\" AS \"C3\", \r\n\"UnionAll4\".\"Name\" AS \"C4\", \r\n\"Join5\".\"Name1\" AS \"C5\", \r\n\"UnionAll5\".\"CatalogName\" AS \"C6\", \r\n\"UnionAll5\".\"SchemaName\" AS \"C7\", \r\n\"UnionAll5\".\"Name\" AS \"C8\", \r\n\"Join5\".\"Name2\" AS \"C9\", \r\n\"Extent2\".\"Name\" AS \"Name\", \r\n\"Extent1\".\"Id\" AS \"Id\", \r\nCASE WHEN (\"Extent1\".\"DeleteRule\" = 'CASCADE') THEN 1 WHEN (\"Extent1\".\"DeleteRule\" <> 'CASCADE') THEN 0 END AS \"C10\"\r\nFROM     (";
    private string query6_vs2013_update3_StartsWith = "SELECT \r\n\"Filter2\".\"Ordinal\" AS \"C1\", \r\n\"Filter2\".\"CatalogName1\" AS \"C2\", \r\n\"Filter2\".\"SchemaName1\" AS \"C3\", \r\n\"Filter2\".\"Name1\" AS \"C4\", \r\n\"Filter2\".\"Name2\" AS \"C5\", \r\n\"Filter2\".\"CatalogName2\" AS \"C6\", \r\n\"Filter2\".\"SchemaName2\" AS \"C7\", \r\n\"Filter2\".\"Name3\" AS \"C8\", \r\n\"Filter2\".\"Name4\" AS \"C9\", \r\n\"Filter2\".\"Name5\" AS \"Name\", \r\n\"Filter2\".\"Id1\" AS \"Id\", \r\nCASE WHEN (\"Filter2\".\"DeleteRule\" = 'CASCADE') THEN 1 WHEN (\"Filter2\".\"DeleteRule\" <> 'CASCADE') THEN 0 END AS \"C10\"\r\nFROM (";
    private string query6_vs2014_StartsWith = "SELECT \r\n\"Join5\".\"Ordinal\" AS \"C1\", \r\n\"UnionAll5\".\"CatalogName\" AS \"C2\", \r\n\"UnionAll5\".\"SchemaName\" AS \"C3\", \r\n\"UnionAll5\".\"Name\" AS \"C4\", \r\n\"Join5\".\"Name1\" AS \"C5\", \r\n\"UnionAll6\".\"CatalogName\" AS \"C6\", \r\n\"UnionAll6\".\"SchemaName\" AS \"C7\", \r\n\"UnionAll6\".\"Name\" AS \"C8\", \r\n\"Join5\".\"Name2\" AS \"C9\", \r\n\"Extent2\".\"Name\" AS \"Name\", \r\n\"Extent1\".\"Id\" AS \"Id\", \r\nCASE WHEN (\"Extent1\".\"DeleteRule\" = 'CASCADE') THEN 1 WHEN (\"Extent1\".\"DeleteRule\" <> 'CASCADE') THEN 0 END AS \"C10\"\r\nFROM     (";
    private string Oracle_own_query6 = "select \r\n\textent1.fk_ordinal    \"C1\", \r\n\tNULL                  \"C2\", \r\n\textent2.pk_owner      \"C3\", \r\n\textent2.pk_table      \"C4\", \r\n\textent2.pk_column     \"C5\", \r\n\tNULL                  \"C6\", \r\n\textent1.fk_owner      \"C7\", \r\n\textent1.fk_table      \"C8\", \r\n\textent1.fk_column     \"C9\", \r\n\textent1.fk_cname      \"Name\", \r\n\textent1.id            \"Id\", \r\n\textent1.fk_deleterule \"C10\" \r\nfrom \r\n( \r\n\tselect \r\n\t\tc1.owner fk_owner, \r\n\t\tc1.table_name fk_table, \r\n\t\tc1.r_constraint_name pk_cname, \r\n\t\tc1.constraint_name fk_cname, \r\n\t\tCASE WHEN c1.delete_rule = 'CASCADE' THEN 1 ELSE 0 END fk_deleterule, \r\n\t\tacc.position fk_ordinal, \r\n\t\tacc.column_name fk_column, \r\n\t\t'''' || acc.table_name || '_' || acc.constraint_name || '''' id \r\n\tfrom \r\n\t\tall_constraints c1, \r\n\t\tall_cons_columns acc \r\n\twhere \r\n\t\tc1.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\tc1.constraint_type = 'R' and \r\n\t\tc1.owner = acc.owner and \r\n\t\tc1.table_name = acc.table_name and \r\n\t\tc1.constraint_name = acc.constraint_name and \r\n\t\t( \r\n     [WHERECLAUSE1] \r\n\t\t) \r\n) extent1 \r\ninner join \r\n( \r\n\tselect \r\n\t\tc2.owner pk_owner, \r\n\t\tc2.table_name pk_table, \r\n\t\tc2.constraint_name pk_cname, \r\n\t\tc2.delete_rule pk_deleterule, \r\n\t\tacc2.column_name pk_column, \r\n   acc2.position position \r\n\tfrom \r\n\t\tall_constraints c2, \r\n\t\tall_cons_columns acc2 \r\n\twhere \r\n\t\tc2.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\tc2.constraint_type = 'P' and \r\n\t\tc2.owner = acc2.owner and \r\n\t\tc2.table_name = acc2.table_name and \r\n\t\tc2.constraint_name = acc2.constraint_name and \r\n\t\t( \r\n     [WHERECLAUSE2] \r\n\t\t)\t\r\n) extent2 \r\nON \r\nextent1.pk_cname = extent2.pk_cname and extent1.fk_ordinal = extent2.position \r\norder by \"Name\", \"Id\", \"C1\" ";
    private string Oracle_own_query6_single_schema = "select \r\n\textent1.fk_ordinal    \"C1\", \r\n\tNULL                  \"C2\", \r\n\textent2.pk_owner      \"C3\", \r\n\textent2.pk_table      \"C4\", \r\n\textent2.pk_column     \"C5\", \r\n\tNULL                  \"C6\", \r\n\textent1.fk_owner      \"C7\", \r\n\textent1.fk_table      \"C8\", \r\n\textent1.fk_column     \"C9\", \r\n\textent1.fk_cname      \"Name\", \r\n\textent1.id            \"Id\", \r\n\textent1.fk_deleterule \"C10\" \r\nfrom \r\n( \r\n\tselect \r\n\t\tSYS_CONTEXT('USERENV', 'CURRENT_USER') fk_owner, \r\n\t\tc1.table_name fk_table, \r\n\t\tc1.r_constraint_name pk_cname, \r\n\t\tc1.constraint_name fk_cname, \r\n\t\tCASE WHEN c1.delete_rule = 'CASCADE' THEN 1 ELSE 0 END fk_deleterule, \r\n\t\tacc.position fk_ordinal, \r\n\t\tacc.column_name fk_column, \r\n\t\t'''' || acc.table_name || '_' || acc.constraint_name || '''' id \r\n\tfrom \r\n\t\tuser_constraints c1, \r\n\t\tuser_cons_columns acc \r\n\twhere \r\n\t\tc1.constraint_type = 'R' and \r\n\t\tc1.table_name = acc.table_name and \r\n\t\tc1.constraint_name = acc.constraint_name and \r\n\t\t( \r\n     [WHERECLAUSE1] \r\n\t\t) \r\n) extent1 \r\ninner join \r\n( \r\n\tselect \r\n\t\tSYS_CONTEXT('USERENV', 'CURRENT_USER') pk_owner, \r\n\t\tc2.table_name pk_table, \r\n\t\tc2.constraint_name pk_cname, \r\n\t\tc2.delete_rule pk_deleterule, \r\n\t\tacc2.column_name pk_column, \r\n   acc2.position position \r\n\tfrom \r\n\t\tuser_constraints c2, \r\n\t\tuser_cons_columns acc2 \r\n\twhere \r\n\t\tc2.constraint_type = 'P' and \r\n\t\tc2.table_name = acc2.table_name and \r\n\t\tc2.constraint_name = acc2.constraint_name and \r\n\t\t( \r\n     [WHERECLAUSE2] \r\n\t\t)\t\r\n) extent2 \r\nON \r\nextent1.pk_cname = extent2.pk_cname and extent1.fk_ordinal = extent2.position \r\norder by \"Name\", \"Id\", \"C1\" ";
    private string query7_StartsWith = "SELECT \r\n\"Project7\".\"C12\" AS \"C1\", \r\n";
    private string query7_StartsWith_EF45 = "SELECT \r\n\"Project7\".\"C14\" AS \"C1\", \r\n";
    private string Oracle_own_query7 = "select  \r\n\t1 AS C1, \r\n\textent5.owner AS C2,  \r\n\textent5.Id,  \r\n\t(CASE  \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL YEAR') != 0 THEN  'interval year to month'  \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL DAY')  != 0 THEN 'interval day to second'  \r\n\t\tWHEN INSTR(ReturnType, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'  \r\n\t\tWHEN INSTR(ReturnType, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'  \r\n\t\tWHEN (INSTR(ReturnType, 'TIMESTAMP') != 0 AND INSTR(ReturnType, 'TIME ZONE') = 0) THEN 'timestamp'  \r\n   WHEN INSTR(ReturnType, 'UNDEFINED')  != 0 OR INSTR(ReturnType, 'OPAQUE/XMLTYPE')  != 0 THEN LOWER(type_name)   \r\n\t\tELSE LOWER(ReturnType) END) AS C4,  \r\n\t\t0 AS C5,  \r\n\t(CASE WHEN ReturnType IS NULL THEN 0 ELSE 1 END) AS C6,  \r\n\t0 AS C7,  \r\n\t0 AS C8,  \r\n\targument_name AS C9, \r\n\t(CASE \r\n\t\tWHEN INSTR(data_type, 'INTERVAL YEAR') != 0 THEN  'interval year to month' \r\n\t\tWHEN INSTR(data_type, 'INTERVAL DAY')  != 0 THEN 'interval day to second' \r\n\t\tWHEN INSTR(data_type, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone' \r\n\t\tWHEN INSTR(data_type, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone' \r\n\t\tWHEN (INSTR(data_type, 'TIMESTAMP') != 0 AND INSTR(data_type, 'TIME ZONE') = 0) THEN 'timestamp' \r\n   WHEN INSTR(data_type, 'UNDEFINED')  != 0 OR INSTR(data_type, 'OPAQUE/XMLTYPE') != 0 THEN LOWER(type_name) \r\n\t\tELSE LOWER(data_type) END) AS C10, \r\n\tdirection AS C11, \r\n type_name as C12 \r\nfrom \r\n( \r\n\tselect \r\n\t\textent3.owner, \r\n\t\textent3.object_id, \r\n\t\textent3.subprogram_id, \r\n\t\textent3.Id, \r\n\t\tReturnType \r\n\tfrom \r\n\t( \r\n\t\tselect \r\n\t\t\textent1.owner, \r\n\t\t\textent1.object_id, \r\n\t\t\textent1.subprogram_id, \r\n\t\t\textent1.Id, \r\n\t\t\textent2.ReturnType \r\n\t\tfrom \r\n\t\t( \r\n\t\t\tselect \r\n\t\t\t\towner, \r\n\t\t\t\tobject_id, \r\n\t\t\t\tsubprogram_id, \r\n\t\t\t\tId \t\r\n\t\t\tfrom \r\n\t\t\t( \r\n\t\t\t\tselect \r\n\t\t\t\t\towner, \r\n\t\t\t\t\tobject_id, \r\n\t\t\t\t\tsubprogram_id, \r\n\t\t\t\t\t(CASE WHEN ap.procedure_name IS NULL THEN ap.object_name ELSE ap.object_name || '.' || ap.procedure_name END) Id \r\n\t\t\t\tfrom \r\n\t\t\t\t\tall_procedures ap \r\n\t\t\t\twhere \r\n\t\t\t\t\tap.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n\t\t\t\t\t(ap.object_type = 'PROCEDURE' or \r\n\t\t\t\t\t(ap.object_type = 'FUNCTION') or \r\n\t\t\t\t\t(ap.object_type='PACKAGE' and ap.procedure_name IS NOT NULL) \r\n\t\t\t\t\t) and \r\n\t\t\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and \r\n\t\t\t\t\t(INSTR(object_name, '.') = 0) \r\n\t\t\t) \r\n\t\t\twhere \r\n\t\t\t( \r\n\t\t\t\t[WHERECLAUSE] \r\n\t\t\t) \r\n\t\t) extent1 \r\n\t\tleft outer join  \r\n\t\t(  \r\n\t\t\tselect  \r\n\t\t\t\tobject_id,  \r\n\t\t\t\tsubprogram_id,  \r\n\t\t\t\tdata_type ReturnType,  \r\n       type_name  \r\n\t\t\tfrom  \r\n\t\t\t\tall_arguments aa  \r\n\t\t\twhere  \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\tposition = 0 and  \r\n\t\t\t\tdata_type != 'REF CURSOR'  \r\n\t\t) extent2  \r\n\t\ton  \r\n\t\t\textent1.object_id = extent2.object_id and  \r\n\t\t\textent1.subprogram_id = extent2.subprogram_id  \r\n\t) extent3  \r\n\tleft outer join  \r\n\t(  \r\n\t\tselect \r\n\t\t\tobject_id,  \r\n\t\t\tsubprogram_id,  \r\n\t\t\tId,  \r\n\t\t\tdata_type, \r\n     type_name \r\n \t\tfrom \r\n\t\t( \r\n\t\t\tselect  \r\n\t\t\t\towner, \r\n\t\t\t\tobject_id,  \r\n\t\t\t\tsubprogram_id,  \r\n\t\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,  \r\n\t\t\t\tdata_type,  \r\n       type_name \r\n\t\t\tfrom  \r\n\t\t\t\tall_arguments aa  \r\n\t\t\twhere   \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\tsequence != position and  \r\n\t\t\t\t(overload is NULL or overload ='1') and  \r\n\t\t\t\tposition = 0  \r\n\t\t) \r\n\t\twhere \r\n\t\t( \r\n\t\t\t[WHERECLAUSE] \r\n\t\t) \r\n\t) extent4  \r\n\ton  \r\n\t\textent3.object_id = extent4.object_id and  \r\n\t\textent3.subprogram_id = extent4.subprogram_id \r\n) extent5  \r\nleft outer join  \r\n(  \r\n\tselect  \r\n\t\tobject_id,  \r\n\t\tsubprogram_id,  \r\n\t\tposition,  \r\n\t\tId,  \r\n\t\tdata_type,  \r\n\t\targument_name,  \r\n\t\tdirection,  \r\n   type_name  \r\n\tfrom  \r\n\t(  \r\n\t\tselect  \r\n\t\t\towner,  \r\n\t\t\tobject_id,  \r\n\t\t\tsubprogram_id,  \r\n\t\t\tposition,  \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,  \r\n\t\t\tdata_type,  \r\n\t\t\targument_name,  \r\n     (CASE WHEN IN_OUT = 'IN/OUT' THEN 'INOUT' ELSE IN_OUT END) direction,  \r\n     type_name  \r\n\t\tfrom  \r\n     all_arguments aa  \r\n\t\twhere  \r\n\t\t(  \r\n\t\t\towner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\taa.position > 0 and  \r\n\t\t\tdata_level = 0 and  \r\n\t\t\tdata_type != 'REF CURSOR'  \r\n\t\t) \r\n\t)  \r\n\twhere  \r\n\t(  \r\n\t\t[WHERECLAUSE] \r\n\t)  \r\n) extent6  \r\non  \r\n\textent5.object_id = extent6.object_id and  \r\n\textent5.subprogram_id = extent6.subprogram_id \r\norder by 2 asc, 3 asc, position asc";
    private string Oracle_own_query3_for_10201_or_earlier = "select  \r\n\t1,  \r\n\tNULL,  \r\n\towner,  \r\n\tId  \r\nfrom  \r\n(  \r\n  select \r\n\towner, \r\n  object_id,  \r\n\t(CASE WHEN PACKAGE_NAME IS NOT NULL THEN PACKAGE_NAME || '.' || OBJECT_NAME ELSE OBJECT_NAME END) Id, \r\n\toverload \r\n  from \r\n\tall_arguments \r\n  where  \r\n\towner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) AND \r\n\t(OVERLOAD IS NULL OR OVERLOAD = '1') AND \r\n\t(INSTR(object_name, '.') = 0) AND \r\n DATA_LEVEL = 0 AND ((POSITION = 0 AND SEQUENCE = 1) OR (POSITION = 1 AND SEQUENCE = 0) OR (SEQUENCE = 1)) \r\n) \r\norder by 3 asc, 4 asc";
    private string Oracle_own_query7_for_10201_or_earlier = "select   \r\n\t1 AS C1, \r\n \textent5.owner AS C2,   \r\n\textent5.Id,   \r\n\t(CASE   \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL YEAR') != 0 THEN  'interval year to month'   \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL DAY')  != 0 THEN 'interval day to second'   \r\n\t\tWHEN INSTR(ReturnType, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'   \r\n\t\tWHEN INSTR(ReturnType, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'   \r\n\t\tWHEN (INSTR(ReturnType, 'TIMESTAMP') != 0 AND INSTR(ReturnType, 'TIME ZONE') = 0) THEN 'timestamp'   \r\n   \t\tWHEN INSTR(ReturnType, 'UNDEFINED') != 0 THEN LOWER(type_name) \r\n               WHEN INSTR(ReturnType, 'REF CURSOR') != 0 THEN LOWER(type_name)   \r\n\t\tELSE LOWER(ReturnType) END) AS C4,   \r\n\t\t0 AS C5,   \r\n\t\t(CASE WHEN ((ReturnType IS NULL) OR (ReturnType = 'REF CURSOR')) THEN 0 ELSE 1 END) AS C6,   \r\n\t\t0 AS C7,   \r\n\t\t0 AS C8,   \r\n\t\targument_name AS C9,  \r\n\t\t(CASE  \r\n\t\tWHEN INSTR(data_type, 'INTERVAL YEAR') != 0 THEN  'interval year to month'  \r\n\t\tWHEN INSTR(data_type, 'INTERVAL DAY')  != 0 THEN 'interval day to second'  \r\n\t\tWHEN INSTR(data_type, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'  \r\n\t\tWHEN INSTR(data_type, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'  \r\n\t\tWHEN (INSTR(data_type, 'TIMESTAMP') != 0 AND INSTR(data_type, 'TIME ZONE') = 0) THEN 'timestamp'  \r\n               WHEN INSTR(data_type, 'UNDEFINED') != 0 THEN LOWER(type_name)  \r\n\t\tELSE LOWER(data_type) END) AS C10,  \r\n\t\tdirection AS C11,  \r\n \ttype_name as C12 \r\nfrom  \r\n(  \r\n\tselect  \r\n\t\textent3.owner,  \r\n\t\textent3.Id,  \r\n\t\tReturnType  \r\n\tfrom  \r\n\t(  \r\n\t\tselect  \r\n\t\t\textent1.owner,  \r\n\t\t\textent1.Id,  \r\n\t\t\textent1.ReturnType \r\n\t\tfrom  \r\n\t\t(  \r\n\t\t\tselect  \r\n\t\t\t\towner,  \r\n\t\t\t\tId, \r\n\t\t\t\tReturnType, \r\n\t\t\t\tposition \r\n\t\t\tfrom  \r\n\t\t\t(  \r\n\t\t\t\tselect  \r\n\t\t\t\t\towner, \r\n\t\t\t\t\t(CASE WHEN (position > 0) THEN NULL ELSE data_type END) ReturnType, \r\n\t\t\t\t\tposition,   \r\n\t\t\t\t\t(CASE WHEN aa.package_name IS NULL THEN aa.object_name ELSE aa.package_name || '.' || aa.object_name END) Id  \r\n\t\t\t\tfrom  \r\n\t\t\t\t\tall_arguments aa  \r\n\t\t\t\twhere  \r\n\t\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and  \r\n\t\t\t\t\t(INSTR(object_name, '.') = 0) and \r\n\t\t\t\t\tDATA_LEVEL = 0 and \r\n    \t\t  ((POSITION = 0 AND SEQUENCE = 1) OR (POSITION = 1 AND SEQUENCE = 0) OR (SEQUENCE = 1)) \r\n\t\t\t)  \r\n\t\t\twhere  \r\n\t\t\t(  \r\n\t\t    [WHERECLAUSE] \r\n\t\t\t)  \r\n\t\t) extent1  \r\n\t) extent3   \r\n\tleft outer join   \r\n\t(   \r\n\t\tselect   \r\n\t\t\tId,   \r\n\t\t\tdata_type,  \r\n     \ttype_name  \r\n \t\tfrom  \r\n\t\t(  \r\n\t\t\tselect   \r\n\t\t\towner,  \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,     \r\n\t\t\tdata_type,   \r\n       \t\t\ttype_name  \r\n\t\t\tfrom   \r\n\t\t\t\tall_arguments aa   \r\n\t\t\twhere    \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n\t\t\t\tsequence != position and   \r\n\t\t\t\t(overload is NULL or overload ='1') and \r\n\t\t\t\tposition = 0 AND data_type != 'REF CURSOR' \r\n\t\t)  \r\n\t\twhere  \r\n\t\t(  \r\n\t\t  [WHERECLAUSE] \r\n\t\t)  \r\n\t) extent4   \r\n\ton   \r\n\t\textent3.Id = extent4.Id \r\n) extent5   \r\nleft outer join   \r\n(   \r\n\tselect   \r\n\t\tposition,   \r\n\t\tId,   \r\n\t\tdata_type,   \r\n\t\targument_name,   \r\n\t\tdirection,   \r\n   \ttype_name   \r\n\tfrom   \r\n\t(   \r\n\t\tselect   \r\n\t\t\towner,   \r\n\t\t\tposition,   \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,   \r\n\t\t\tdata_type,   \r\n\t\t\targument_name,  \r\n\t\t\t(CASE WHEN IN_OUT = 'IN/OUT' THEN 'INOUT' ELSE IN_OUT END) direction,   \r\n     \ttype_name   \r\n\t\tfrom   \r\n     \tall_arguments aa   \r\n\t\twhere   \r\n\t\t(   \r\n\t\t\towner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and   \r\n\t\t\taa.position > 0 and   \r\n\t\t\tdata_level = 0 and   \r\n\t\t\tdata_type != 'REF CURSOR'   \r\n\t\t)  \r\n\t)   \r\n\twhere   \r\n\t(   \r\n\t\t[WHERECLAUSE] \r\n\t)   \r\n) extent6   \r\non   \r\n\textent5.Id = extent6.Id \r\norder by 2 asc, 3 asc, position asc";
    private string Oracle_own_query7_for_10201_or_earlier_EF45 = "select   \r\n\t1 AS C1, \r\n \t1 AS C2, \r\n \textent5.owner AS C3,   \r\n\textent5.Id AS C4,   \r\n\t(CASE   \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL YEAR') != 0 THEN  'interval year to month'   \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL DAY')  != 0 THEN 'interval day to second'   \r\n\t\tWHEN INSTR(ReturnType, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'   \r\n\t\tWHEN INSTR(ReturnType, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'   \r\n\t\tWHEN (INSTR(ReturnType, 'TIMESTAMP') != 0 AND INSTR(ReturnType, 'TIME ZONE') = 0) THEN 'timestamp'   \r\n   \t\tWHEN INSTR(ReturnType, 'UNDEFINED')  != 0 THEN LOWER(type_name) \r\n               WHEN INSTR(ReturnType, 'REF CURSOR')  != 0 THEN LOWER(type_name)   \r\n\t\tELSE LOWER(ReturnType) END) AS C5,   \r\n\t\t0 AS C6,   \r\n\t\t(CASE WHEN ((ReturnType IS NULL) OR (ReturnType = 'REF CURSOR')) THEN 0 ELSE 1 END) AS C7,   \r\n\t\t0 AS C8,   \r\n\t\t0 AS C9,   \r\n\t\t0 AS C10,   \r\n\t\targument_name AS C11,  \r\n\t\t(CASE  \r\n\t\tWHEN INSTR(data_type, 'INTERVAL YEAR') != 0 THEN  'interval year to month'  \r\n\t\tWHEN INSTR(data_type, 'INTERVAL DAY')  != 0 THEN 'interval day to second'  \r\n\t\tWHEN INSTR(data_type, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'  \r\n\t\tWHEN INSTR(data_type, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'  \r\n\t\tWHEN (INSTR(data_type, 'TIMESTAMP') != 0 AND INSTR(data_type, 'TIME ZONE') = 0) THEN 'timestamp'  \r\n               WHEN INSTR(data_type, 'UNDEFINED') != 0 THEN LOWER(type_name)  \r\n\t\tELSE LOWER(data_type) END) AS C12,  \r\n\t\tdirection AS C13  \r\nfrom  \r\n(  \r\n\tselect  \r\n\t\textent3.owner,  \r\n\t\textent3.Id,  \r\n\t\tReturnType  \r\n\tfrom  \r\n\t(  \r\n\t\tselect  \r\n\t\t\textent1.owner,  \r\n\t\t\textent1.Id,  \r\n\t\t\textent1.ReturnType \r\n\t\tfrom  \r\n\t\t(  \r\n\t\t\tselect  \r\n\t\t\t\towner,  \r\n\t\t\t\tId, \r\n\t\t\t\tReturnType, \r\n\t\t\t\tposition \r\n\t\t\tfrom  \r\n\t\t\t(  \r\n\t\t\t\tselect  \r\n\t\t\t\t\towner, \r\n\t\t\t\t\t(CASE WHEN (position > 0) THEN NULL ELSE data_type END) ReturnType, \r\n\t\t\t\t\tposition,   \r\n\t\t\t\t\t(CASE WHEN aa.package_name IS NULL THEN aa.object_name ELSE aa.package_name || '.' || aa.object_name END) Id  \r\n\t\t\t\tfrom  \r\n\t\t\t\t\tall_arguments aa  \r\n\t\t\t\twhere  \r\n\t\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and  \r\n\t\t\t\t\t(INSTR(object_name, '.') = 0) and \r\n\t\t\t\t\tDATA_LEVEL = 0 and \r\n    \t\t  ((POSITION = 0 AND SEQUENCE = 1) OR (POSITION = 1 AND SEQUENCE = 0) OR (SEQUENCE = 1)) \r\n\t\t\t)  \r\n\t\t\twhere  \r\n\t\t\t(  \r\n\t\t    [WHERECLAUSE] \r\n\t\t\t)  \r\n\t\t) extent1  \r\n\t) extent3   \r\n\tleft outer join   \r\n\t(   \r\n\t\tselect   \r\n\t\t\tId,   \r\n\t\t\tdata_type,  \r\n     \ttype_name  \r\n \t\tfrom  \r\n\t\t(  \r\n\t\t\tselect   \r\n\t\t\towner,  \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,     \r\n\t\t\tdata_type,   \r\n       \t\t\ttype_name  \r\n\t\t\tfrom   \r\n\t\t\t\tall_arguments aa   \r\n\t\t\twhere    \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n\t\t\t\tsequence != position and   \r\n\t\t\t\t(overload is NULL or overload ='1') and \r\n\t\t\t\tposition = 0 AND data_type != 'REF CURSOR' \r\n\t\t)  \r\n\t\twhere  \r\n\t\t(  \r\n\t\t  [WHERECLAUSE] \r\n\t\t)  \r\n\t) extent4   \r\n\ton   \r\n\t\textent3.Id = extent4.Id \r\n) extent5   \r\nleft outer join   \r\n(   \r\n\tselect   \r\n\t\tposition,   \r\n\t\tId,   \r\n\t\tdata_type,   \r\n\t\targument_name,   \r\n\t\tdirection,   \r\n   \ttype_name   \r\n\tfrom   \r\n\t(   \r\n\t\tselect   \r\n\t\t\towner,   \r\n\t\t\tposition,   \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,   \r\n\t\t\tdata_type,   \r\n\t\t\targument_name,  \r\n\t\t\t(CASE WHEN IN_OUT = 'IN/OUT' THEN 'INOUT' ELSE IN_OUT END) direction,   \r\n     \ttype_name   \r\n\t\tfrom   \r\n     \tall_arguments aa   \r\n\t\twhere   \r\n\t\t(   \r\n\t\t\towner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and   \r\n\t\t\taa.position > 0 and   \r\n\t\t\tdata_level = 0 and   \r\n\t\t\tdata_type != 'REF CURSOR'   \r\n\t\t)  \r\n\t)   \r\n\twhere   \r\n\t(   \r\n\t\t[WHERECLAUSE] \r\n\t)   \r\n) extent6   \r\non   \r\n\textent5.Id = extent6.Id \r\norder by 3 asc, 4 asc, position asc";
    private string Oracle_own_query7_EF45 = "select  \r\n\t1 AS C1, \r\n\t1 AS C2, \r\n\textent5.owner AS C3,  \r\n\textent5.Id As C4,  \r\n\t(CASE  \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL YEAR') != 0 THEN  'interval year to month'  \r\n\t\tWHEN INSTR(ReturnType, 'INTERVAL DAY')  != 0 THEN 'interval day to second'  \r\n\t\tWHEN INSTR(ReturnType, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone'  \r\n\t\tWHEN INSTR(ReturnType, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone'  \r\n\t\tWHEN (INSTR(ReturnType, 'TIMESTAMP') != 0 AND INSTR(ReturnType, 'TIME ZONE') = 0) THEN 'timestamp'  \r\n               WHEN INSTR(ReturnType, 'UNDEFINED')  != 0 OR INSTR(ReturnType, 'OPAQUE/XMLTYPE')  != 0 THEN LOWER(type_name)   \r\n\t\tELSE LOWER(ReturnType) END) AS C5,  \r\n\t\t0 AS C6,  \r\n\t(CASE WHEN ReturnType IS NULL THEN 0 ELSE 1 END) AS C7,  \r\n\t0 AS C8,  \r\n\t0 AS C9,  \r\n\t0 AS C10, \r\n\targument_name AS C11, \r\n\t(CASE \r\n\t\tWHEN INSTR(data_type, 'INTERVAL YEAR') != 0 THEN  'interval year to month' \r\n\t\tWHEN INSTR(data_type, 'INTERVAL DAY')  != 0 THEN 'interval day to second' \r\n\t\tWHEN INSTR(data_type, 'WITH TIME ZONE')  != 0 THEN 'timestamp with time zone' \r\n\t\tWHEN INSTR(data_type, 'WITH LOCAL TIME ZONE') != 0 THEN 'timestamp with local time zone' \r\n\t\tWHEN (INSTR(data_type, 'TIMESTAMP') != 0 AND INSTR(data_type, 'TIME ZONE') = 0) THEN 'timestamp' \r\n   WHEN INSTR(data_type, 'UNDEFINED')  != 0 OR INSTR(data_type, 'OPAQUE/XMLTYPE')  != 0 THEN LOWER(type_name) \r\n\t\tELSE LOWER(data_type) END) AS C12, \r\n\tdirection AS C13 \r\nfrom \r\n( \r\n\tselect \r\n\t\textent3.owner, \r\n\t\textent3.object_id, \r\n\t\textent3.subprogram_id, \r\n\t\textent3.Id, \r\n\t\tReturnType \r\n\tfrom \r\n\t( \r\n\t\tselect \r\n\t\t\textent1.owner, \r\n\t\t\textent1.object_id, \r\n\t\t\textent1.subprogram_id, \r\n\t\t\textent1.Id, \r\n\t\t\textent2.ReturnType \r\n\t\tfrom \r\n\t\t( \r\n\t\t\tselect \r\n\t\t\t\towner, \r\n\t\t\t\tobject_id, \r\n\t\t\t\tsubprogram_id, \r\n\t\t\t\tId \t\r\n\t\t\tfrom \r\n\t\t\t( \r\n\t\t\t\tselect \r\n\t\t\t\t\towner, \r\n\t\t\t\t\tobject_id, \r\n\t\t\t\t\tsubprogram_id, \r\n\t\t\t\t\t(CASE WHEN ap.procedure_name IS NULL THEN ap.object_name ELSE ap.object_name || '.' || ap.procedure_name END) Id \r\n\t\t\t\tfrom \r\n\t\t\t\t\tall_procedures ap \r\n\t\t\t\twhere \r\n\t\t\t\t\tap.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and \r\n\t\t\t\t\t(ap.object_type = 'PROCEDURE' or \r\n\t\t\t\t\t(ap.object_type = 'FUNCTION') or \r\n\t\t\t\t\t(ap.object_type='PACKAGE' and ap.procedure_name IS NOT NULL) \r\n\t\t\t\t\t) and \r\n\t\t\t\t\t(OVERLOAD IS NULL OR OVERLOAD = '1') and \r\n\t\t\t\t\t(INSTR(object_name, '.') = 0) \r\n\t\t\t) \r\n\t\t\twhere \r\n\t\t\t( \r\n\t\t\t\t[WHERECLAUSE] \r\n\t\t\t) \r\n\t\t) extent1 \r\n\t\tleft outer join  \r\n\t\t(  \r\n\t\t\tselect  \r\n\t\t\t\tobject_id,  \r\n\t\t\t\tsubprogram_id,  \r\n\t\t\t\tdata_type ReturnType,  \r\n       type_name  \r\n\t\t\tfrom  \r\n\t\t\t\tall_arguments aa  \r\n\t\t\twhere  \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\tposition = 0 and  \r\n\t\t\t\tdata_type != 'REF CURSOR'  \r\n\t\t) extent2  \r\n\t\ton  \r\n\t\t\textent1.object_id = extent2.object_id and  \r\n\t\t\textent1.subprogram_id = extent2.subprogram_id  \r\n\t) extent3  \r\n\tleft outer join  \r\n\t(  \r\n\t\tselect \r\n\t\t\tobject_id,  \r\n\t\t\tsubprogram_id,  \r\n\t\t\tId,  \r\n\t\t\tdata_type, \r\n     type_name \r\n \t\tfrom \r\n\t\t( \r\n\t\t\tselect  \r\n\t\t\t\towner, \r\n\t\t\t\tobject_id,  \r\n\t\t\t\tsubprogram_id,  \r\n\t\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,  \r\n\t\t\t\tdata_type,  \r\n       type_name \r\n\t\t\tfrom  \r\n\t\t\t\tall_arguments aa  \r\n\t\t\twhere   \r\n\t\t\t\taa.owner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\t\tsequence != position and  \r\n\t\t\t\t(overload is NULL or overload ='1') and  \r\n\t\t\t\tposition = 0  \r\n\t\t) \r\n\t\twhere \r\n\t\t( \r\n\t\t\t[WHERECLAUSE] \r\n\t\t) \r\n\t) extent4  \r\n\ton  \r\n\t\textent3.object_id = extent4.object_id and  \r\n\t\textent3.subprogram_id = extent4.subprogram_id \r\n) extent5  \r\nleft outer join  \r\n(  \r\n\tselect  \r\n\t\tobject_id,  \r\n\t\tsubprogram_id,  \r\n\t\tposition,  \r\n\t\tId,  \r\n\t\tdata_type,  \r\n\t\targument_name,  \r\n\t\tdirection,  \r\n   type_name  \r\n\tfrom  \r\n\t(  \r\n\t\tselect  \r\n\t\t\towner,  \r\n\t\t\tobject_id,  \r\n\t\t\tsubprogram_id,  \r\n\t\t\tposition,  \r\n\t\t\t(CASE WHEN aa.PACKAGE_NAME IS NOT NULL THEN aa.PACKAGE_NAME || '.' || aa.OBJECT_NAME ELSE aa.OBJECT_NAME END) Id,  \r\n\t\t\tdata_type,  \r\n\t\t\targument_name,  \r\n     (CASE WHEN IN_OUT = 'IN/OUT' THEN 'INOUT' ELSE IN_OUT END) direction,  \r\n     type_name  \r\n\t\tfrom  \r\n     all_arguments aa  \r\n\t\twhere  \r\n\t\t(  \r\n\t\t\towner in (SYS_CONTEXT('USERENV', 'CURRENT_USER')) and  \r\n\t\t\taa.position > 0 and  \r\n\t\t\tdata_level = 0 and  \r\n\t\t\tdata_type != 'REF CURSOR'  \r\n\t\t) \r\n\t)  \r\n\twhere  \r\n\t(  \r\n\t\t[WHERECLAUSE] \r\n\t)  \r\n) extent6  \r\non  \r\n\textent5.object_id = extent6.object_id and  \r\n\textent5.subprogram_id = extent6.subprogram_id \r\norder by 3 asc, 4 asc, position asc";

    public static EFOracleProviderServices Instance
    {
      get
      {
        return EFOracleProviderServices.m_providerInstance;
      }
    }

    private EFOracleProviderServices()
    {
      if (EFProviderSettings.Instance == null)
        EFProviderSettings.InitializeProviderSettings<EntityFrameworkProviderSettings>();
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<Func<MigrationSqlGenerator>>((Func<MigrationSqlGenerator>) (() => (MigrationSqlGenerator) new OracleMigrationSqlGenerator()), (object) this.ProviderInvariantName));
      this.AddDependencyResolver((IDbDependencyResolver) new SingletonDependencyResolver<IDbConnectionFactory>((IDbConnectionFactory) new OracleConnectionFactory()));
    }

    protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::CreateDbCommandDefinition()\n");
      if (!(providerManifest is EFOracleProviderManifest))
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, nameof (EFOracleProviderServices)));
      DbCommandDefinition commandDefinition = this.CreateCommandDefinition(this.CreateCommand((EFOracleProviderManifest) providerManifest, commandTree));
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::CreateDbCommandDefinition()\n");
      return commandDefinition;
    }

    private DbCommand CreateCommand(EFOracleProviderManifest providerManifest, DbCommandTree commandTree)
    {
      EntityUtils.CheckArgumentNull<EFOracleProviderManifest>(providerManifest, nameof (providerManifest));
      EntityUtils.CheckArgumentNull<DbCommandTree>(commandTree, nameof (commandTree));
      EFOracleVersion storageVersion = EFOracleVersionUtils.GetStorageVersion(providerManifest.Token);
      OracleCommand oracleCommand = new OracleCommand();
      Debugger.IsLogging();
      DbQueryCommandTree queryCommandTree = commandTree as DbQueryCommandTree;
      if (queryCommandTree != null)
      {
        DbProjectExpression query = queryCommandTree.Query as DbProjectExpression;
        if (query != null)
        {
          StructuralType edmType1 = query.Projection.ResultType.EdmType as StructuralType;
          if (edmType1 != null)
          {
                        var types = new Type[edmType1.Members.Count];
              oracleCommand.ExpectedColumnTypes=types;
            for (int index = 0; index < edmType1.Members.Count; ++index)
            {
              PrimitiveType edmType2 = edmType1.Members[index].TypeUsage.EdmType as PrimitiveType;
                types[index] = edmType2.ClrEquivalentType;
            }


          }
        }
      }
      DbFunctionCommandTree functionCommandTree = commandTree as DbFunctionCommandTree;
      if (functionCommandTree != null && functionCommandTree.ResultType != null)
      {
        EdmType edmType1 = EF6MetadataHelpers.GetElementTypeUsage(functionCommandTree.ResultType).EdmType;
        if (EF6MetadataHelpers.IsRowType((GlobalItem) edmType1))
        {
          ReadOnlyMetadataCollection<EdmMember> members = ((StructuralType) edmType1).Members;
            var types = new Type[members.Count];
            oracleCommand.ExpectedColumnTypes=types;
          for (int index = 0; index < members.Count; ++index)
          {
            PrimitiveType edmType2 = (PrimitiveType) members[index].TypeUsage.EdmType;
              types[index] = edmType2.ClrEquivalentType;
          }
        }
        else if (EF6MetadataHelpers.IsPrimitiveType(edmType1))
        {
            var types = new Type[1];
            oracleCommand.ExpectedColumnTypes=types;
            types[0] = edmType1.GetType();
        }
      }
      DbInsertCommandTree insertCommandTree = commandTree as DbInsertCommandTree;
      if (insertCommandTree != null)
      {
        DbNewInstanceExpression returning = insertCommandTree.Returning as DbNewInstanceExpression;
        if (returning != null)
        {
          int count = returning.Arguments.Count;
          if (count > 0)
          {
              var types = new Type[count];
              oracleCommand.ExpectedColumnTypes=types;
            for (int index = 0; index < count; ++index)
            {
              PrimitiveType edmType = returning.Arguments[index].ResultType.EdmType as PrimitiveType;
                types[index] = edmType.ClrEquivalentType;
            }
          }
        }
      }
      List<OracleParameter> parameters;
      CommandType commandType;
      HashSet<string> ListOfParamsToMakeUnicodeFalse;
      oracleCommand.CommandText = SqlGenerator.GenerateSql(commandTree, providerManifest, storageVersion, out parameters, out commandType, out ListOfParamsToMakeUnicodeFalse);
      oracleCommand.CommandType = commandType;
      Debugger.IsLogging();
      EdmFunction edmFunction = (EdmFunction) null;
      if (commandTree is DbFunctionCommandTree)
        edmFunction = ((DbFunctionCommandTree) commandTree).EdmFunction;
      foreach (KeyValuePair<string, TypeUsage> parameter in commandTree.Parameters)
      {
        FunctionParameter functionParameter;
        OracleParameter oracleParameter;
        if (edmFunction != null && edmFunction.Parameters.TryGetValue(parameter.Key, false, out functionParameter))
        {
          oracleParameter = EFOracleProviderServices.CreateOracleParameter(functionParameter.Name, functionParameter.TypeUsage, functionParameter.Mode, (object) DBNull.Value, storageVersion);
        }
        else
        {
          TypeUsage type = ListOfParamsToMakeUnicodeFalse == null || !ListOfParamsToMakeUnicodeFalse.Contains(parameter.Key) ? parameter.Value : EF6MetadataHelpers.CopyTypeUsageAndSetUnicodeFacetToFalse(parameter.Value);
          oracleParameter = EFOracleProviderServices.CreateOracleParameter(parameter.Key, type, ParameterMode.In, (object) DBNull.Value, storageVersion);
        }
        oracleCommand.Parameters.Add(oracleParameter);
      }
      if (parameters != null && 0 < parameters.Count)
      {
        if (!(commandTree is DbInsertCommandTree) && !(commandTree is DbUpdateCommandTree) && !(commandTree is DbDeleteCommandTree))
          throw new InvalidOperationException("SqlGenParametersNotPermitted");
        foreach (OracleParameter oracleParameter in parameters)
          oracleCommand.Parameters.Add(oracleParameter);
      }
      oracleCommand.m_isFromEF = true;
      oracleCommand.BindByName = true;
      oracleCommand.InitialLONGFetchSize = EFProviderSettings.Instance.InitialLONGFetchSize;
      if (EFProviderSettings.Instance.ThickOrThin == EFProviderSettings.EFOracleProviderType.Thick)
        oracleCommand.InitialLOBFetchSize = EFProviderSettings.Instance.InitialLOBFetchSize;
      if (EFOracleProviderServices.m_GetDbProviderManifestTokenWasCalled)
      {
        bool flag1 = false;
        bool flag2 = false;
        StringBuilder stringBuilder = new StringBuilder();
        EFOracleProviderServices.m_filteringStrs = (string) null;
        EFOracleProviderServices.m_filteringStrArray = (string[]) null;
        Hashtable hashtable = (Hashtable) null;
        if (ODTSettings.m_schemaFilterHashtable != null)
          hashtable = ODTSettings.m_schemaFilterHashtable;
        if (hashtable != null)
        {
          object obj = (object) null;
          try
          {
            OracleConnectionStringBuilder connectionStringBuilder1 = new OracleConnectionStringBuilder(EFOracleProviderServices.m_conStr);
            foreach (DictionaryEntry dictionaryEntry in hashtable)
            {
              OracleConnectionStringBuilder connectionStringBuilder2 = new OracleConnectionStringBuilder((string) dictionaryEntry.Key);
              if (connectionStringBuilder1.UserID == connectionStringBuilder2.UserID && connectionStringBuilder1.ProxyUserId == connectionStringBuilder2.ProxyUserId && (connectionStringBuilder1.DataSource == connectionStringBuilder2.DataSource && connectionStringBuilder1.DBAPrivilege == connectionStringBuilder2.DBAPrivilege))
              {
                obj = dictionaryEntry.Value;
                break;
              }
            }
            if (obj != null)
              EFOracleProviderServices.m_filteringStrArray = (string[]) obj;
          }
          catch
          {
          }
        }
        if (EFOracleProviderServices.m_filteringStrArray != null)
        {
          for (int index = 0; index < EFOracleProviderServices.m_filteringStrArray.Length; ++index)
          {
            if (!string.IsNullOrEmpty(EFOracleProviderServices.m_filteringStrArray[index]))
            {
              if (index > 0)
                stringBuilder.Append(", ");
              stringBuilder.Append(":s");
              stringBuilder.Append(index.ToString());
            }
          }
          if (stringBuilder.Length > 0)
            EFOracleProviderServices.m_filteringStrs = stringBuilder.ToString();
        }
        stringBuilder.Clear();
        string empty1 = string.Empty;
        string empty2 = string.Empty;
        if (EFOracleProviderServices.m_filteringStrArray != null && EFOracleProviderServices.m_filteringStrArray.Length > 1 && !string.IsNullOrEmpty(EFOracleProviderServices.m_filteringStrs))
          flag2 = true;
        if (oracleCommand.CommandText.StartsWith(this.query3_StartsWith) || oracleCommand.CommandText.StartsWith(this.query3_var2_StartsWith))
        {
          if (EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EDM) Oracle SQL Substitution: Query 3\n");
          if (EFOracleProviderServices.m_10202_or_later_version)
            oracleCommand.CommandText = this.Oracle_own_query3;
          else
            oracleCommand.CommandText = this.Oracle_own_query3_for_10201_or_earlier;
        }
        else if (oracleCommand.CommandText.StartsWith(this.query4_5_StartsWith) || oracleCommand.CommandText.StartsWith(this.query4_5_framework_sp1_StartsWith))
        {
          bool flag3 = false;
          if (oracleCommand.CommandText.Contains("IsUpdatable"))
            flag3 = true;
          if (!flag3 && EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EDM) Oracle SQL Substitution: Query 4\n");
          else if (EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EDM) Oracle SQL Substitution: Query 5\n");
          string str1 = !flag2 ? this.Oracle_own_query4_5_single_schema : this.Oracle_own_query4_5;
          if (oracleCommand.Parameters.Count == 0)
          {
            if (!flag3)
            {
              if (flag2)
                stringBuilder.Append("atc.table_name NOT IN (select object_name from all_objects where object_type='VIEW')");
              else
                stringBuilder.Append("atc.table_name IN (select object_name from user_objects where object_type='TABLE')");
            }
            else if (flag2)
              stringBuilder.Append("atc.table_name NOT IN (select object_name from all_objects where object_type='TABLE')");
            else
              stringBuilder.Append("atc.table_name IN (select object_name from user_objects where object_type='VIEW')");
          }
          else if (oracleCommand.Parameters.Count == 1)
          {
            if (oracleCommand.CommandText.Contains("WHERE ( NOT"))
            {
              stringBuilder.Append("NOT (atc.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              stringBuilder.Append(")");
            }
            else
            {
              stringBuilder.Append("atc.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              if (!flag3)
              {
                if (flag2)
                  stringBuilder.Append(" and atc.table_name NOT IN (select object_name from all_objects where object_type='VIEW')");
                else
                  stringBuilder.Append(" and atc.table_name IN (select object_name from user_objects where object_type='TABLE')");
              }
              else if (flag2)
                stringBuilder.Append(" and atc.table_name NOT IN (select object_name from all_objects where object_type='TABLE')");
              else
                stringBuilder.Append(" and atc.table_name IN (select object_name from user_objects where object_type='VIEW')");
            }
          }
          else
          {
            bool flag4 = false;
            if (oracleCommand.CommandText.Contains("WHERE ( NOT"))
              flag4 = true;
            if (flag4)
              stringBuilder.Append("not ");
            stringBuilder.Append("(");
            for (int index = 0; index < oracleCommand.Parameters.Count; ++index)
            {
              if (index % 2 == 0)
              {
                if (flag2)
                {
                  stringBuilder.Append("(atc.owner = ");
                  stringBuilder.Append(":");
                }
                else
                {
                  stringBuilder.Append("(SYS_CONTEXT('USERENV', 'CURRENT_USER') = ");
                  stringBuilder.Append(":");
                }
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(" and ");
              }
              else
              {
                stringBuilder.Append("atc.table_name = ");
                stringBuilder.Append(":");
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(") ");
                if (index + 2 < oracleCommand.Parameters.Count)
                  stringBuilder.Append("or \r\n");
              }
            }
            stringBuilder.Append(")");
            if (flag4)
            {
              if (!flag3)
              {
                if (flag2)
                  stringBuilder.Append(" and atc.table_name NOT IN (select object_name from all_objects where object_type='VIEW')\r\n");
                else
                  stringBuilder.Append(" and atc.table_name IN (select object_name from user_objects where object_type='TABLE')\r\n");
              }
              else if (flag2)
                stringBuilder.Append(" and atc.table_name NOT IN (select object_name from all_objects where object_type='TABLE')\r\n");
              else
                stringBuilder.Append(" and atc.table_name IN (select object_name from user_objects where object_type='VIEW')\r\n");
            }
          }
          if (stringBuilder.Length > 0)
          {
            string newValue = stringBuilder.ToString();
            string str2 = str1.Replace("[WHERECLAUSE1]", newValue).Replace("[WHERECLAUSE2]", newValue.Replace("atc.", "acc."));
            if (EFOracleProviderServices.version < EFOracleVersion.Oracle12cR1)
              str2 = str2.Replace("CASE IDENTITY_COLUMN WHEN 'YES' THEN 1 ELSE 0 END", "0");
            oracleCommand.CommandText = str2;
          }
        }
        else if (oracleCommand.CommandText.StartsWith(this.query6_StartsWith) || oracleCommand.CommandText.StartsWith(this.query6_framework_sp1_StartsWith) || (oracleCommand.CommandText.StartsWith(this.query6_vs2013_update3_StartsWith) || oracleCommand.CommandText.StartsWith(this.query6_vs2014_StartsWith)))
        {
          if (EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EDM) Oracle SQL Substitution: Query 6\n");
          string str1 = !flag2 ? this.Oracle_own_query6_single_schema : this.Oracle_own_query6;
          if (oracleCommand.Parameters.Count == 0)
            stringBuilder.Append("1 = 1");
          else if (oracleCommand.Parameters.Count == 2)
          {
            if (oracleCommand.CommandText.Contains("WHERE (( NOT"))
            {
              stringBuilder.Append("NOT (c1.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              stringBuilder.Append(" and \r\n");
              stringBuilder.Append("c1.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[1].ToString());
              stringBuilder.Append(")");
            }
            else
            {
              stringBuilder.Append("(");
              stringBuilder.Append("c1.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              stringBuilder.Append(" or \r\n");
              stringBuilder.Append("c1.table_name like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[1].ToString());
              stringBuilder.Append(")");
            }
          }
          else
          {
            bool flag3 = false;
            if (oracleCommand.CommandText.Contains("WHERE (( NOT"))
              flag3 = true;
            if (flag3)
              stringBuilder.Append("not ");
            stringBuilder.Append("(");
            for (int index = 0; index < oracleCommand.Parameters.Count; ++index)
            {
              if (index % 2 == 0)
              {
                stringBuilder.Append("(c1.owner = ");
                stringBuilder.Append(":");
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(" and ");
              }
              else
              {
                stringBuilder.Append("c1.table_name = ");
                stringBuilder.Append(":");
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(") ");
                if (index + 2 < oracleCommand.Parameters.Count)
                  stringBuilder.Append("or \r\n");
              }
            }
            stringBuilder.Append(")");
          }
          if (stringBuilder.Length > 0)
          {
            string newValue = stringBuilder.ToString();
            string str2 = str1.Replace("[WHERECLAUSE1]", newValue).Replace("[WHERECLAUSE2]", newValue.Replace("c1.", "c2."));
            oracleCommand.CommandText = str2;
          }
        }
        else if (oracleCommand.CommandText.StartsWith(this.query7_StartsWith) || (flag1 = oracleCommand.CommandText.StartsWith(this.query7_StartsWith_EF45)))
        {
          if (EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EDM) Oracle SQL Substitution: Query 7\n");
          string str1 = !EFOracleProviderServices.m_10202_or_later_version ? (!flag1 ? this.Oracle_own_query7_for_10201_or_earlier : this.Oracle_own_query7_for_10201_or_earlier_EF45) : (!flag1 ? this.Oracle_own_query7 : this.Oracle_own_query7_EF45);
          if (oracleCommand.Parameters.Count == 0)
            stringBuilder.Append("1=1");
          else if (oracleCommand.Parameters.Count == 1)
          {
            if (oracleCommand.CommandText.Contains("WHERE ( NOT"))
            {
              stringBuilder.Append("(not (Id like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              stringBuilder.Append(")) ");
            }
            else
            {
              stringBuilder.Append("(Id like ");
              stringBuilder.Append(":");
              stringBuilder.Append(oracleCommand.Parameters[0].ToString());
              stringBuilder.Append(") ");
            }
          }
          else
          {
            bool flag3 = false;
            if (oracleCommand.CommandText.Contains("WHERE ( NOT"))
              flag3 = true;
            if (flag3)
              stringBuilder.Append("not ");
            stringBuilder.Append("(");
            for (int index = 0; index < oracleCommand.Parameters.Count; ++index)
            {
              if (index % 2 == 0)
              {
                stringBuilder.Append("(owner = ");
                stringBuilder.Append(":");
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(" and ");
              }
              else
              {
                stringBuilder.Append("Id = ");
                stringBuilder.Append(":");
                stringBuilder.Append(oracleCommand.Parameters[index].ToString());
                stringBuilder.Append(") ");
                if (index + 2 < oracleCommand.Parameters.Count)
                  stringBuilder.Append("or \r\n");
              }
            }
            stringBuilder.Append(")");
          }
          if (stringBuilder.Length > 0)
          {
            string newValue = stringBuilder.ToString();
            string str2 = str1.Replace("[WHERECLAUSE]", newValue);
            oracleCommand.CommandText = str2;
          }
        }
        if (flag2 && oracleCommand.CommandText.Contains("SYS_CONTEXT('USERENV', 'CURRENT_USER')"))
        {
          oracleCommand.CommandText = oracleCommand.CommandText.Replace("SYS_CONTEXT('USERENV', 'CURRENT_USER')", EFOracleProviderServices.m_filteringStrs);
          for (int index = 0; index < EFOracleProviderServices.m_filteringStrArray.Length; ++index)
            oracleCommand.Parameters.Add(":s" + index.ToString(), OracleDbType.Varchar2, (object) EFOracleProviderServices.m_filteringStrArray[index], ParameterDirection.Input);
        }
      }
      return (DbCommand) oracleCommand;
    }

    protected override void SetDbParameterValue(DbParameter parameter, TypeUsage parameterType, object value)
    {
      parameter.Value = value;
    }

    protected override string GetDbProviderManifestToken(DbConnection connection)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::GetDbProviderManifestToken()\n");
      EntityUtils.CheckArgumentNull<DbConnection>(connection, nameof (connection));
      OracleConnection connection1 = connection as OracleConnection;
      if (connection1 == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, typeof (OracleConnection).ToString()));
      if (string.IsNullOrEmpty(connection1.ConnectionString))
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, typeof (OracleConnection).ToString()));
      bool flag = false;
      try
      {
        if (connection1.State != ConnectionState.Open)
        {
          connection1.Open();
          flag = true;
        }
        EFOracleProviderServices.version = EFOracleVersionUtils.GetStorageVersion(connection1);
        EFOracleProviderServices.m_conStr = connection1.ConnectionString;
        EFOracleProviderServices.m_GetDbProviderManifestTokenWasCalled = true;
        EFOracleProviderServices.m_LightSwitch_Wizard_Mode = !ODTSettings.EdmEventHasSubscribers();
        EFOracleProviderServices.m_10202_or_later_version = connection1.m_isDb10gR2OrHigher && (connection1.m_majorVersion >= 11 || EFOracleProviderServices.check10gR2_4thDigit(connection1.ServerVersion));
        return EFOracleVersionUtils.GetVersionHint(EFOracleProviderServices.version);
      }
      finally
      {
        if (flag)
        {
          connection1.Close();
          if (EFProviderSettings.s_tracingEnabled)
            EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::GetDbProviderManifestToken()\n");
        }
      }
    }

    protected override DbProviderManifest GetDbProviderManifest(string versionHint)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::GetDbProviderManifest()\n");
      if (string.IsNullOrEmpty(versionHint))
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, "ProviderManifestToken"));
      EFOracleProviderServices.versionHint_static = versionHint;
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::GetDbProviderManifest()\n");
      return (DbProviderManifest) new EFOracleProviderManifest(versionHint);
    }

    protected override string DbCreateDatabaseScript(string providerManifestToken, StoreItemCollection storeItemCollection)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::DbCreateDatabaseScript()\n");
      if (providerManifestToken == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "ProviderManifestToken"));
      if (storeItemCollection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "StoreItemCollection"));
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::DbCreateDatabaseScript()\n");
      return EFOracleDdlBuilder.CreateObjectsScript(storeItemCollection, providerManifestToken);
    }

    protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::DbCreateDatabase()\n");
      if (connection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "OracleConnection"));
      if (storeItemCollection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "StoreItemCollection"));
      OracleConnection connection1 = connection as OracleConnection;
      if (connection1 == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, typeof (OracleConnection).ToString()));
      string createObjectsScript = EFOracleDdlBuilder.CreateObjectsScript(storeItemCollection, EFOracleProviderServices.versionHint_static);
      EFOracleProviderServices.UsingConnection(connection1, (Action<OracleConnection>) (conn =>
      {
        string commandText = string.Empty;
        string str = string.Empty;
        int length1 = "\n\n".Length;
        if (createObjectsScript.Length > 0)
        {
          int num;
          commandText = createObjectsScript.Substring(0, num = createObjectsScript.IndexOf("\n\n"));
          str = createObjectsScript.Substring(num + length1, createObjectsScript.Length - (num + length1));
        }
        while (commandText.Length > 0)
        {
          try
          {
            if (!commandText.Contains("begin ") && !commandText.EndsWith("end;") && commandText.EndsWith(";"))
              commandText = commandText.TrimEnd(';');
            EFOracleProviderServices.CreateCommand(conn, commandText, commandTimeout).ExecuteNonQuery();
          }
          catch (Exception ex)
          {
            if (!ex.Message.Contains("ORA-00955"))
              throw;
          }
          commandText = string.Empty;
          int length2 = str.IndexOf("\n\n");
          if (str.Length > 0 && length2 > 1)
          {
            commandText = str.Substring(0, length2);
            str = str.Substring(length2 + length1, str.Length - (length2 + length1));
          }
        }
      }));
      if (!EFProviderSettings.s_tracingEnabled)
        return;
      EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::DbCreateDatabase()\n");
    }

    private static string GetDatabaseName(OracleConnection oracleConnection)
    {
      if (oracleConnection.State == ConnectionState.Closed)
        oracleConnection.Open();
      string databaseName = oracleConnection.DatabaseName;
      if (oracleConnection.State == ConnectionState.Open)
        oracleConnection.Close();
      if (string.IsNullOrEmpty(databaseName))
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "DatabaseName"));
      return databaseName;
    }

    private static void GetDatabaseFileNames(OracleConnection connection, out string dataFileName, out string logFileName)
    {
      if (connection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "OracleConnection"));
      OracleConnectionStringBuilder connectionStringBuilder = new OracleConnectionStringBuilder(connection.ConnectionString);
      dataFileName = (string) null;
      logFileName = (string) null;
    }

    private new static string ExpandDataDirectory(string filenameWithMacro)
    {
      if (filenameWithMacro == null || filenameWithMacro.Length <= "|DataDirectory|".Length)
        return (string) null;
      if (!filenameWithMacro.StartsWith("|DataDirectory|", StringComparison.OrdinalIgnoreCase))
        return (string) null;
      string str1 = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
      if (string.IsNullOrEmpty(str1))
        str1 = AppDomain.CurrentDomain.BaseDirectory;
      string str2 = filenameWithMacro.Substring("|DataDirectory|".Length, filenameWithMacro.Length - "|DataDirectory|".Length);
      if (0 < str1.Length && (int) str1[str1.Length - 1] == 92)
        str1 = str1.Substring(0, str1.Length - 1);
      if (0 >= str2.Length || (int) str2[0] != 92)
        str2 = "\\" + str2;
      return str1 + str2;
    }

    protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::DbDatabaseExists()\n");
      if (connection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "OracleConnection"));
      if (storeItemCollection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "StoreItemCollection"));
      OracleConnection oracleConnection = connection as OracleConnection;
      if (oracleConnection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, typeof (OracleConnection).ToString()));
      EFOracleProviderServices.GetDatabaseName(oracleConnection);
      bool exists = false;
      EFOracleProviderServices.UsingMasterConnection(oracleConnection, (Action<OracleConnection>) (conn =>
      {
        EFOracleProviderServices.version = EFOracleVersionUtils.GetStorageVersion(conn);
        EFOracleProviderServices.m_conStr = conn.ConnectionString;
        int count = 0;
        string tableExistsScript = EFOracleDdlBuilder.CreateTableExistsScript(storeItemCollection, out count);
        if (count > 0)
          exists = (int) ((Decimal) EFOracleProviderServices.CreateCommand(conn, tableExistsScript, commandTimeout).ExecuteScalar()) > 0;
        else
          exists = true;
      }));
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::DbDatabaseExists()\n");
      return exists;
    }

    protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
    {
      if (EFProviderSettings.s_tracingEnabled)
        EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (ENTRY) EFOracleProviderServices::DbDeleteDatabase()\n");
      if (connection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "OracleConnection"));
      if (storeItemCollection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-7000, "StoreItemCollection"));
      OracleConnection oracleConnection = connection as OracleConnection;
      if (oracleConnection == null)
        throw new ArgumentException(EFProviderSettings.Instance.GetErrorMessage(-1202, typeof (OracleConnection).ToString()));
      string str1 = EFOracleDdlBuilder.DropDatabaseScript(EFOracleProviderServices.GetDatabaseName(oracleConnection), storeItemCollection);
      OracleConnection.ClearPool(oracleConnection);
      string piece = string.Empty;
      string empty = string.Empty;
      int length1 = "\n\n".Length;
      int num;
      piece = str1.Substring(0, num = str1.IndexOf("\n\n"));
      string str2 = str1.Substring(num + length1, str1.Length - (num + length1));
      while (piece.Length > 0)
      {
        try
        {
          EFOracleProviderServices.UsingMasterConnection(oracleConnection, (Action<OracleConnection>) (conn => EFOracleProviderServices.CreateCommand(conn, piece, commandTimeout).ExecuteNonQuery()));
        }
        catch (Exception ex)
        {
          if (!ex.Message.Contains("ORA-00942"))
          {
            if (!ex.Message.Contains("ORA-02289"))
              throw;
          }
        }
        piece = string.Empty;
        int length2 = str2.IndexOf("\n\n");
        if (str2.Length > 0 && length2 > 1)
        {
          piece = str2.Substring(0, length2);
          str2 = str2.Substring(length2 + length1, str2.Length - (length2 + length1));
        }
      }
      if (!EFProviderSettings.s_tracingEnabled)
        return;
      EFProviderSettings.Instance.Trace(EFProviderSettings.EFTraceLevel.Entry, " (EXIT) EFOracleProviderServices::DbDeleteDatabase()\n");
    }

    private static DbCommand CreateCommand(OracleConnection connection, string commandText, int? commandTimeout)
    {
      OracleCommand oracleCommand = new OracleCommand(commandText, connection);
      if (commandTimeout.HasValue)
        oracleCommand.CommandTimeout = commandTimeout.Value;
      return (DbCommand) oracleCommand;
    }

    private static void UsingConnection(OracleConnection connection, Action<OracleConnection> act)
    {
      bool flag = connection.State == ConnectionState.Closed;
      if (flag)
        connection.Open();
      try
      {
        act(connection);
      }
      finally
      {
        if (flag && connection.State == ConnectionState.Open)
          connection.Close();
      }
    }

    private static void UsingMasterConnection(OracleConnection connection, Action<OracleConnection> act)
    {
      OracleConnectionStringBuilder connectionStringBuilder = new OracleConnectionStringBuilder(connection.ConnectionString);
      try
      {
        using (OracleConnection connection1 = (OracleConnection) connection.Clone())
          EFOracleProviderServices.UsingConnection(connection1, act);
      }
      catch (OracleException ex)
      {
        if (string.IsNullOrEmpty(connectionStringBuilder.UserID))
          throw new InvalidOperationException("Credentials are missing from the connection string");
        if (ex.Message.Contains("ORA-00942") || ex.Message.Contains("ORA-00955"))
          return;
        throw;
      }
    }

    internal static OracleParameter CreateOracleParameter(string name, TypeUsage type, ParameterMode mode, object value, EFOracleVersion version)
    {
      Type type1 = value.GetType();
      OracleParameter oracleParameter = !(type1 == typeof (bool)) ? (!(type1 == typeof (Guid)) ? (!(type1 == typeof (DateTimeOffset)) ? new OracleParameter(name, value) : new OracleParameter(name, (object) null)) : new OracleParameter(name, (object) ((Guid) value).ToByteArray())) : (type == null || type.EdmType == null || (type.EdmType.Name == null || !(type.EdmType.Name.ToLowerInvariant() == "pl/sql boolean")) ? new OracleParameter(name, Convert.ChangeType((object) (bool) value, TypeCode.Decimal, (IFormatProvider) CultureInfo.InvariantCulture)) : new OracleParameter(name, (object) (bool) value));
      ParameterDirection parameterDirection = EF6MetadataHelpers.ParameterModeToParameterDirection(mode);
      if (oracleParameter.Direction != parameterDirection)
        oracleParameter.Direction = parameterDirection;
      bool isOutParam = mode != ParameterMode.In;
      int? size;
      byte? precision;
      byte? scale;
      OracleDbType dbType = EFOracleProviderServices.GetDbType(type, isOutParam, version, out size, out precision, out scale);
      if (oracleParameter.OracleDbType != dbType)
        oracleParameter.OracleDbType = dbType;
      if (size.HasValue && (isOutParam || oracleParameter.Size != size.Value))
        oracleParameter.Size = size.Value;
      if (precision.HasValue && (isOutParam || (int) oracleParameter.Precision != (int) precision.Value))
        oracleParameter.Precision = precision.Value;
      if (scale.HasValue && (isOutParam || (int) oracleParameter.Scale != (int) scale.Value))
        oracleParameter.Scale = scale.Value;
      bool flag = EF6MetadataHelpers.IsNullable(type);
      if (isOutParam || flag != oracleParameter.IsNullable)
        oracleParameter.IsNullable = flag;
      if (oracleParameter.OracleDbType == OracleDbType.IntervalYM)
      {
        if (!(value is DBNull))
          oracleParameter.Value = (object) Decimal.ToInt64((Decimal) value);
      }
      else if (oracleParameter.OracleDbType == OracleDbType.IntervalDS)
      {
        if (!(value is DBNull))
        {
          TimeSpan timeSpan = TimeSpan.FromSeconds((double) ((Decimal) value));
          oracleParameter.Value = (object) timeSpan;
        }
      }
      else if (oracleParameter.OracleDbType == OracleDbType.TimeStampTZ)
      {
        oracleParameter.m_bReturnDateTimeOffset=true ;
        if (!(value is DBNull))
          oracleParameter.Value = (object) (DateTimeOffset) value;
      }
      oracleParameter.OracleDbTypeEx = oracleParameter.OracleDbType;
      if (!isOutParam && type1 == typeof (string) && value != null)
      {
        int length = ((string) value).Length;
        if (length > oracleParameter.Size)
          oracleParameter.Size = length;
      }
      return oracleParameter;
    }

    private static OracleDbType GetDbType(TypeUsage type, bool isOutParam, EFOracleVersion version, out int? size, out byte? precision, out byte? scale)
    {
      PrimitiveTypeKind primitiveTypeKind = EF6MetadataHelpers.GetPrimitiveTypeKind(type);
      size = new int?();
      precision = new byte?();
      scale = new byte?();
      switch (primitiveTypeKind)
      {
        case PrimitiveTypeKind.Binary:
          size = EFOracleProviderServices.GetParameterSize(type, isOutParam);
          if (type != null && type.EdmType != null && type.EdmType.Name != null)
          {
            if (type.EdmType.Name.ToLowerInvariant() == "blob")
              return OracleDbType.Blob;
            if (type.EdmType.Name.ToLowerInvariant() == "raw")
              return OracleDbType.Raw;
            if (type.EdmType.Name.ToLowerInvariant() == "long raw")
              return OracleDbType.LongRaw;
            if (type.EdmType.Name.ToLowerInvariant() == "bfile")
              return OracleDbType.BFile;
          }
          OracleDbType binaryDbType = EFOracleProviderServices.GetBinaryDbType(type);
          if (!size.HasValue && (binaryDbType == OracleDbType.Blob || binaryDbType == OracleDbType.LongRaw))
            return OracleDbType.Blob;
          return binaryDbType;
        case PrimitiveTypeKind.Boolean:
          return OracleDbType.Decimal;
        case PrimitiveTypeKind.Byte:
          return OracleDbType.Byte;
        case PrimitiveTypeKind.DateTime:
          if (type != null && type.EdmType != null && type.EdmType.Name != null)
          {
            if (type.EdmType.Name.ToLowerInvariant() == "date")
              return OracleDbType.Date;
            if (type.EdmType.Name.ToLowerInvariant() == "timestamp")
              return OracleDbType.TimeStamp;
            if (type.EdmType.Name.ToLowerInvariant() == "timestamp with local time zone")
              return OracleDbType.TimeStampLTZ;
          }
          if (type.Facets["Precision"].Value == null)
            return OracleDbType.Date;
          return (int) (byte) type.Facets["Precision"].Value > 9 ? OracleDbType.TimeStampLTZ : OracleDbType.TimeStamp;
        case PrimitiveTypeKind.Decimal:
          precision = EFOracleProviderServices.GetParameterPrecision(type, new byte?());
          scale = EFOracleProviderServices.GetScale(type);
          if (type != null && type.EdmType != null && type.EdmType.Name != null)
          {
            if (type.EdmType.Name.ToLowerInvariant() == "number")
              return OracleDbType.Decimal;
            if (type.EdmType.Name.ToLowerInvariant() == "interval year to month")
              return OracleDbType.IntervalYM;
            if (type.EdmType.Name.ToLowerInvariant() == "interval day to second")
              return OracleDbType.IntervalDS;
            if (type.EdmType.Name.ToLowerInvariant() == "float")
              return OracleDbType.Decimal;
          }
          byte? nullable1 = precision;
          if ((nullable1.HasValue ? new int?((int) nullable1.GetValueOrDefault()) : new int?()).HasValue)
          {
            byte? nullable2 = precision;
            if (((int) nullable2.GetValueOrDefault() != 250 ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
              return OracleDbType.IntervalYM;
          }
          byte? nullable3 = precision;
          if ((nullable3.HasValue ? new int?((int) nullable3.GetValueOrDefault()) : new int?()).HasValue)
          {
            byte? nullable2 = precision;
            if (((int) nullable2.GetValueOrDefault() != 251 ? 0 : (nullable2.HasValue ? 1 : 0)) != 0)
              return OracleDbType.IntervalDS;
          }
          return OracleDbType.Decimal;
        case PrimitiveTypeKind.Double:
          return OracleDbType.Double;
        case PrimitiveTypeKind.Guid:
          return OracleDbType.Raw;
        case PrimitiveTypeKind.Single:
          return OracleDbType.Single;
        case PrimitiveTypeKind.SByte:
          return OracleDbType.Byte;
        case PrimitiveTypeKind.Int16:
          return OracleDbType.Int16;
        case PrimitiveTypeKind.Int32:
          return OracleDbType.Int32;
        case PrimitiveTypeKind.Int64:
          return OracleDbType.Int64;
        case PrimitiveTypeKind.String:
          size = EFOracleProviderServices.GetParameterSize(type, isOutParam);
          if (type != null && type.EdmType != null && type.EdmType.Name != null)
          {
            if (type.EdmType.Name.ToLowerInvariant() == "xmltype")
              return OracleDbType.XmlType;
            if (type.EdmType.Name.ToLowerInvariant() == "nvarchar2")
              return OracleDbType.NVarchar2;
            if (type.EdmType.Name.ToLowerInvariant() == "varchar2")
              return OracleDbType.Varchar2;
            if (type.EdmType.Name.ToLowerInvariant() == "nclob")
              return OracleDbType.NClob;
            if (type.EdmType.Name.ToLowerInvariant() == "clob")
              return OracleDbType.Clob;
            if (type.EdmType.Name.ToLowerInvariant() == "nchar")
              return OracleDbType.NChar;
            if (type.EdmType.Name.ToLowerInvariant() == "long")
              return OracleDbType.Long;
            if (type.EdmType.Name.ToLowerInvariant() == "char")
              return OracleDbType.Char;
            if (type.EdmType.Name.ToLowerInvariant() == "rowid" || type.EdmType.Name.ToLowerInvariant() == "urowid")
              return OracleDbType.Varchar2;
          }
          OracleDbType stringDbType = EFOracleProviderServices.GetStringDbType(type);
          if (!size.HasValue)
          {
            if (stringDbType == OracleDbType.Char || stringDbType == OracleDbType.Varchar2)
              return OracleDbType.Clob;
            if (stringDbType == OracleDbType.NChar || stringDbType == OracleDbType.NVarchar2)
              return OracleDbType.NClob;
          }
          return stringDbType;
        case PrimitiveTypeKind.Time:
          return OracleDbType.TimeStamp;
        case PrimitiveTypeKind.DateTimeOffset:
          return OracleDbType.TimeStampTZ;
        default:
          throw new InvalidOperationException(EFProviderSettings.Instance.GetErrorMessage(-1202, primitiveTypeKind.ToString()));
      }
    }

    private static int? GetParameterSize(TypeUsage type, bool isOutParam)
    {
      int maxLength;
      if (EF6MetadataHelpers.TryGetMaxLength(type, out maxLength))
        return new int?(maxLength);
      if (type.EdmType.Name.ToLowerInvariant() == "varchar2")
      {
        if (EFOracleVersionUtils.GetStorageVersion(EFOracleProviderServices.versionHint_static) >= EFOracleVersion.Oracle12cR1)
          return new int?((int) short.MaxValue);
        return new int?(4000);
      }
      if (type.EdmType.Name.ToLowerInvariant() == "nvarchar2")
      {
        if (EFOracleVersionUtils.GetStorageVersion(EFOracleProviderServices.versionHint_static) >= EFOracleVersion.Oracle12cR1)
          return new int?((int) short.MaxValue);
        return new int?(4000);
      }
      if (type.EdmType.Name.ToLowerInvariant() == "char")
        return new int?(2000);
      if (type.EdmType.Name.ToLowerInvariant() == "nchar")
        return new int?(2000);
      if (type.EdmType.Name.ToLowerInvariant() == "raw")
      {
        if (EFOracleVersionUtils.GetStorageVersion(EFOracleProviderServices.versionHint_static) >= EFOracleVersion.Oracle12cR1)
          return new int?((int) short.MaxValue);
        return new int?(2000);
      }
      if (type.EdmType.Name.ToLowerInvariant() == "rowid")
        return new int?(18);
      if (type.EdmType.Name.ToLowerInvariant() == "urowid")
        return new int?(4000);
      if (type.EdmType.Name.ToLowerInvariant() == "long")
        return new int?(32670);
      if (type.EdmType.Name.ToLowerInvariant() == "long raw")
        return new int?(32670);
      return new int?();
    }

    private static OracleDbType GetStringDbType(TypeUsage type)
    {
      OracleDbType oracleDbType;
      if (type.EdmType.Name.ToLowerInvariant() == "xmltype")
      {
        oracleDbType = OracleDbType.XmlType;
      }
      else
      {
        bool isFixedLength;
        if (!EF6MetadataHelpers.TryGetIsFixedLength(type, out isFixedLength))
          isFixedLength = false;
        bool isUnicode;
        if (!EF6MetadataHelpers.TryGetIsUnicode(type, out isUnicode))
          isUnicode = true;
        oracleDbType = !isFixedLength ? (isUnicode ? OracleDbType.NVarchar2 : OracleDbType.Varchar2) : (isUnicode ? OracleDbType.NChar : OracleDbType.Char);
      }
      return oracleDbType;
    }

    private static OracleDbType GetBinaryDbType(TypeUsage type)
    {
      bool isFixedLength;
      if (!EF6MetadataHelpers.TryGetIsFixedLength(type, out isFixedLength))
        isFixedLength = false;
      int maxLength = 0;
      if (EF6MetadataHelpers.TryGetMaxLength(type, out maxLength))
      {
        if (maxLength <= 2000)
          return OracleDbType.Raw;
        return !isFixedLength ? OracleDbType.Blob : OracleDbType.LongRaw;
      }
      return !isFixedLength ? OracleDbType.Blob : OracleDbType.LongRaw;
    }

    private static byte? GetParameterPrecision(TypeUsage type, byte? defaultIfUndefined)
    {
      byte precision;
      if (EF6MetadataHelpers.TryGetPrecision(type, out precision))
        return new byte?(precision);
      return defaultIfUndefined;
    }

    private static byte? GetScale(TypeUsage type)
    {
      byte scale;
      if (EF6MetadataHelpers.TryGetScale(type, out scale))
        return new byte?(scale);
      return new byte?();
    }

    internal static bool check10gR2_4thDigit(string dbVersion)
    {
      string str = dbVersion.Substring("10.2.0.".Length);
      int length = str.IndexOf(".");
      return int.Parse(str.Substring(0, length)) >= 2;
    }
  }
}
