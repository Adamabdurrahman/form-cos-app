-- ================================================================================
-- MIGRATION SCRIPT: Split db_master_data → db_master_data + db_cos_checksheet
-- Generated   : 2026-02-16 13:18
-- Blueprint   : klarifikasi.txt (APPROVED)
-- Server      : ADAM123\SQLEXPRESS
-- Auth        : sa / 07Mei2005
-- ================================================================================
--
-- INSTRUKSI EKSEKUSI:
--   1. BACKUP dulu: BACKUP DATABASE db_master_data TO DISK = 'C:\backup\db_master_data_before_split.bak'
--   2. Jalankan script ini di SSMS (connect sebagai sa ke ADAM123\SQLEXPRESS)
--   3. Jalankan SECTION 1 dulu, pastikan database terbuat
--   4. Jalankan SECTION 2 untuk copy data
--   5. VERIFIKASI row counts (SECTION 3)
--   6. JANGAN jalankan SECTION 4 (DROP) sampai 100% yakin data aman
--
-- ================================================================================


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 1: CREATE DATABASE
-- ████████████████████████████████████████████████████████████████████████████████

USE [master];
GO

-- Buat database baru jika belum ada
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'db_cos_checksheet')
BEGIN
    CREATE DATABASE [db_cos_checksheet];
    PRINT '✅ Database db_cos_checksheet CREATED.';
END
ELSE
BEGIN
    PRINT '⚠️  Database db_cos_checksheet sudah ada, SKIP create.';
END
GO

USE [db_cos_checksheet];
GO


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 2A: COPY DATA — GROUP B (Master/Lookup → prefix tlkp_cos_)
-- ████████████████████████████████████████████████████████████████████████████████
-- Tabel-tabel ini berisi data REFERENSI/STATIS yang jarang berubah.
-- Menggunakan SELECT * INTO untuk copy schema + data sekaligus.
-- IDENTITY kolom akan otomatis terbawa.

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 2A: Copying GROUP B tables (Master/Lookup)';
PRINT '══════════════════════════════════════════════════════';

-- 1. cos_form_definitions → tlkp_cos_form_definitions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_form_definitions')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_form_definitions]
    FROM [db_master_data].[dbo].[cos_form_definitions];
    PRINT '✅ [1/8] tlkp_cos_form_definitions — copied.';
END
ELSE PRINT '⚠️  [1/8] tlkp_cos_form_definitions — already exists, SKIP.';

-- 2. cos_check_items → tlkp_cos_check_items
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_check_items')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_check_items]
    FROM [db_master_data].[dbo].[cos_check_items];
    PRINT '✅ [2/8] tlkp_cos_check_items — copied.';
END
ELSE PRINT '⚠️  [2/8] tlkp_cos_check_items — already exists, SKIP.';

-- 3. cos_check_sub_rows → tlkp_cos_check_sub_rows
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_check_sub_rows')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_check_sub_rows]
    FROM [db_master_data].[dbo].[cos_check_sub_rows];
    PRINT '✅ [3/8] tlkp_cos_check_sub_rows — copied.';
END
ELSE PRINT '⚠️  [3/8] tlkp_cos_check_sub_rows — already exists, SKIP.';

-- 4. cos_battery_types → tlkp_cos_battery_types
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_battery_types')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_battery_types]
    FROM [db_master_data].[dbo].[cos_battery_types];
    PRINT '✅ [4/8] tlkp_cos_battery_types — copied.';
END
ELSE PRINT '⚠️  [4/8] tlkp_cos_battery_types — already exists, SKIP.';

-- 5. cos_battery_standards → tlkp_cos_battery_standards
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_battery_standards')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_battery_standards]
    FROM [db_master_data].[dbo].[cos_battery_standards];
    PRINT '✅ [5/8] tlkp_cos_battery_standards — copied.';
END
ELSE PRINT '⚠️  [5/8] tlkp_cos_battery_standards — already exists, SKIP.';

-- 6. cos_problem_columns → tlkp_cos_problem_columns
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_problem_columns')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_problem_columns]
    FROM [db_master_data].[dbo].[cos_problem_columns];
    PRINT '✅ [6/8] tlkp_cos_problem_columns — copied.';
END
ELSE PRINT '⚠️  [6/8] tlkp_cos_problem_columns — already exists, SKIP.';

