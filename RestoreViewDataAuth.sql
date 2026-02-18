-- ================================================================================
-- SCRIPT: Restore VIEW_DATAAUTH to FULL ORIGINAL SCHEMA
-- Generated : 2026-02-17
-- Database  : db_master_data (ADAM123\SQLEXPRESS)
-- ================================================================================
-- Script ini menambahkan kolom-kolom yang HILANG dari tabel VIEW_DATAAUTH.
-- Kolom yang sudah ada akan di-SKIP (IF NOT EXISTS check).
-- AMAN untuk dijalankan berulang kali (idempotent).
-- ================================================================================

USE [db_master_data];
GO

PRINT '══════════════════════════════════════════════════════';
PRINT ' Restoring VIEW_DATAAUTH — Full Original Schema';
PRINT '══════════════════════════════════════════════════════';

-- ═══ Personal Info (missing columns) ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'middle_name')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [middle_name] NVARCHAR(50) NULL; PRINT '✅ Added: middle_name'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'gender')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [gender] TINYINT NULL; PRINT '✅ Added: gender'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'user_id')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [user_id] INT NULL; PRINT '✅ Added: user_id'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'taxno')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [taxno] VARCHAR(50) NULL; PRINT '✅ Added: taxno'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'geocoord')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [geocoord] VARCHAR(50) NULL; PRINT '✅ Added: geocoord'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'req_status')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [req_status] TINYINT NULL; PRINT '✅ Added: req_status'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'lastreqno')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [lastreqno] VARCHAR(50) NULL; PRINT '✅ Added: lastreqno'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'photo')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [photo] VARCHAR(100) NULL; PRINT '✅ Added: photo'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'phone')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [phone] VARCHAR(255) NULL; PRINT '✅ Added: phone'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'birthplace')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [birthplace] VARCHAR(50) NULL; PRINT '✅ Added: birthplace'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'birthdate')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [birthdate] DATETIME NULL; PRINT '✅ Added: birthdate'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'maritalstatus')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [maritalstatus] TINYINT NULL; PRINT '✅ Added: maritalstatus'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'address')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [address] VARCHAR(255) NULL; PRINT '✅ Added: address'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'city_id')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [city_id] INT NULL; PRINT '✅ Added: city_id'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'state_id')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [state_id] INT NULL; PRINT '✅ Added: state_id'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'country_id')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [country_id] INT NULL; PRINT '✅ Added: country_id'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'zipcode')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [zipcode] VARCHAR(50) NULL; PRINT '✅ Added: zipcode'; END

-- ═══ Employment Info ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'start_date')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [start_date] DATETIME NULL; PRINT '✅ Added: start_date'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'end_date')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [end_date] DATETIME NULL; PRINT '✅ Added: end_date'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'is_main')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [is_main] TINYINT NULL; PRINT '✅ Added: is_main'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'empcompany_status')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [empcompany_status] TINYINT NULL; PRINT '✅ Added: empcompany_status'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'grade_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [grade_code] VARCHAR(50) NULL; PRINT '✅ Added: grade_code'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'employ_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [employ_code] VARCHAR(50) NULL; PRINT '✅ Added: employ_code'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'cost_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [cost_code] VARCHAR(50) NULL; PRINT '✅ Added: cost_code'; END

-- ═══ Supervisor Hierarchy ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'spv_parent')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [spv_parent] VARCHAR(50) NULL; PRINT '✅ Added: spv_parent'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'spv_pos')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [spv_pos] INT NULL; PRINT '✅ Added: spv_pos'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'spv_path')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [spv_path] VARCHAR(8000) NULL; PRINT '✅ Added: spv_path'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'spv_level')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [spv_level] TINYINT NULL; PRINT '✅ Added: spv_level'; END

-- ═══ Manager Hierarchy ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'mgr_parent')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [mgr_parent] VARCHAR(50) NULL; PRINT '✅ Added: mgr_parent'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'mgr_pos')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [mgr_pos] INT NULL; PRINT '✅ Added: mgr_pos'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'mgr_path')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [mgr_path] VARCHAR(8000) NULL; PRINT '✅ Added: mgr_path'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'mgr_level')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [mgr_level] TINYINT NULL; PRINT '✅ Added: mgr_level'; END

-- ═══ Position Info ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_code] VARCHAR(50) NULL; PRINT '✅ Added: pos_code'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'parent_id')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [parent_id] INT NULL; PRINT '✅ Added: parent_id'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_name_en')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_name_en] VARCHAR(100) NULL; PRINT '✅ Added: pos_name_en'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_name_my')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_name_my] VARCHAR(100) NULL; PRINT '✅ Added: pos_name_my'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_name_th')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_name_th] NVARCHAR(100) NULL; PRINT '✅ Added: pos_name_th'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'jobstatuscode')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [jobstatuscode] VARCHAR(100) NULL; PRINT '✅ Added: jobstatuscode'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_level')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_level] INT NULL; PRINT '✅ Added: pos_level'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'parent_path')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [parent_path] VARCHAR(255) NULL; PRINT '✅ Added: parent_path'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'pos_flag')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [pos_flag] TINYINT NULL; PRINT '✅ Added: pos_flag'; END

-- ═══ Department / Misc ═══

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'dorder')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [dorder] INT NULL; PRINT '✅ Added: dorder'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'jobtitle_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [jobtitle_code] VARCHAR(50) NULL; PRINT '✅ Added: jobtitle_code'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'report_topos')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [report_topos] INT NULL; PRINT '✅ Added: report_topos'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'clevel')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [clevel] INT NULL; PRINT '✅ Added: clevel'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'corder')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [corder] INT NULL; PRINT '✅ Added: corder'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'changeflag')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [changeflag] VARCHAR(3) NULL; PRINT '✅ Added: changeflag'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'report_postype')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [report_postype] VARCHAR(2) NULL; PRINT '✅ Added: report_postype'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'dept_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [dept_code] VARCHAR(50) NULL; PRINT '✅ Added: dept_code'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'grade_order')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [grade_order] INT NULL; PRINT '✅ Added: grade_order'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'grade_category')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [grade_category] VARCHAR(255) NULL; PRINT '✅ Added: grade_category'; END

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('VIEW_DATAAUTH') AND name = 'worklocation_code')
BEGIN ALTER TABLE [VIEW_DATAAUTH] ADD [worklocation_code] VARCHAR(50) NULL; PRINT '✅ Added: worklocation_code'; END


-- ═══ VERIFICATION ═══

PRINT '';
PRINT '══════════════════════════════════════════════════════';
PRINT ' VERIFICATION — Column count';
PRINT '══════════════════════════════════════════════════════';

SELECT
    COUNT(*) AS [Total Columns],
    CASE WHEN COUNT(*) = 63 THEN '✅ MATCH (63/63 — full schema)' 
         ELSE '⚠️ MISMATCH — expected 63, got ' + CAST(COUNT(*) AS VARCHAR) 
    END AS [Status]
FROM sys.columns
WHERE object_id = OBJECT_ID('VIEW_DATAAUTH');

PRINT '';
PRINT '══════════════════════════════════════════════════════';
PRINT ' DONE! VIEW_DATAAUTH schema restored to full original.';
PRINT '══════════════════════════════════════════════════════';
