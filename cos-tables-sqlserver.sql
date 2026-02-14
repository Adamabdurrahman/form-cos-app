-- ══════════════════════════════════════════════════════════════════════════════
-- COS VALIDATION FORM — SQL Server Script
-- Jalankan di database db_master_data yang sudah ada
-- Script ini HANYA membuat tabel cos_* (tabel baru) + isi seed data
-- Tabel tlkp_* dan VIEW_* TIDAK disentuh (sudah ada)
-- ══════════════════════════════════════════════════════════════════════════════

USE [db_master_data];
GO

-- ┌─────────────────────────────────────────────────────────────┐
-- │  1. CREATE TABLES                                           │
-- └─────────────────────────────────────────────────────────────┘

-- ── 1.1 Form Definitions ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_form_definitions')
BEGIN
    CREATE TABLE [dbo].[cos_form_definitions] (
        [id]              INT            IDENTITY(1,1) NOT NULL,
        [code]            NVARCHAR(50)   NOT NULL,
        [title]           NVARCHAR(200)  NOT NULL,
        [subtitle]        NVARCHAR(300)  NULL,
        [doc_number]      NVARCHAR(50)   NULL,
        [revision]        NVARCHAR(20)   NULL,
        [effective_date]  DATETIME2      NULL,
        [slot_count]      INT            NOT NULL DEFAULT 5,
        [is_active]       BIT            NOT NULL DEFAULT 1,
        [created_at]      DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        [updated_at]      DATETIME2      NULL,
        CONSTRAINT [PK_cos_form_definitions] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [UQ_cos_form_definitions_code] UNIQUE ([code])
    );
    PRINT 'Created: cos_form_definitions';
END
GO

-- ── 1.2 Check Items ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_check_items')
BEGIN
    CREATE TABLE [dbo].[cos_check_items] (
        [id]                INT            IDENTITY(1,1) NOT NULL,
        [form_id]           INT            NOT NULL,
        [item_key]          NVARCHAR(100)  NOT NULL,
        [label]             NVARCHAR(300)  NOT NULL,
        [type]              NVARCHAR(30)   NOT NULL,       -- visual, numeric, conditional
        [visual_standard]   NVARCHAR(200)  NULL,
        [numeric_std_key]   NVARCHAR(100)  NULL,
        [fixed_standard]    NVARCHAR(100)  NULL,
        [fixed_min]         DECIMAL(18,4)  NULL,
        [fixed_max]         DECIMAL(18,4)  NULL,
        [frequency]         NVARCHAR(100)  NULL,
        [keterangan]        NVARCHAR(300)  NULL,
        [conditional_label] NVARCHAR(200)  NULL,
        [sort_order]        INT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_cos_check_items] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_check_items_form] FOREIGN KEY ([form_id])
            REFERENCES [dbo].[cos_form_definitions]([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_cos_check_items_form_key] UNIQUE ([form_id], [item_key])
    );
    PRINT 'Created: cos_check_items';
END
GO

-- ── 1.3 Check Sub Rows ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_check_sub_rows')
BEGIN
    CREATE TABLE [dbo].[cos_check_sub_rows] (
        [id]              INT            IDENTITY(1,1) NOT NULL,
        [check_item_id]   INT            NOT NULL,
        [suffix]          NVARCHAR(30)   NOT NULL,
        [label]           NVARCHAR(200)  NOT NULL,
        [fixed_standard]  NVARCHAR(100)  NULL,
        [fixed_min]       DECIMAL(18,4)  NULL,
        [fixed_max]       DECIMAL(18,4)  NULL,
        [sort_order]      INT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_cos_check_sub_rows] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_check_sub_rows_item] FOREIGN KEY ([check_item_id])
            REFERENCES [dbo].[cos_check_items]([id]) ON DELETE CASCADE
    );
    PRINT 'Created: cos_check_sub_rows';
END
GO