-- 7. cos_signature_slots → tlkp_cos_signature_slots
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_signature_slots')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_signature_slots]
    FROM [db_master_data].[dbo].[cos_signature_slots];
    PRINT '✅ [7/8] tlkp_cos_signature_slots — copied.';
END
ELSE PRINT '⚠️  [7/8] tlkp_cos_signature_slots — already exists, SKIP.';

-- 8. cos_employee_signatures → tlkp_cos_employee_signatures
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'tlkp_cos_employee_signatures')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[tlkp_cos_employee_signatures]
    FROM [db_master_data].[dbo].[cos_employee_signatures];
    PRINT '✅ [8/8] tlkp_cos_employee_signatures — copied.';
END
ELSE PRINT '⚠️  [8/8] tlkp_cos_employee_signatures — already exists, SKIP.';

PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 2B: COPY DATA — GROUP C (Transaction → prefix t_cos_)
-- ████████████████████████████████████████████████████████████████████████████████
-- Tabel-tabel ini berisi data TRANSAKSI/INPUT USER yang sering berubah.

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 2B: Copying GROUP C tables (Transaction)';
PRINT '══════════════════════════════════════════════════════';

-- 1. cos_submissions → t_cos_submissions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_cos_submissions')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[t_cos_submissions]
    FROM [db_master_data].[dbo].[cos_submissions];
    PRINT '✅ [1/5] t_cos_submissions — copied.';
END
ELSE PRINT '⚠️  [1/5] t_cos_submissions — already exists, SKIP.';

-- 2. cos_check_values → t_cos_check_values
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_cos_check_values')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[t_cos_check_values]
    FROM [db_master_data].[dbo].[cos_check_values];
    PRINT '✅ [2/5] t_cos_check_values — copied.';
END
ELSE PRINT '⚠️  [2/5] t_cos_check_values — already exists, SKIP.';

-- 3. cos_problems → t_cos_problems
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_cos_problems')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[t_cos_problems]
    FROM [db_master_data].[dbo].[cos_problems];
    PRINT '✅ [3/5] t_cos_problems — copied.';
END
ELSE PRINT '⚠️  [3/5] t_cos_problems — already exists, SKIP.';

-- 4. cos_signature_entries → t_cos_signature_entries
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_cos_signature_entries')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[t_cos_signature_entries]
    FROM [db_master_data].[dbo].[cos_signature_entries];
    PRINT '✅ [4/5] t_cos_signature_entries — copied.';
END
ELSE PRINT '⚠️  [4/5] t_cos_signature_entries — already exists, SKIP.';

-- 5. cos_approval_attachments → t_cos_approval_attachments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 't_cos_approval_attachments')
BEGIN
    SELECT * INTO [db_cos_checksheet].[dbo].[t_cos_approval_attachments]
    FROM [db_master_data].[dbo].[cos_approval_attachments];
    PRINT '✅ [5/5] t_cos_approval_attachments — copied.';
END
ELSE PRINT '⚠️  [5/5] t_cos_approval_attachments — already exists, SKIP.';

PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 2C: RECREATE PRIMARY KEYS (SELECT INTO tidak copy PK/Identity)
-- ████████████████████████████████████████████████████████████████████████████████
-- SELECT * INTO hanya copy data + column types, TIDAK copy:
--   - PRIMARY KEY constraints
--   - IDENTITY property
--   - UNIQUE constraints
--   - FOREIGN KEY constraints
--   - INDEX
-- Kita perlu buat ulang PK dan Identity secara manual.

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 2C: Recreating PRIMARY KEYS';
PRINT '══════════════════════════════════════════════════════';

USE [db_cos_checksheet];
GO

-- ── GROUP B: Master Tables ──

-- tlkp_cos_form_definitions
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_form_definitions') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_form_definitions] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_form_definitions] ADD CONSTRAINT PK_tlkp_cos_form_definitions PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_form_definitions.id';
END

-- tlkp_cos_check_items
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_check_items') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_check_items] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_check_items] ADD CONSTRAINT PK_tlkp_cos_check_items PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_check_items.id';
END

-- tlkp_cos_check_sub_rows
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_check_sub_rows') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_check_sub_rows] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_check_sub_rows] ADD CONSTRAINT PK_tlkp_cos_check_sub_rows PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_check_sub_rows.id';
END

