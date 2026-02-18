-- ══════════════════════════════════════════════════════════════
-- CREATE TABLE: t_cos_user_accounts
-- Database: db_cos_checksheet
-- Purpose: Menyimpan kredensial login (NPK ↔ Username/Password)
-- Date: 2026-02-18 (Sesi 11 — Auth Overhaul)
-- ══════════════════════════════════════════════════════════════

USE [db_cos_checksheet];
GO

-- Safety check: hanya buat jika belum ada
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 't_cos_user_accounts')
BEGIN
    CREATE TABLE [dbo].[t_cos_user_accounts] (
        [id]            INT IDENTITY(1,1) PRIMARY KEY,
        [npk]           NVARCHAR(20)  NOT NULL,
        [username]      NVARCHAR(50)  NOT NULL,
        [password_hash] NVARCHAR(255) NOT NULL,
        [created_at]    DATETIME2     NOT NULL DEFAULT GETDATE(),
        [updated_at]    DATETIME2     NOT NULL DEFAULT GETDATE(),

        CONSTRAINT [UQ_user_accounts_npk]      UNIQUE ([npk]),
        CONSTRAINT [UQ_user_accounts_username]  UNIQUE ([username])
    );

    PRINT 'Table t_cos_user_accounts created successfully.';
END
ELSE
BEGIN
    PRINT 'Table t_cos_user_accounts already exists. Skipping.';
END
GO

-- Index untuk pencarian login by username (performance)
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_user_accounts_username' AND object_id = OBJECT_ID('t_cos_user_accounts'))
BEGIN
    CREATE NONCLUSTERED INDEX [IX_user_accounts_username]
    ON [dbo].[t_cos_user_accounts] ([username]);

    PRINT 'Index IX_user_accounts_username created.';
END
GO

-- Verifikasi
SELECT 
    COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 't_cos_user_accounts'
ORDER BY ORDINAL_POSITION;
GO