-- ── 1.4 Battery Types ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_battery_types')
BEGIN
    CREATE TABLE [dbo].[cos_battery_types] (
        [id]      INT            IDENTITY(1,1) NOT NULL,
        [name]    NVARCHAR(100)  NOT NULL,
        [kat_id]  INT            NULL,          -- optional FK ke tlkp_kategori
        CONSTRAINT [PK_cos_battery_types] PRIMARY KEY CLUSTERED ([id])
    );
    PRINT 'Created: cos_battery_types';
END
GO

-- ── 1.5 Battery Standards ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_battery_standards')
BEGIN
    CREATE TABLE [dbo].[cos_battery_standards] (
        [id]               INT            IDENTITY(1,1) NOT NULL,
        [battery_type_id]  INT            NOT NULL,
        [param_key]        NVARCHAR(100)  NOT NULL,
        [value]            NVARCHAR(100)  NOT NULL,
        [min_value]        DECIMAL(18,4)  NULL,
        [max_value]        DECIMAL(18,4)  NULL,
        CONSTRAINT [PK_cos_battery_standards] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_battery_standards_type] FOREIGN KEY ([battery_type_id])
            REFERENCES [dbo].[cos_battery_types]([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_cos_battery_standards_type_key] UNIQUE ([battery_type_id], [param_key])
    );
    PRINT 'Created: cos_battery_standards';
END
GO

-- ── 1.6 Problem Columns ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_problem_columns')
BEGIN
    CREATE TABLE [dbo].[cos_problem_columns] (
        [id]          INT            IDENTITY(1,1) NOT NULL,
        [form_id]     INT            NOT NULL,
        [column_key]  NVARCHAR(50)   NOT NULL,
        [label]       NVARCHAR(100)  NOT NULL,
        [field_type]  NVARCHAR(30)   NOT NULL,
        [width]       NVARCHAR(20)   NOT NULL,
        [sort_order]  INT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_cos_problem_columns] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_problem_columns_form] FOREIGN KEY ([form_id])
            REFERENCES [dbo].[cos_form_definitions]([id]) ON DELETE CASCADE
    );
    PRINT 'Created: cos_problem_columns';
END
GO

-- ── 1.7 Signature Slots ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_signature_slots')
BEGIN
    CREATE TABLE [dbo].[cos_signature_slots] (
        [id]          INT            IDENTITY(1,1) NOT NULL,
        [form_id]     INT            NOT NULL,
        [role_key]    NVARCHAR(50)   NOT NULL,
        [label]       NVARCHAR(100)  NOT NULL,
        [sort_order]  INT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_cos_signature_slots] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_signature_slots_form] FOREIGN KEY ([form_id])
            REFERENCES [dbo].[cos_form_definitions]([id]) ON DELETE CASCADE
    );
    PRINT 'Created: cos_signature_slots';
END
GO

-- ── 1.8 Submissions ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_submissions')
BEGIN
    CREATE TABLE [dbo].[cos_submissions] (
        [id]               INT            IDENTITY(1,1) NOT NULL,
        [form_id]          INT            NOT NULL,
        [tanggal]          DATETIME2      NOT NULL,
        [line_id]          INT            NULL,
        [shift_id]         INT            NULL,
        [operator_emp_id]  NVARCHAR(50)   NULL,
        [leader_emp_id]    NVARCHAR(50)   NULL,
        [kasubsie_emp_id]  NVARCHAR(50)   NULL,
        [kasie_emp_id]     NVARCHAR(50)   NULL,
        [battery_slots_json] NVARCHAR(MAX) NULL,
        [created_at]       DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        [updated_at]       DATETIME2      NULL,
        CONSTRAINT [PK_cos_submissions] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_submissions_form] FOREIGN KEY ([form_id])
            REFERENCES [dbo].[cos_form_definitions]([id])
    );
    PRINT 'Created: cos_submissions';
END
GO

-- ── 1.9 Check Values ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_check_values')
BEGIN
    CREATE TABLE [dbo].[cos_check_values] (
        [id]             INT            IDENTITY(1,1) NOT NULL,
        [submission_id]  INT            NOT NULL,
        [setting_key]    NVARCHAR(200)  NOT NULL,
        [value]          NVARCHAR(500)  NULL,
        CONSTRAINT [PK_cos_check_values] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_check_values_sub] FOREIGN KEY ([submission_id])
            REFERENCES [dbo].[cos_submissions]([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_cos_check_values_sub_key] UNIQUE ([submission_id], [setting_key])
    );
    PRINT 'Created: cos_check_values';