-- tlkp_cos_battery_types
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_battery_types') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_battery_types] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_battery_types] ADD CONSTRAINT PK_tlkp_cos_battery_types PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_battery_types.id';
END

-- tlkp_cos_battery_standards
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_battery_standards') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_battery_standards] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_battery_standards] ADD CONSTRAINT PK_tlkp_cos_battery_standards PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_battery_standards.id';
END

-- tlkp_cos_problem_columns
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_problem_columns') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_problem_columns] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_problem_columns] ADD CONSTRAINT PK_tlkp_cos_problem_columns PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_problem_columns.id';
END

-- tlkp_cos_signature_slots
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_signature_slots') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_signature_slots] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_signature_slots] ADD CONSTRAINT PK_tlkp_cos_signature_slots PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_signature_slots.id';
END

-- tlkp_cos_employee_signatures
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('tlkp_cos_employee_signatures') AND type = 'PK')
BEGIN
    ALTER TABLE [tlkp_cos_employee_signatures] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [tlkp_cos_employee_signatures] ADD CONSTRAINT PK_tlkp_cos_employee_signatures PRIMARY KEY ([id]);
    PRINT '✅ PK: tlkp_cos_employee_signatures.id';
END

-- ── GROUP C: Transaction Tables ──

-- t_cos_submissions
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('t_cos_submissions') AND type = 'PK')
BEGIN
    ALTER TABLE [t_cos_submissions] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [t_cos_submissions] ADD CONSTRAINT PK_t_cos_submissions PRIMARY KEY ([id]);
    PRINT '✅ PK: t_cos_submissions.id';
END

-- t_cos_check_values
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('t_cos_check_values') AND type = 'PK')
BEGIN
    ALTER TABLE [t_cos_check_values] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [t_cos_check_values] ADD CONSTRAINT PK_t_cos_check_values PRIMARY KEY ([id]);
    PRINT '✅ PK: t_cos_check_values.id';
END

-- t_cos_problems
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('t_cos_problems') AND type = 'PK')
BEGIN
    ALTER TABLE [t_cos_problems] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [t_cos_problems] ADD CONSTRAINT PK_t_cos_problems PRIMARY KEY ([id]);
    PRINT '✅ PK: t_cos_problems.id';
END

-- t_cos_signature_entries
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('t_cos_signature_entries') AND type = 'PK')
BEGIN
    ALTER TABLE [t_cos_signature_entries] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [t_cos_signature_entries] ADD CONSTRAINT PK_t_cos_signature_entries PRIMARY KEY ([id]);
    PRINT '✅ PK: t_cos_signature_entries.id';
END

-- t_cos_approval_attachments
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('t_cos_approval_attachments') AND type = 'PK')
BEGIN
    ALTER TABLE [t_cos_approval_attachments] ALTER COLUMN [id] INT NOT NULL;
    ALTER TABLE [t_cos_approval_attachments] ADD CONSTRAINT PK_t_cos_approval_attachments PRIMARY KEY ([id]);
    PRINT '✅ PK: t_cos_approval_attachments.id';
END

PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 2D: RECREATE UNIQUE CONSTRAINTS & INDEXES
-- ████████████████████████████████████████████████████████████████████████████████

USE [db_cos_checksheet];
GO

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 2D: Recreating UNIQUE constraints & indexes';
PRINT '══════════════════════════════════════════════════════';

-- Unique: form_definitions.code
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('tlkp_cos_form_definitions') AND name = 'IX_tlkp_cos_form_definitions_code')
BEGIN
    CREATE UNIQUE INDEX IX_tlkp_cos_form_definitions_code ON [tlkp_cos_form_definitions] ([code]);
    PRINT '✅ UNIQUE INDEX: tlkp_cos_form_definitions.code';
END

-- Unique: check_items (form_id + item_key)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('tlkp_cos_check_items') AND name = 'IX_tlkp_cos_check_items_form_item')
BEGIN
    CREATE UNIQUE INDEX IX_tlkp_cos_check_items_form_item ON [tlkp_cos_check_items] ([form_id], [item_key]);
    PRINT '✅ UNIQUE INDEX: tlkp_cos_check_items (form_id, item_key)';
END

