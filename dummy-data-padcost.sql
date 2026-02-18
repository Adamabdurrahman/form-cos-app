-- ══════════════════════════════════════════════════════
-- DUMMY DATA untuk Test Auto-Detect Running Battery
-- Jalankan di SSMS (SQL Server Management Studio)
-- ══════════════════════════════════════════════════════

-- ── STEP 0: Cek battery types yang sudah ada di COS ──
-- Jalankan ini dulu untuk lihat nama-nama item yang tersedia:
SELECT TOP 20 Id, Name, SourceItemNum 
FROM db_cos_checksheet.dbo.tlkp_cos_battery_types 
ORDER BY Id;

-- ── STEP 1: Cek data line & shift yang dipakai ──
-- Pastikan line_id dan shift_id sesuai dengan yang kamu pilih di form
-- Contoh: line_id = 16, shift_id = 3

-- ══════════════════════════════════════════════════════
-- STEP 2: Insert Dummy Achievement (Header)
-- ══════════════════════════════════════════════════════
USE db_padCost;
GO

-- Hapus dummy data lama (jika ada) supaya bersih
DELETE FROM t_actAchievement WHERE achi_id IN (
    SELECT achi_id FROM t_achievement WHERE temp_line = 'DUMMY_TEST'
);
DELETE FROM t_achievement WHERE temp_line = 'DUMMY_TEST';
GO

-- Insert 1 achievement header untuk hari ini, shift 3
INSERT INTO t_achievement (achi_date, shift_id, grup_id, achi_kasubsie, achi_leader, id_section, temp_line, start_shift, end_shift)
VALUES (
    CAST(GETDATE() AS DATE),   -- hari ini
    3,                          -- shift_id (sesuaikan dengan yang kamu test)
    1,                          -- grup_id
    NULL,                       -- achi_kasubsie
    NULL,                       -- achi_leader
    NULL,                       -- id_section
    'DUMMY_TEST',               -- temp_line (marker supaya gampang di-cleanup)
    GETDATE(),                  -- start_shift
    DATEADD(HOUR, 8, GETDATE()) -- end_shift
);
GO

-- Ambil achi_id yang baru di-insert
DECLARE @achiId INT = SCOPE_IDENTITY();
PRINT 'Inserted achievement with achi_id = ' + CAST(@achiId AS VARCHAR);

-- ══════════════════════════════════════════════════════
-- STEP 3: Insert Dummy ActAchievement (Detail)
-- Gunakan actl_item yang SAMA PERSIS dengan Name/SourceItemNum
-- di tlkp_cos_battery_types
-- ══════════════════════════════════════════════════════

-- ⚠ PENTING: Ganti nilai actl_item di bawah dengan nama battery
-- yang BENAR-BENAR ADA di tlkp_cos_battery_types (dari STEP 0)
-- Contoh di bawah pakai placeholder — GANTI sebelum jalankan!

INSERT INTO t_actAchievement (actl_item, actl_planQty, actl_fgQty, achi_id, line_id, crea_by, crea_date)
VALUES 
    -- Battery 1 (GANTI 'NAMA_BATTERY_1' dengan nama yang valid dari STEP 0)
    ('NAMA_BATTERY_1', 100.00, 95.00, @achiId, 16, 'DUMMY', GETDATE()),
    -- Battery 2 (GANTI 'NAMA_BATTERY_2' dengan nama yang berbeda dari STEP 0)
    ('NAMA_BATTERY_2', 80.00, 78.00, @achiId, 16, 'DUMMY', GETDATE());
GO

-- ══════════════════════════════════════════════════════
-- STEP 4: Verifikasi data sudah masuk
-- ══════════════════════════════════════════════════════

-- Cek achievement header
SELECT * FROM t_achievement WHERE temp_line = 'DUMMY_TEST';

-- Cek detail (harusnya 2 rows)
SELECT a.achi_id, a.achi_date, a.shift_id, b.line_id, b.actl_item, b.actl_planQty, b.actl_fgQty
FROM t_achievement a
INNER JOIN t_actAchievement b ON a.achi_id = b.achi_id
WHERE a.temp_line = 'DUMMY_TEST';

-- Simulasi query yang backend jalankan:
SELECT DISTINCT b.actl_item 
FROM t_achievement AS a
INNER JOIN t_actAchievement AS b ON a.achi_id = b.achi_id
WHERE a.achi_date = CAST(GETDATE() AS DATE) 
  AND a.shift_id = 3        -- shift yang kamu test
  AND b.line_id = 16        -- line yang kamu test
  AND b.actl_item IS NOT NULL;

-- ══════════════════════════════════════════════════════
-- CLEANUP (jalankan SETELAH selesai testing)
-- ══════════════════════════════════════════════════════
-- DELETE FROM t_actAchievement WHERE achi_id IN (
--     SELECT achi_id FROM t_achievement WHERE temp_line = 'DUMMY_TEST'
-- );
-- DELETE FROM t_achievement WHERE temp_line = 'DUMMY_TEST';