END
GO

-- ── 1.10 Problems ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_problems')
BEGIN
    CREATE TABLE [dbo].[cos_problems] (
        [id]             INT            IDENTITY(1,1) NOT NULL,
        [submission_id]  INT            NOT NULL,
        [sort_order]     INT            NOT NULL DEFAULT 0,
        [values_json]    NVARCHAR(MAX)  NULL,
        CONSTRAINT [PK_cos_problems] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_problems_sub] FOREIGN KEY ([submission_id])
            REFERENCES [dbo].[cos_submissions]([id]) ON DELETE CASCADE
    );
    PRINT 'Created: cos_problems';
END
GO

-- ── 1.11 Signature Entries ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'cos_signature_entries')
BEGIN
    CREATE TABLE [dbo].[cos_signature_entries] (
        [id]              INT            IDENTITY(1,1) NOT NULL,
        [submission_id]   INT            NOT NULL,
        [role_key]        NVARCHAR(50)   NOT NULL,
        [signature_data]  NVARCHAR(MAX)  NULL,
        CONSTRAINT [PK_cos_signature_entries] PRIMARY KEY CLUSTERED ([id]),
        CONSTRAINT [FK_cos_signature_entries_sub] FOREIGN KEY ([submission_id])
            REFERENCES [dbo].[cos_submissions]([id]) ON DELETE CASCADE,
        CONSTRAINT [UQ_cos_signature_entries_sub_role] UNIQUE ([submission_id], [role_key])
    );
    PRINT 'Created: cos_signature_entries';
END
GO

-- ── 1.12 EF Migration History (supaya EF tahu tabel sudah ada) ──
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory')
BEGIN
    CREATE TABLE [dbo].[__EFMigrationsHistory] (
        [MigrationId]    NVARCHAR(150)  NOT NULL,
        [ProductVersion] NVARCHAR(32)   NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED ([MigrationId])
    );
    PRINT 'Created: __EFMigrationsHistory';
END
GO


-- ┌─────────────────────────────────────────────────────────────┐
-- │  2. SEED DATA                                               │
-- └─────────────────────────────────────────────────────────────┘

-- ══════════════════════════════════════════════════
-- 2.1 Form Definition
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_form_definitions] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_form_definitions] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_form_definitions] ([id], [code], [title], [subtitle], [slot_count], [is_active], [created_at])
    VALUES (1, N'COS_VALIDATION', N'VALIDASI PROSES COS', N'Form-A2 1-K.051-5-2', 3, 1, '2025-01-01T00:00:00');
END

SET IDENTITY_INSERT [dbo].[cos_form_definitions] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.2 Battery Types
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_battery_types] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_battery_types] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_battery_types] ([id], [name]) VALUES
        (1, N'NS40ZL'),
        (2, N'NS60L'),
        (3, N'34B19LS'),
        (4, N'N50Z'),
        (5, N'N70Z'),
        (6, N'34B19LS OE TYT');
END