-- Unique: battery_standards (battery_type_id + param_key)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('tlkp_cos_battery_standards') AND name = 'IX_tlkp_cos_battery_standards_type_param')
BEGIN
    CREATE UNIQUE INDEX IX_tlkp_cos_battery_standards_type_param ON [tlkp_cos_battery_standards] ([battery_type_id], [param_key]);
    PRINT '✅ UNIQUE INDEX: tlkp_cos_battery_standards (battery_type_id, param_key)';
END

-- Unique: check_values (submission_id + setting_key)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('t_cos_check_values') AND name = 'IX_t_cos_check_values_sub_key')
BEGIN
    CREATE UNIQUE INDEX IX_t_cos_check_values_sub_key ON [t_cos_check_values] ([submission_id], [setting_key]);
    PRINT '✅ UNIQUE INDEX: t_cos_check_values (submission_id, setting_key)';
END

-- Unique: signature_entries (submission_id + role_key)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('t_cos_signature_entries') AND name = 'IX_t_cos_signature_entries_sub_role')
BEGIN
    CREATE UNIQUE INDEX IX_t_cos_signature_entries_sub_role ON [t_cos_signature_entries] ([submission_id], [role_key]);
    PRINT '✅ UNIQUE INDEX: t_cos_signature_entries (submission_id, role_key)';
END

-- Unique: employee_signatures (emp_id)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID('tlkp_cos_employee_signatures') AND name = 'IX_tlkp_cos_employee_signatures_emp')
BEGIN
    CREATE UNIQUE INDEX IX_tlkp_cos_employee_signatures_emp ON [tlkp_cos_employee_signatures] ([emp_id]);
    PRINT '✅ UNIQUE INDEX: tlkp_cos_employee_signatures (emp_id)';
END

PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 2E: RECREATE FOREIGN KEYS (within db_cos_checksheet only)
-- ████████████████████████████████████████████████████████████████████████████████
-- Cross-database FK (ke db_master_data) TIDAK dibuat — dihandle di app level.

USE [db_cos_checksheet];
GO

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 2E: Recreating FOREIGN KEYS (intra-DB only)';
PRINT '══════════════════════════════════════════════════════';