SET IDENTITY_INSERT [dbo].[cos_battery_types] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.3 Battery Standards (96 rows)
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_battery_standards] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_battery_standards] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_battery_standards]
        ([id], [battery_type_id], [param_key], [value], [min_value], [max_value])
    VALUES
        -- pourWait
        ( 1, 1, N'pourWait',          N'-',             NULL,     NULL),
        ( 2, 2, N'pourWait',          N'-',             NULL,     NULL),
        ( 3, 3, N'pourWait',          N'1.0 - 2.0',    1.0000,   2.0000),
        ( 4, 4, N'pourWait',          N'-',             NULL,     NULL),
        ( 5, 5, N'pourWait',          N'-',             NULL,     NULL),
        ( 6, 6, N'pourWait',          N'1.0 - 2.0',    1.0000,   2.0000),
        -- pourTime
        ( 7, 1, N'pourTime',          N'',              NULL,     NULL),
        ( 8, 2, N'pourTime',          N'2.5 - 4.0',    2.5000,   4.0000),
        ( 9, 3, N'pourTime',          N'1.5 - 3.0',    1.5000,   3.0000),
        (10, 4, N'pourTime',          N'3.0 - 5.0',    3.0000,   5.0000),
        (11, 5, N'pourTime',          N'3.5 - 5.5',    3.5000,   5.5000),
        (12, 6, N'pourTime',          N'1.5 - 3.0',    1.5000,   3.0000),
        -- dipTime2
        (13, 1, N'dipTime2',          N'',              NULL,     NULL),
        (14, 2, N'dipTime2',          N'2.0 - 3.5',    2.0000,   3.5000),
        (15, 3, N'dipTime2',          N'1.0 - 2.5',    1.0000,   2.5000),
        (16, 4, N'dipTime2',          N'2.5 - 4.0',    2.5000,   4.0000),
        (17, 5, N'dipTime2',          N'3.0 - 4.5',    3.0000,   4.5000),
        (18, 6, N'dipTime2',          N'1.0 - 2.5',    1.0000,   2.5000),
        -- dumpTime
        (19, 1, N'dumpTime',          N'',              NULL,     NULL),
        (20, 2, N'dumpTime',          N'1.5 - 3.0',    1.5000,   3.0000),
        (21, 3, N'dumpTime',          N'0.8 - 2.0',    0.8000,   2.0000),
        (22, 4, N'dumpTime',          N'2.0 - 3.5',    2.0000,   3.5000),
        (23, 5, N'dumpTime',          N'2.5 - 4.0',    2.5000,   4.0000),
        (24, 6, N'dumpTime',          N'0.8 - 2.0',    0.8000,   2.0000),
        -- lugDryTime
        (25, 1, N'lugDryTime',        N'',              NULL,     NULL),
        (26, 2, N'lugDryTime',        N'3.5 - 5.5',    3.5000,   5.5000),
        (27, 3, N'lugDryTime',        N'2.5 - 4.5',    2.5000,   4.5000),
        (28, 4, N'lugDryTime',        N'4.0 - 6.0',    4.0000,   6.0000),
        (29, 5, N'lugDryTime',        N'4.5 - 6.5',    4.5000,   6.5000),
        (30, 6, N'lugDryTime',        N'2.5 - 4.5',    2.5000,   4.5000),
        -- largeVibratorTime
        (31, 1, N'largeVibratorTime', N'',              NULL,     NULL),
        (32, 2, N'largeVibratorTime', N'1.5 - 3.5',    1.5000,   3.5000),
        (33, 3, N'largeVibratorTime', N'0.8 - 2.5',    0.8000,   2.5000),
        (34, 4, N'largeVibratorTime', N'2.0 - 4.0',    2.0000,   4.0000),
        (35, 5, N'largeVibratorTime', N'2.5 - 4.5',    2.5000,   4.5000),
        (36, 6, N'largeVibratorTime', N'0.8 - 2.5',    0.8000,   2.5000),
        -- smallVibratorTime
        (37, 1, N'smallVibratorTime', N'',              NULL,     NULL),
        (38, 2, N'smallVibratorTime', N'1.5 - 3.5',    1.5000,   3.5000),
        (39, 3, N'smallVibratorTime', N'0.8 - 2.5',    0.8000,   2.5000),
        (40, 4, N'smallVibratorTime', N'2.0 - 4.0',    2.0000,   4.0000),
        (41, 5, N'smallVibratorTime', N'2.5 - 4.5',    2.5000,   4.5000),
        (42, 6, N'smallVibratorTime', N'0.8 - 2.5',    0.8000,   2.5000),
        -- coolingTime
        (43, 1, N'coolingTime',       N'',              NULL,     NULL),
        (44, 2, N'coolingTime',       N'25 - 35',       25.0000,  35.0000),
        (45, 3, N'coolingTime',       N'18 - 28',       18.0000,  28.0000),
        (46, 4, N'coolingTime',       N'28 - 38',       28.0000,  38.0000),
        (47, 5, N'coolingTime',       N'30 - 42',       30.0000,  42.0000),
        (48, 6, N'coolingTime',       N'18 - 28',       18.0000,  28.0000),
        -- leadPumpSpeed
        (49, 1, N'leadPumpSpeed',     N'',              NULL,     NULL),
        (50, 2, N'leadPumpSpeed',     N'45 - 65',       45.0000,  65.0000),
        (51, 3, N'leadPumpSpeed',     N'35 - 55',       35.0000,  55.0000),
        (52, 4, N'leadPumpSpeed',     N'50 - 70',       50.0000,  70.0000),
        (53, 5, N'leadPumpSpeed',     N'55 - 75',       55.0000,  75.0000),
        (54, 6, N'leadPumpSpeed',     N'35 - 55',       35.0000,  55.0000),
        -- tempAirDryer
        (55, 1, N'tempAirDryer',      N'300 - 400',     300.0000, 400.0000),
        (56, 2, N'tempAirDryer',      N'310 - 410',     310.0000, 410.0000),
        (57, 3, N'tempAirDryer',      N'290 - 390',     290.0000, 390.0000),
        (58, 4, N'tempAirDryer',      N'320 - 420',     320.0000, 420.0000),
        (59, 5, N'tempAirDryer',      N'330 - 430',     330.0000, 430.0000),
        (60, 6, N'tempAirDryer',      N'290 - 390',     290.0000, 390.0000),
        -- tempPot
        (61, 1, N'tempPot',           N'470 - 490',     470.0000, 490.0000),
        (62, 2, N'tempPot',           N'475 - 495',     475.0000, 495.0000),
        (63, 3, N'tempPot',           N'465 - 485',     465.0000, 485.0000),
        (64, 4, N'tempPot',           N'480 - 500',     480.0000, 500.0000),
        (65, 5, N'tempPot',           N'485 - 505',     485.0000, 505.0000),
        (66, 6, N'tempPot',           N'465 - 485',     465.0000, 485.0000),
        -- tempPipe
        (67, 1, N'tempPipe',          N'410 - 430',     410.0000, 430.0000),
        (68, 2, N'tempPipe',          N'415 - 435',     415.0000, 435.0000),
        (69, 3, N'tempPipe',          N'405 - 425',     405.0000, 425.0000),
        (70, 4, N'tempPipe',          N'420 - 440',     420.0000, 440.0000),
        (71, 5, N'tempPipe',          N'425 - 445',     425.0000, 445.0000),
        (72, 6, N'tempPipe',          N'405 - 425',     405.0000, 425.0000),
        -- tempCrossBlock
        (73, 1, N'tempCrossBlock',    N'390 - 410',     390.0000, 410.0000),
        (74, 2, N'tempCrossBlock',    N'395 - 415',     395.0000, 415.0000),
        (75, 3, N'tempCrossBlock',    N'385 - 405',     385.0000, 405.0000),
        (76, 4, N'tempCrossBlock',    N'400 - 420',     400.0000, 420.0000),
        (77, 5, N'tempCrossBlock',    N'405 - 425',     405.0000, 425.0000),
        (78, 6, N'tempCrossBlock',    N'385 - 405',     385.0000, 405.0000),
        -- tempElbow
        (79, 1, N'tempElbow',         N'370 - 390',     370.0000, 390.0000),
        (80, 2, N'tempElbow',         N'375 - 395',     375.0000, 395.0000),
        (81, 3, N'tempElbow',         N'365 - 385',     365.0000, 385.0000),
        (82, 4, N'tempElbow',         N'380 - 400',     380.0000, 400.0000),
        (83, 5, N'tempElbow',         N'385 - 405',     385.0000, 405.0000),
        (84, 6, N'tempElbow',         N'365 - 385',     365.0000, 385.0000),
        -- tempMold
        (85, 1, N'tempMold',          N'160 - 190',     160.0000, 190.0000),
        (86, 2, N'tempMold',          N'165 - 195',     165.0000, 195.0000),
        (87, 3, N'tempMold',          N'155 - 185',     155.0000, 185.0000),
        (88, 4, N'tempMold',          N'170 - 200',     170.0000, 200.0000),
        (89, 5, N'tempMold',          N'175 - 205',     175.0000, 205.0000),
        (90, 6, N'tempMold',          N'155 - 185',     155.0000, 185.0000),
        -- coolingFlowRate
        (91, 1, N'coolingFlowRate',   N'6 - 10',        6.0000,   10.0000),
        (92, 2, N'coolingFlowRate',   N'7 - 11',        7.0000,   11.0000),
        (93, 3, N'coolingFlowRate',   N'5 - 9',         5.0000,   9.0000),
        (94, 4, N'coolingFlowRate',   N'8 - 12',        8.0000,   12.0000),
        (95, 5, N'coolingFlowRate',   N'9 - 13',        9.0000,   13.0000),
        (96, 6, N'coolingFlowRate',   N'5 - 9',         5.0000,   9.0000);
END

SET IDENTITY_INSERT [dbo].[cos_battery_standards] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.4 Check Items (32 rows)
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_check_items] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_check_items] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_check_items]
        ([id], [form_id], [item_key], [label], [type], [visual_standard], [numeric_std_key], [fixed_standard], [fixed_min], [fixed_max], [frequency], [keterangan], [conditional_label], [sort_order])
    VALUES
        ( 1, 1, N'kekuatanCastingStrap',  N'Kekuatan Casting Strap',                     N'visual',  N'Ditarik tidak lepas',          NULL,                    NULL,              NULL,     NULL,     N'1 batt / shift / ganti type', NULL,                             NULL,              1),
        ( 2, 1, N'meniscus',              N'Meniscus',                                    N'visual',  N'Positif',                      NULL,                    NULL,              NULL,     NULL,     N'1 batt / shift / ganti type', NULL,                             NULL,              2),
        ( 3, 1, N'hasilCastingStrap',     N'Hasil Casting Strap',                         N'visual',  N'Tidak ada flash',              NULL,                    NULL,              NULL,     NULL,     N'1 Batt / shift / ganti type', NULL,                             NULL,              3),
        ( 4, 1, N'levelFlux',             N'Level Flux',                                  N'visual',  N'Terisi Flux',                  NULL,                    NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,              4),
        ( 5, 1, N'pourWait',              N'Pour Wait (Khusus Line 8)',                   N'numeric', NULL,                            N'pourWait',             NULL,              NULL,     NULL,     N'2 x / Shift / ganti type',    NULL,                             N'Khusus Line 8',  5),
        ( 6, 1, N'pourTime',              N'Pour Time',                                   N'numeric', NULL,                            N'pourTime',             NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,              6),
        ( 7, 1, N'dipTime2',              N'Dip Time 2',                                  N'numeric', NULL,                            N'dipTime2',             NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,              7),
        ( 8, 1, N'dumpTime',              N'Dump Time (Drain back time)',                 N'numeric', NULL,                            N'dumpTime',             NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,              8),
        ( 9, 1, N'lugDryTime',            N'Lug Dry Time',                                N'numeric', NULL,                            N'lugDryTime',           NULL,              NULL,     NULL,     N'2 x / Shift / ganti type',    N'untuk 34B19LS OE TYT',          NULL,              9),
        (10, 1, N'largeVibratorTime',     N'Large Vibrator Time',                         N'numeric', NULL,                            N'largeVibratorTime',    NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,             10),
        (11, 1, N'smallVibratorTime',     N'Small Vibrator Time',                         N'numeric', NULL,                            N'smallVibratorTime',    NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,             11),
        (12, 1, N'coolingTime',           N'Cooling Time',                                N'numeric', NULL,                            N'coolingTime',          NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,             12),
        (13, 1, N'leadPumpSpeed',         N'Lead Pump Speed',                             N'numeric', NULL,                            N'leadPumpSpeed',        NULL,              NULL,     NULL,     NULL,                           NULL,                             NULL,             13),
        (14, 1, N'checkAlignment',        N'Check Alignment',                             N'visual',  N'Bergerak',                     NULL,                    NULL,              NULL,     NULL,     N'1 x / shift',                 NULL,                             NULL,             14),
        (15, 1, N'checkDatumTable',       N'Check Datum Table Alignment',                 N'visual',  N'Bersih',                       NULL,                    NULL,              NULL,     NULL,     N'1 x / shift',                 N'Tidak ada ceceran pasta',       NULL,             15),
        (16, 1, N'cleaningNozzle',        N'Cleaning of Nozzle Lug Dry',                  N'visual',  N'Bersih',                       NULL,                    NULL,              NULL,     NULL,     N'1 x / shift',                 N'Spray dengan udara',            NULL,             16),
        (17, 1, N'tempAirNozzleLugDry',   N'Temp Air Nozzle Lug Dry',                    N'numeric', NULL,                            NULL,                    N'> 275° C',       275.0000, NULL,     N'2 x / shift',                 N'Cek dgn Thermocouple',          NULL,             17),
        (18, 1, N'tempAirDryer',          N'Temp Air Dryer (hot air)',                    N'numeric', NULL,                            N'tempAirDryer',         NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             18),
        (19, 1, N'blowerPipeTemp',        N'Blower Pipe Temp (Khusus Line 7)',            N'numeric', NULL,                            NULL,                    N'> 300° C',       300.0000, NULL,     N'2 x / shift',                 NULL,                             N'Khusus Line 7', 19),
        (20, 1, N'blowerNozzle1Temp',     N'Blower Nozzle 1 Temp (Khusus Line 7)',        N'numeric', NULL,                            NULL,                    N'> 200° C',       200.0000, NULL,     N'2 x / shift',                 NULL,                             N'Khusus Line 7', 20),
        (21, 1, N'blowerNozzle2Temp',     N'Blower Nozzle 2 Temp (Khusus Line 7)',        N'numeric', NULL,                            NULL,                    N'> 200° C',       200.0000, NULL,     N'2 x / shift',                 NULL,                             N'Khusus Line 7', 21),
        (22, 1, N'tempPot',               N'Temperatur Pot',                              N'numeric', NULL,                            N'tempPot',              NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             22),
        (23, 1, N'tempPipe',              N'Temperatur Pipe',                             N'numeric', NULL,                            N'tempPipe',             NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             23),
        (24, 1, N'tempCrossBlock',        N'Temp. Cross Block',                           N'numeric', NULL,                            N'tempCrossBlock',       NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             24),
        (25, 1, N'tempElbow',             N'Temp. Elbow (Lead Lump)',                     N'numeric', NULL,                            N'tempElbow',            NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             25),
        (26, 1, N'tempMold',              N'Temperatur Mold',                             N'numeric', NULL,                            N'tempMold',             NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             26),
        (27, 1, N'coolingFlowRate',       N'Cooling Water Flow Rate',                     N'numeric', NULL,                            N'coolingFlowRate',      NULL,              NULL,     NULL,     N'2 x / shift',                 NULL,                             NULL,             27),
        (28, 1, N'coolingWaterTemp',      N'Cooling Water Temperature',                   N'numeric', NULL,                            NULL,                    N'28 ± 2 °C',     26.0000,  30.0000,  N'2 x / shift',                 NULL,                             NULL,             28),
        (29, 1, N'sprueBrush',            N'Sprue Brush',                                 N'visual',  N'Berfungsi dengan baik',        NULL,                    NULL,              NULL,     NULL,     N'2 x / Shift',                 NULL,                             NULL,             29),
        (30, 1, N'cleaningCavityMold',    N'Cleaning Cavity Mold COS',                    N'visual',  N'Tidak tersumbat dross',        NULL,                    NULL,              NULL,     NULL,     N'3 x / Shift',                 NULL,                             NULL,             30),
        (31, 1, N'fluxTime',              N'Flux Time',                                   N'numeric', NULL,                            NULL,                    NULL,              NULL,     NULL,     N'1 batt / shift / ganti type', NULL,                             NULL,             31),
        (32, 1, N'overflowHydrazine',     N'Overflow Hydrazine',                          N'numeric', NULL,                            NULL,                    NULL,              NULL,     NULL,     N'1 batt / shift / ganti type', NULL,                             NULL,             32);