-- check_items.form_id → form_definitions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_check_items_form')
BEGIN
    ALTER TABLE [tlkp_cos_check_items]
        ADD CONSTRAINT FK_check_items_form
        FOREIGN KEY ([form_id]) REFERENCES [tlkp_cos_form_definitions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: check_items.form_id → form_definitions.id (CASCADE)';
END

-- check_sub_rows.check_item_id → check_items.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_sub_rows_check_item')
BEGIN
    ALTER TABLE [tlkp_cos_check_sub_rows]
        ADD CONSTRAINT FK_sub_rows_check_item
        FOREIGN KEY ([check_item_id]) REFERENCES [tlkp_cos_check_items]([id]);
    PRINT '✅ FK: check_sub_rows.check_item_id → check_items.id';
END

-- battery_types.form_id → form_definitions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_battery_types_form')
BEGIN
    ALTER TABLE [tlkp_cos_battery_types]
        ADD CONSTRAINT FK_battery_types_form
        FOREIGN KEY ([form_id]) REFERENCES [tlkp_cos_form_definitions]([id]);
    PRINT '✅ FK: battery_types.form_id → form_definitions.id';
END

-- battery_standards.battery_type_id → battery_types.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_battery_standards_type')
BEGIN
    ALTER TABLE [tlkp_cos_battery_standards]
        ADD CONSTRAINT FK_battery_standards_type
        FOREIGN KEY ([battery_type_id]) REFERENCES [tlkp_cos_battery_types]([id]);
    PRINT '✅ FK: battery_standards.battery_type_id → battery_types.id';
END

-- problem_columns.form_id → form_definitions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_problem_columns_form')
BEGIN
    ALTER TABLE [tlkp_cos_problem_columns]
        ADD CONSTRAINT FK_problem_columns_form
        FOREIGN KEY ([form_id]) REFERENCES [tlkp_cos_form_definitions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: problem_columns.form_id → form_definitions.id (CASCADE)';
END

-- signature_slots.form_id → form_definitions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_signature_slots_form')
BEGIN
    ALTER TABLE [tlkp_cos_signature_slots]
        ADD CONSTRAINT FK_signature_slots_form
        FOREIGN KEY ([form_id]) REFERENCES [tlkp_cos_form_definitions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: signature_slots.form_id → form_definitions.id (CASCADE)';
END

-- submissions.form_id → form_definitions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_submissions_form')
BEGIN
    ALTER TABLE [t_cos_submissions]
        ADD CONSTRAINT FK_submissions_form
        FOREIGN KEY ([form_id]) REFERENCES [tlkp_cos_form_definitions]([id]);
    PRINT '✅ FK: submissions.form_id → form_definitions.id';
END

-- check_values.submission_id → submissions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_check_values_submission')
BEGIN
    ALTER TABLE [t_cos_check_values]
        ADD CONSTRAINT FK_check_values_submission
        FOREIGN KEY ([submission_id]) REFERENCES [t_cos_submissions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: check_values.submission_id → submissions.id (CASCADE)';
END

-- problems.submission_id → submissions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_problems_submission')
BEGIN
    ALTER TABLE [t_cos_problems]
        ADD CONSTRAINT FK_problems_submission
        FOREIGN KEY ([submission_id]) REFERENCES [t_cos_submissions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: problems.submission_id → submissions.id (CASCADE)';
END

-- signature_entries.submission_id → submissions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_signature_entries_submission')
BEGIN
    ALTER TABLE [t_cos_signature_entries]
        ADD CONSTRAINT FK_signature_entries_submission
        FOREIGN KEY ([submission_id]) REFERENCES [t_cos_submissions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: signature_entries.submission_id → submissions.id (CASCADE)';
END

-- approval_attachments.submission_id → submissions.id
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_approval_attachments_submission')
BEGIN
    ALTER TABLE [t_cos_approval_attachments]
        ADD CONSTRAINT FK_approval_attachments_submission
        FOREIGN KEY ([submission_id]) REFERENCES [t_cos_submissions]([id]) ON DELETE CASCADE;
    PRINT '✅ FK: approval_attachments.submission_id → submissions.id (CASCADE)';
END

PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 3: VERIFICATION — Compare row counts
-- ████████████████████████████████████████████████████████████████████████████████
-- Jalankan section ini untuk memastikan semua data ter-copy dengan benar.

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 3: VERIFICATION — Row count comparison';
PRINT '══════════════════════════════════════════════════════';
PRINT '';

-- Group B comparisons
SELECT 'GROUP B: MASTER' AS [Group], 'cos_form_definitions' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_form_definitions]) AS [Old Count],
    'tlkp_cos_form_definitions' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_form_definitions]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_check_items' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_check_items]) AS [Old Count],
    'tlkp_cos_check_items' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_check_items]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_check_sub_rows' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_check_sub_rows]) AS [Old Count],
    'tlkp_cos_check_sub_rows' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_check_sub_rows]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_battery_types' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_battery_types]) AS [Old Count],
    'tlkp_cos_battery_types' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_battery_types]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_battery_standards' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_battery_standards]) AS [Old Count],
    'tlkp_cos_battery_standards' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_battery_standards]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_problem_columns' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_problem_columns]) AS [Old Count],
    'tlkp_cos_problem_columns' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_problem_columns]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_signature_slots' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_signature_slots]) AS [Old Count],
    'tlkp_cos_signature_slots' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_signature_slots]) AS [New Count];

SELECT 'GROUP B: MASTER' AS [Group], 'cos_employee_signatures' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_employee_signatures]) AS [Old Count],
    'tlkp_cos_employee_signatures' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[tlkp_cos_employee_signatures]) AS [New Count];

-- Group C comparisons
SELECT 'GROUP C: TRANS' AS [Group], 'cos_submissions' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_submissions]) AS [Old Count],
    't_cos_submissions' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[t_cos_submissions]) AS [New Count];

SELECT 'GROUP C: TRANS' AS [Group], 'cos_check_values' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_check_values]) AS [Old Count],
    't_cos_check_values' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[t_cos_check_values]) AS [New Count];

SELECT 'GROUP C: TRANS' AS [Group], 'cos_problems' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_problems]) AS [Old Count],
    't_cos_problems' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[t_cos_problems]) AS [New Count];

SELECT 'GROUP C: TRANS' AS [Group], 'cos_signature_entries' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_signature_entries]) AS [Old Count],
    't_cos_signature_entries' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[t_cos_signature_entries]) AS [New Count];