END

SET IDENTITY_INSERT [dbo].[cos_check_items] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.5 Check Sub Rows (16 rows)
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_check_sub_rows] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_check_sub_rows] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_check_sub_rows]
        ([id], [check_item_id], [suffix], [label], [fixed_standard], [fixed_min], [fixed_max], [sort_order])
    VALUES
        ( 1,  1, N'plus',      N'+',               NULL,               NULL,     NULL,     1),
        ( 2,  1, N'minus',     N'−',               NULL,               NULL,     NULL,     2),
        ( 3,  2, N'plus',      N'+',               NULL,               NULL,     NULL,     1),
        ( 4,  2, N'minus',     N'−',               NULL,               NULL,     NULL,     2),
        ( 5, 23, N'L',         N'L',               NULL,               NULL,     NULL,     1),
        ( 6, 23, N'R',         N'R',               NULL,               NULL,     NULL,     2),
        ( 7, 26, N'mold1',     N'Mold 1',          NULL,               NULL,     NULL,     1),
        ( 8, 26, N'mold2',     N'Mold 2',          NULL,               NULL,     NULL,     2),
        ( 9, 26, N'post1',     N'Post 1',          NULL,               NULL,     NULL,     3),
        (10, 26, N'post2',     N'Post 2',          NULL,               NULL,     NULL,     4),
        (11, 27, N'mold1',     N'Mold 1',          NULL,               NULL,     NULL,     1),
        (12, 27, N'mold2',     N'Mold 2',          NULL,               NULL,     NULL,     2),
        (13, 31, N'line6',     N'Line 6',          N'1 - 3 detik',     1.0000,   3.0000,   1),
        (14, 31, N'lineOther', N'Line 2,3,4,5,7&8',N'0.1 - 1 detik',  0.1000,   1.0000,   2),
        (15, 32, N'line2',     N'Line 2',          N'10 detik',        10.0000,  10.0000,  1),
        (16, 32, N'line7',     N'Line 7',          N'5 detik',         5.0000,   5.0000,   2);
END

SET IDENTITY_INSERT [dbo].[cos_check_sub_rows] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.6 Problem Columns (6 rows)
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_problem_columns] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_problem_columns] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_problem_columns]
        ([id], [form_id], [column_key], [label], [field_type], [width], [sort_order])
    VALUES
        (1, 1, N'item',     N'ITEM',     N'text',   N'120px', 1),
        (2, 1, N'masalah',  N'MASALAH',  N'text',   N'200px', 2),
        (3, 1, N'tindakan', N'TINDAKAN', N'text',   N'200px', 3),
        (4, 1, N'waktu',    N'WAKTU',    N'text',   N'80px',  4),
        (5, 1, N'menit',    N'MENIT',    N'number', N'60px',  5),
        (6, 1, N'pic',      N'PIC',      N'text',   N'100px', 6);
END

SET IDENTITY_INSERT [dbo].[cos_problem_columns] OFF;
GO

-- ══════════════════════════════════════════════════
-- 2.7 Signature Slots (4 rows)
-- ══════════════════════════════════════════════════
SET IDENTITY_INSERT [dbo].[cos_signature_slots] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[cos_signature_slots] WHERE [id] = 1)
BEGIN
    INSERT INTO [dbo].[cos_signature_slots]
        ([id], [form_id], [role_key], [label], [sort_order])
    VALUES
        (1, 1, N'operator',  N'Dibuat',     1),
        (2, 1, N'leader',    N'Diperiksa',  2),
        (3, 1, N'kasubsie',  N'Diketahui',  3),
        (4, 1, N'kasie',     N'Disetujui',  4);
END

SET IDENTITY_INSERT [dbo].[cos_signature_slots] OFF;
GO

-- ══════════════════════════════════════════════════
-- DONE
-- ══════════════════════════════════════════════════
PRINT '';
PRINT '══════════════════════════════════════════════════';
PRINT '  COS Validation tables created successfully!';
PRINT '  11 tables + seed data (1 form, 6 battery types,';
PRINT '  96 standards, 32 check items, 16 sub rows,';
PRINT '  6 problem columns, 4 signature slots)';
PRINT '══════════════════════════════════════════════════';
GO