SELECT 'GROUP C: TRANS' AS [Group], 'cos_approval_attachments' AS [Old Table],
    (SELECT COUNT(*) FROM [db_master_data].[dbo].[cos_approval_attachments]) AS [Old Count],
    't_cos_approval_attachments' AS [New Table],
    (SELECT COUNT(*) FROM [db_cos_checksheet].[dbo].[t_cos_approval_attachments]) AS [New Count];

PRINT '';
PRINT '✅ Verifikasi selesai. Pastikan semua Old Count = New Count!';
PRINT '';


-- ████████████████████████████████████████████████████████████████████████████████
-- SECTION 4: CLEANUP — DROP OLD TABLES FROM db_master_data (⚠️ COMMENTED OUT!)
-- ████████████████████████████████████████████████████████████████████████████████
-- ⚠️  JANGAN JALANKAN SECTION INI sampai:
--   1. Semua row counts di Section 3 sudah MATCH (Old Count = New Count)
--   2. Aplikasi sudah di-test dan berjalan normal dengan db_cos_checksheet
--   3. Backup db_master_data sudah dibuat
--
-- Uncomment dan jalankan SATU PER SATU jika sudah yakin.

PRINT '══════════════════════════════════════════════════════';
PRINT ' SECTION 4: CLEANUP — DROP OLD TABLES (COMMENTED OUT)';
PRINT ' ⚠️  Uncomment hanya setelah 100% verifikasi selesai!';
PRINT '══════════════════════════════════════════════════════';

USE [db_master_data];
GO

-- 1. cos_approval_attachments
IF OBJECT_ID('[dbo].[cos_approval_attachments]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_approval_attachments];
    PRINT 'DROPPED: cos_approval_attachments';
END

-- 2. cos_signature_entries
IF OBJECT_ID('[dbo].[cos_signature_entries]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_signature_entries];
    PRINT 'DROPPED: cos_signature_entries';
END

-- 3. cos_problems
IF OBJECT_ID('[dbo].[cos_problems]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_problems];
    PRINT 'DROPPED: cos_problems';
END

-- 4. cos_check_values
IF OBJECT_ID('[dbo].[cos_check_values]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_check_values];
    PRINT 'DROPPED: cos_check_values';
END

-- 5. cos_submissions
IF OBJECT_ID('[dbo].[cos_submissions]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_submissions];
    PRINT 'DROPPED: cos_submissions';
END

-- 6. cos_employee_signatures
IF OBJECT_ID('[dbo].[cos_employee_signatures]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_employee_signatures];
    PRINT 'DROPPED: cos_employee_signatures';
END

-- 7. cos_battery_standards
IF OBJECT_ID('[dbo].[cos_battery_standards]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_battery_standards];
    PRINT 'DROPPED: cos_battery_standards';
END

-- 8. cos_battery_types
IF OBJECT_ID('[dbo].[cos_battery_types]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_battery_types];
    PRINT 'DROPPED: cos_battery_types';
END

-- 9. cos_check_sub_rows
IF OBJECT_ID('[dbo].[cos_check_sub_rows]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_check_sub_rows];
    PRINT 'DROPPED: cos_check_sub_rows';
END

-- 10. cos_check_items
IF OBJECT_ID('[dbo].[cos_check_items]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_check_items];
    PRINT 'DROPPED: cos_check_items';
END

-- 11. cos_problem_columns
IF OBJECT_ID('[dbo].[cos_problem_columns]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_problem_columns];
    PRINT 'DROPPED: cos_problem_columns';
END

-- 12. cos_signature_slots
IF OBJECT_ID('[dbo].[cos_signature_slots]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_signature_slots];
    PRINT 'DROPPED: cos_signature_slots';
END

-- 13. cos_form_definitions
IF OBJECT_ID('[dbo].[cos_form_definitions]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[cos_form_definitions];
    PRINT 'DROPPED: cos_form_definitions';
END

PRINT '';
PRINT '------------------------------------------------------------';
PRINT ' MIGRATION SCRIPT SELESAI.';
PRINT ' ';
PRINT ' Langkah selanjutnya:';
PRINT '   1. Verifikasi table di Object Explorer (refresh)';
PRINT '   2. Jalankan backend: cd backend && dotnet run';
PRINT '   3. Test semua fitur di browser';
PRINT '------------------------------------------------------------';