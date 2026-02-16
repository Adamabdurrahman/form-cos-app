import React, { useState, useCallback, useMemo, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import DateBox from 'devextreme-react/date-box';
import SelectBox from 'devextreme-react/select-box';
import NumberBox from 'devextreme-react/number-box';
import TextBox from 'devextreme-react/text-box';
import TextArea from 'devextreme-react/text-area';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
    getOperators,
    getBatteryTypes,
    getLines,
    getShifts,
    getMolds,
    getHierarchy,
    getFormDefinitionById,
    getFormSubmissionById,
    updateFormSubmission,
    approveSubmission,
    type OperatorDto,
    type BatteryTypeDto,
    type LineDto,
    type ShiftDto,
    type MoldDto,
    type CheckItemDto,
    type SubRowDto,
    type FormDefinitionDto,
    type FormProblemColumnDto,
    type FormSignatureSlotDto,
    type FormSubmissionPayload,
} from '../../api/cos-api';
import gsLogo from '../../assets/GS.png';
import SignaturePad from '../../components/signature-pad/SignaturePad';
import './submission-response.scss';

// ====================== TYPES ======================

interface ProblemRow {
    id: number;
    [key: string]: unknown;
}

interface BatterySlot {
    type: string | null;
    mold: string | null;
}

// ====================== COMPONENT ======================

export function SubmissionResponse() {
    const { id } = useParams<{ id: string }>();
    const navigate = useNavigate();

    // ===== MODE =====
    const [editMode, setEditMode] = useState(false);

    // ===== FORM DEFINITION (from DB) =====
    const [formDef, setFormDef] = useState<FormDefinitionDto | null>(null);

    // ===== REFERENCE DATA (from API) =====
    const [operatorList, setOperatorList] = useState<OperatorDto[]>([]);
    const [lineList, setLineList] = useState<LineDto[]>([]);
    const [shiftList, setShiftList] = useState<ShiftDto[]>([]);
    const [batteryTypes, setBatteryTypes] = useState<BatteryTypeDto[]>([]);
    const [moldList, setMoldList] = useState<MoldDto[]>([]);
    const [loading, setLoading] = useState(true);
    const [allBatteryTypes, setAllBatteryTypes] = useState<BatteryTypeDto[]>([]);

    // ===== HEADER STATE =====
    const [tanggal, setTanggal] = useState<Date>(new Date());
    const [lineId, setLineId] = useState<number | null>(null);
    const [shiftId, setShiftId] = useState<number | null>(null);
    const [operatorEmpId, setOperatorEmpId] = useState<string | null>(null);
    const [hierarchyIds, setHierarchyIds] = useState<{ leaderEmpId?: string | null; kasubsieEmpId?: string | null; kasieEmpId?: string | null }>({});
    const [hierarchyNames, setHierarchyNames] = useState<Record<string, string>>({});

    // ===== BATTERY SLOTS =====
    const [batterySlots, setBatterySlots] = useState<BatterySlot[]>([]);

    // ===== SETTINGS (check item values) =====
    const [settings, setSettings] = useState<Record<string, unknown>>({});

    // ===== PROBLEMS =====
    const [problems, setProblems] = useState<ProblemRow[]>([]);

    // ===== SIGNATURES =====
    const [signatures, setSignatures] = useState<Record<string, string | null>>({});

    // ===== DERIVED DATA =====
    const checkItems = formDef?.checkItems ?? [];
    const problemColumns: FormProblemColumnDto[] = formDef?.problemColumns ?? [];
    const signatureSlots: FormSignatureSlotDto[] = formDef?.signatureSlots ?? [];
    const slotCount = formDef?.slotCount ?? 3;

    // ===== APPROVAL WORKFLOW STATE =====
    const [status, setStatus] = useState<string>('pending_leader');
    const [hasNg, setHasNg] = useState(false);
    const [operatorSignedAt, setOperatorSignedAt] = useState<string | null>(null);
    const [leaderSignedAt, setLeaderSignedAt] = useState<string | null>(null);
    const [kasubsieSignedAt, setKasubsieSignedAt] = useState<string | null>(null);
    const [kasieSignedAt, setKasieSignedAt] = useState<string | null>(null);
    const [leaderMemo, setLeaderMemo] = useState<string>('');
    const [kasubsieMemo, setKasubsieMemo] = useState<string>('');
    const [kasieMemo, setKasieMemo] = useState<string>('');
    const [approvalSig, setApprovalSig] = useState<string | null>(null);
    const [approvalMemo, setApprovalMemo] = useState<string>('');

    // ===== LOAD DATA =====
    useEffect(() => {
        async function loadData() {
            try {
                // Load submission first
                const submission = await getFormSubmissionById(Number(id));

                // Load reference data & form definition in parallel
                const [ops, lines, shifts, btypes, molds, fd] = await Promise.all([
                    getOperators(submission.lineId),
                    getLines(),
                    getShifts(),
                    getBatteryTypes(submission.lineId ?? null),
                    getMolds(),
                    getFormDefinitionById(submission.formId),
                ]);
                // Also fetch ALL battery types for fallback
                const allBt = submission.lineId ? await getBatteryTypes() : btypes;
                setAllBatteryTypes(allBt);
                setOperatorList(ops);
                setLineList(lines);
                setShiftList(shifts);
                setBatteryTypes(btypes);
                setMoldList(molds);
                setFormDef(fd);

                // Populate header from submission
                setTanggal(new Date(submission.tanggal));
                setLineId(submission.lineId);
                setShiftId(submission.shiftId);
                setOperatorEmpId(submission.operatorEmpId);
                setHierarchyIds({
                    leaderEmpId: submission.leaderEmpId ?? null,
                    kasubsieEmpId: submission.kasubsieEmpId ?? null,
                    kasieEmpId: submission.kasieEmpId ?? null,
                });
                setHierarchyNames({
                    operator: submission.operatorName ?? '',
                    leader: submission.leaderName ?? '',
                    kasubsie: submission.kasubsieName ?? '',
                    kasie: submission.kasieName ?? '',
                });

                // Populate battery slots
                let slots: BatterySlot[] = [];
                try {
                    if (submission.batterySlotsJson) {
                        slots = JSON.parse(submission.batterySlotsJson);
                    }
                } catch { /* ignore */ }
                if (slots.length === 0) {
                    slots = Array.from({ length: fd.slotCount }, () => ({ type: null, mold: null }));
                }
                setBatterySlots(slots);

                // Populate check values (settings)
                const settingsMap: Record<string, unknown> = {};
                submission.checkValues.forEach(cv => {
                    // Detect if it's a visual check (ok/ng) or numeric
                    if (cv.value === 'ok' || cv.value === 'ng') {
                        settingsMap[cv.settingKey] = cv.value;
                    } else {
                        const num = Number(cv.value);
                        settingsMap[cv.settingKey] = isNaN(num) ? cv.value : num;
                    }
                });
                setSettings(settingsMap);

                // Populate problems
                const parsedProblems: ProblemRow[] = submission.problems.map((p, idx) => {
                    try {
                        const vals = JSON.parse(p.valuesJson);
                        return { id: Date.now() + idx, ...vals };
                    } catch {
                        return { id: Date.now() + idx };
                    }
                });
                if (parsedProblems.length === 0) {
                    const emptyRow: ProblemRow = { id: Date.now() };
                    fd.problemColumns.forEach(col => {
                        emptyRow[col.columnKey] = col.fieldType === 'number' ? null : '';
                    });
                    parsedProblems.push(emptyRow);
                }
                setProblems(parsedProblems);

                // Populate signatures
                const sigs: Record<string, string | null> = {};
                fd.signatureSlots.forEach(slot => { sigs[slot.roleKey] = null; });
                submission.signatures.forEach(sig => { sigs[sig.roleKey] = sig.signatureData; });
                setSignatures(sigs);

                // Approval workflow data
                setStatus(submission.status ?? 'pending_leader');
                setHasNg(submission.hasNg ?? false);
                setOperatorSignedAt(submission.operatorSignedAt ?? null);
                setLeaderSignedAt(submission.leaderSignedAt ?? null);
                setKasubsieSignedAt(submission.kasubsieSignedAt ?? null);
                setKasieSignedAt(submission.kasieSignedAt ?? null);
                setLeaderMemo(submission.leaderMemo ?? '');
                setKasubsieMemo(submission.kasubsieMemo ?? '');
                setKasieMemo(submission.kasieMemo ?? '');
            } catch (err) {
                console.error('Failed to load data:', err);
                notify('Gagal memuat data submission', 'error', 3000);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, [id]);

    // ===== CHAIN SELECT EFFECTS (edit mode) =====
    const handleLineChange = useCallback((newLineId: number | null) => {
        setLineId(newLineId);
        setOperatorEmpId(null);
        setHierarchyIds({});
        setHierarchyNames(prev => ({ ...prev, leader: '', kasubsie: '', kasie: '' }));
        // Clear battery slots type (since available types changed)
        setBatterySlots(prev => prev.map(() => ({ type: null, mold: null })));
    }, []);

    const handleOperatorChange = useCallback(async (empId: string | null) => {
        setOperatorEmpId(empId);
        if (empId) {
            try {
                const h = await getHierarchy(empId);
                setHierarchyIds({
                    leaderEmpId: h.leaderEmpId ?? null,
                    kasubsieEmpId: h.kasubsieEmpId ?? null,
                    kasieEmpId: h.kasieEmpId ?? null,
                });
                setHierarchyNames(prev => ({
                    ...prev,
                    operator: h.operatorName ?? '',
                    leader: h.leaderName ?? '',
                    kasubsie: h.kasubsieName ?? '',
                    kasie: h.kasieName ?? '',
                }));
            } catch { /* ignore */ }
        } else {
            setHierarchyIds({});
            setHierarchyNames(prev => ({ ...prev, leader: '', kasubsie: '', kasie: '' }));
        }
    }, []);

    // Effect: when lineId changes → reload batteryTypes + operators
    useEffect(() => {
        if (!editMode) return;
        let cancelled = false;
        (async () => {
            const [filteredBt, filteredOps] = await Promise.all([
                getBatteryTypes(lineId),
                getOperators(lineId),
            ]);
            if (!cancelled) {
                setBatteryTypes(filteredBt);
                setOperatorList(filteredOps);
            }
        })();
        return () => { cancelled = true; };
    }, [editMode, lineId]);

    // ===== HANDLERS (only active in edit mode) =====
    const updateSlot = useCallback((index: number, field: 'type' | 'mold', value: string | null) => {
        setBatterySlots(prev => {
            const updated = [...prev];
            updated[index] = { ...updated[index], [field]: value };
            if (field === 'type') updated[index].mold = null;
            return updated;
        });
    }, []);

    const updateSetting = useCallback((key: string, value: unknown) => {
        setSettings(prev => ({ ...prev, [key]: value }));
    }, []);

    const addProblemRow = useCallback(() => {
        const row: ProblemRow = { id: Date.now() };
        problemColumns.forEach(col => {
            row[col.columnKey] = col.fieldType === 'number' ? null : '';
        });
        setProblems(prev => [...prev, row]);
    }, [problemColumns]);

    const removeProblemRow = useCallback((rowId: number) => {
        setProblems(prev => prev.length > 1 ? prev.filter(p => p.id !== rowId) : prev);
    }, []);

    const updateProblem = useCallback((rowId: number, field: string, value: unknown) => {
        setProblems(prev =>
            prev.map(p => (p.id === rowId ? { ...p, [field]: value } : p))
        );
    }, []);

    const updateSignature = useCallback((key: string, dataUrl: string | null) => {
        setSignatures(prev => ({ ...prev, [key]: dataUrl }));
    }, []);

    // ===== COMPUTED =====
    const getMoldsForSlot = useCallback((_slotIdx: number) => {
        return moldList.map(m => m.code);
    }, [moldList]);

    const getStandardValue = useCallback(
        (item: CheckItemDto, bt: BatteryTypeDto | undefined): string => {
            if (!bt || !item.numericStdKey) return '';
            const std = bt.standards.find(s => s.paramKey === item.numericStdKey);
            return std?.value ?? '';
        }, []
    );

    const getStandard = useCallback(
        (item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number): string => {
            if (subRow?.fixedStandard) return subRow.fixedStandard;
            if (item.type === 'visual') return item.visualStandard ?? '';
            if (item.fixedStandard) return item.fixedStandard;
            const btName = batterySlots[slotIdx]?.type;
            if (!btName || !item.numericStdKey) return '';
            const typeData = batteryTypes.find(t => t.name === btName);
            return getStandardValue(item, typeData);
        },
        [batterySlots, batteryTypes, getStandardValue]
    );

    const getMinMax = useCallback(
        (item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number): { min?: number; max?: number } => {
            if (subRow?.fixedMin != null || subRow?.fixedMax != null) {
                return { min: subRow?.fixedMin ?? undefined, max: subRow?.fixedMax ?? undefined };
            }
            if (item.fixedMin != null || item.fixedMax != null) {
                return { min: item.fixedMin ?? undefined, max: item.fixedMax ?? undefined };
            }
            if (item.numericStdKey) {
                const btName = batterySlots[slotIdx]?.type;
                if (btName) {
                    const typeData = batteryTypes.find(t => t.name === btName);
                    const std = typeData?.standards.find(s => s.paramKey === item.numericStdKey);
                    if (std) {
                        return { min: std.minValue ?? undefined, max: std.maxValue ?? undefined };
                    }
                }
            }
            return {};
        },
        [batterySlots, batteryTypes]
    );

    const isOutOfRange = useCallback(
        (value: number | null | undefined, min?: number, max?: number): boolean => {
            if (value == null) return false;
            if (min != null && value < min) return true;
            if (max != null && value > max) return true;
            return false;
        }, []
    );

    const operatorName = useMemo(() => {
        if (!operatorEmpId) return '';
        return operatorList.find(o => o.empId === operatorEmpId)?.name ?? hierarchyNames.operator ?? '';
    }, [operatorEmpId, operatorList, hierarchyNames]);

    const getEmpIdForRole = useCallback((roleKey: string): string | null => {
        if (roleKey === 'operator') return operatorEmpId;
        if (roleKey === 'leader') return hierarchyIds.leaderEmpId ?? null;
        if (roleKey === 'kasubsie') return hierarchyIds.kasubsieEmpId ?? null;
        if (roleKey === 'kasie') return hierarchyIds.kasieEmpId ?? null;
        return null;
    }, [operatorEmpId, hierarchyIds]);

    const lineName = useMemo(() => {
        if (!lineId) return '-';
        return lineList.find(l => l.id === lineId)?.name ?? String(lineId);
    }, [lineId, lineList]);

    const shiftName = useMemo(() => {
        if (!shiftId) return '-';
        return shiftList.find(s => s.id === shiftId)?.name ?? String(shiftId);
    }, [shiftId, shiftList]);

    // ===== RENDER HELPERS =====

    function renderSettingInput(item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number) {
        const key = subRow?.suffix
            ? `${item.itemKey}_${subRow.suffix}_${slotIdx}`
            : `${item.itemKey}_${slotIdx}`;

        if (!batterySlots[slotIdx]?.type) {
            return <span className="empty-cell">—</span>;
        }

        if (item.type === 'visual') {
            const val = (settings[key] as ('ok' | 'ng' | undefined)) ?? 'ng';
            if (editMode) {
                return (
                    <div className="visual-check-pair">
                        <div
                            className={`visual-btn ng-btn ${val === 'ng' ? 'active' : ''}`}
                            onClick={() => updateSetting(key, 'ng')}
                            title="NG"
                        >
                            ✗
                        </div>
                        <div
                            className={`visual-btn ok-btn ${val === 'ok' ? 'active' : ''}`}
                            onClick={() => updateSetting(key, 'ok')}
                            title="OK"
                        >
                            ✓
                        </div>
                    </div>
                );
            }
            // Read-only visual — only show the result
            return (
                <div className={`visual-readonly ${val === 'ng' ? 'ng' : 'ok'}`}>
                    {val === 'ng' ? '✗' : '✓'}
                </div>
            );
        }

        // Numeric
        const numVal = settings[key] as number | null | undefined;
        const { min, max } = getMinMax(item, subRow, slotIdx);
        const outOfRange = isOutOfRange(numVal ?? null, min, max);

        if (editMode) {
            return (
                <NumberBox
                    value={numVal ?? undefined}
                    onValueChanged={(e) => updateSetting(key, e.value)}
                    stylingMode="underlined"
                    height={26}
                    showSpinButtons={false}
                    className={outOfRange ? 'out-of-range' : ''}
                />
            );
        }

        // Read-only numeric
        return (
            <span className={`readonly-value${outOfRange ? ' out-of-range-text' : ''}`}>
                {numVal != null ? numVal : '—'}
            </span>
        );
    }

    function renderCheckItemRows(item: CheckItemDto): React.ReactNode {
        const rows = item.subRows && item.subRows.length > 0 ? item.subRows : null;
        const hasSubRows = !!rows;
        const rowList: (SubRowDto | null)[] = rows ?? [null];
        const rowCount = rowList.length;

        return rowList.map((subRow, subIdx) => (
            <tr key={`${item.itemKey}_${subRow?.suffix ?? 'main'}`} className="check-row">
                {subIdx === 0 && hasSubRows && (
                    <td rowSpan={rowCount} className="item-name-cell">{item.label}</td>
                )}
                {!hasSubRows && (
                    <td colSpan={2} className="item-name-cell">{item.label}</td>
                )}
                {hasSubRows && (
                    <td className="sub-label-cell">{subRow?.label}</td>
                )}
                {Array.from({ length: slotCount }, (_, slotIdx) => (
                    <React.Fragment key={slotIdx}>
                        <td className="standard-cell">{getStandard(item, subRow, slotIdx)}</td>
                        <td className={`setting-cell${item.type === 'visual' && batterySlots[slotIdx]?.type ? ' visual-setting' : ''}`}>
                            {renderSettingInput(item, subRow, slotIdx)}
                        </td>
                    </React.Fragment>
                ))}
                {subIdx === 0 && (
                    <>
                        <td rowSpan={hasSubRows ? rowCount : undefined} className="frequency-cell">
                            {item.frequency}
                        </td>
                        <td rowSpan={hasSubRows ? rowCount : undefined} className="ket-cell">
                            {item.keterangan ?? ''}
                        </td>
                    </>
                )}
            </tr>
        ));
    }

    // ===== APPROVAL =====
    const currentApprovalRole = useMemo(() => {
        if (status === 'pending_leader') return 'leader';
        if (status === 'pending_kasubsie') return 'kasubsie';
        if (status === 'pending_kasie') return 'kasie';
        return null; // completed
    }, [status]);

    const currentApprovalEmpId = useMemo(() => {
        if (currentApprovalRole === 'leader') return hierarchyIds.leaderEmpId ?? null;
        if (currentApprovalRole === 'kasubsie') return hierarchyIds.kasubsieEmpId ?? null;
        if (currentApprovalRole === 'kasie') return hierarchyIds.kasieEmpId ?? null;
        return null;
    }, [currentApprovalRole, hierarchyIds]);

    const currentApprovalName = useMemo(() => {
        if (currentApprovalRole) return hierarchyNames[currentApprovalRole] || currentApprovalRole;
        return '';
    }, [currentApprovalRole, hierarchyNames]);

    const handleApprove = useCallback(async () => {
        if (!currentApprovalRole) return;
        if (!approvalSig) {
            notify('Tanda tangan wajib diisi sebelum approve.', 'warning', 3000);
            return;
        }
        if (hasNg && !approvalMemo.trim()) {
            notify('Memo wajib diisi karena terdapat item NG.', 'warning', 3000);
            return;
        }
        try {
            const result = await approveSubmission(Number(id), {
                role: currentApprovalRole,
                signatureData: approvalSig,
                memo: approvalMemo || null,
            });
            setStatus(result.status);
            // Update timestamps locally
            const now = new Date().toISOString();
            if (currentApprovalRole === 'leader') { setLeaderSignedAt(now); setLeaderMemo(approvalMemo); }
            if (currentApprovalRole === 'kasubsie') { setKasubsieSignedAt(now); setKasubsieMemo(approvalMemo); }
            if (currentApprovalRole === 'kasie') { setKasieSignedAt(now); setKasieMemo(approvalMemo); }
            // Update signature display
            setSignatures(prev => ({ ...prev, [currentApprovalRole!]: approvalSig }));
            setApprovalSig(null);
            setApprovalMemo('');
            notify(`Berhasil di-approve oleh ${currentApprovalRole}!`, 'success', 3000);
        } catch (err) {
            console.error('Approve error:', err);
            notify('Gagal approve. Lihat console untuk detail.', 'error', 3000);
        }
    }, [currentApprovalRole, approvalSig, approvalMemo, hasNg, id]);

    const formatTimestamp = (ts: string | null) => {
        if (!ts) return null;
        const d = new Date(ts);
        return d.toLocaleDateString('id-ID') + ' ' + d.toLocaleTimeString('id-ID', { hour: '2-digit', minute: '2-digit' });
    };

    // ===== PRINT =====
    const handlePrint = useCallback(() => {
        window.print();
    }, []);

    // ===== SAVE (update) =====
    const handleSave = useCallback(async () => {
        if (!formDef) return;

        const payload: FormSubmissionPayload = {
            formId: formDef.id,
            tanggal: tanggal.toISOString(),
            lineId: lineId,
            shiftId: shiftId,
            operatorEmpId: operatorEmpId,
            batterySlotsJson: JSON.stringify(batterySlots),
            checkValues: Object.entries(settings)
                .filter(([, v]) => v !== undefined)
                .map(([k, v]) => ({ settingKey: k, value: v != null ? String(v) : null })),
            problems: problems.map((p, idx) => {
                const vals: Record<string, unknown> = {};
                problemColumns.forEach(col => { vals[col.columnKey] = p[col.columnKey]; });
                return { sortOrder: idx + 1, valuesJson: JSON.stringify(vals) };
            }),
            signatures: signatureSlots.map(slot => ({
                roleKey: slot.roleKey,
                signatureData: signatures[slot.roleKey] ?? null,
            })),
        };

        try {
            await updateFormSubmission(Number(id), payload);
            notify('Submission berhasil diperbarui!', 'success', 2500);
            setEditMode(false);
        } catch (err) {
            console.error('Update error:', err);
            notify('Gagal menyimpan perubahan. Lihat console untuk detail.', 'error', 3000);
        }
    }, [formDef, tanggal, lineId, shiftId, operatorEmpId, batterySlots, settings, problems, problemColumns, signatures, signatureSlots, id]);

    // ===== MAIN RENDER =====
    if (loading || !formDef) {
        return (
            <div className="submission-response-page" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
                <LoadPanel visible={loading} />
                {!loading && <p>Submission tidak ditemukan.</p>}
            </div>
        );
    }

    return (
        <div className="submission-response-page">
            {/* ===== TOOLBAR (no-print) ===== */}
            <div className="response-toolbar no-print">
                <Button text="← Kembali" icon="back" stylingMode="outlined" onClick={() => navigate('/admin/submissions')} />
                <div className="toolbar-right">
                    {!editMode ? (
                        <>
                            <Button text="Edit" icon="edit" stylingMode="outlined" type="default" onClick={() => setEditMode(true)} />
                            <Button text="Print" icon="print" stylingMode="contained" type="default" onClick={handlePrint} />
                        </>
                    ) : (
                        <>
                            <Button text="Batal" icon="close" stylingMode="outlined" onClick={() => setEditMode(false)} />
                            <Button text="Simpan Perubahan" icon="save" stylingMode="contained" type="success" onClick={handleSave} />
                        </>
                    )}
                </div>
            </div>

            {/* ===== FORM ===== */}
            <div className={`cos-form ${editMode ? 'edit-mode' : 'view-mode'}`}>
                {/* ==================== HEADER ==================== */}
                <div className="form-header">
                    <div className="header-left">
                        <img src={gsLogo} alt="PT. GS Battery" className="gs-logo" />
                    </div>
                    <div className="header-center">
                        <h3 className="form-title">{formDef.title}</h3>
                        <div className="form-id">{formDef.subtitle}</div>
                    </div>
                    <div className="header-right">
                        <div className="approval-section">
                            {signatureSlots.map(slot => (
                                <div className="approval-box" key={slot.roleKey}>
                                    {editMode ? (
                                        <SignaturePad
                                            label={slot.label}
                                            name={hierarchyNames[slot.roleKey] || operatorName || slot.roleKey}
                                            empId={getEmpIdForRole(slot.roleKey)}
                                            initialValue={signatures[slot.roleKey]}
                                            onChange={(d) => updateSignature(slot.roleKey, d)}
                                        />
                                    ) : (
                                        <>
                                            <div className="signature-view-label">{slot.label}</div>
                                            {signatures[slot.roleKey] ? (
                                                <img src={signatures[slot.roleKey]!} alt={slot.label} className="signature-view-img" />
                                            ) : (
                                                <div className="signature-view-empty">—</div>
                                            )}
                                            <div className="signature-view-name">
                                                {hierarchyNames[slot.roleKey] || operatorName || ''}
                                            </div>
                                        </>
                                    )}
                                </div>
                            ))}
                        </div>
                    </div>
                </div>

                {/* ==================== INFO FIELDS ==================== */}
                <div className="form-info">
                    <table className="info-table">
                        <tbody>
                            <tr>
                                <td className="info-label">Tanggal</td>
                                <td className="info-sep">:</td>
                                <td className="info-value">
                                    {editMode ? (
                                        <DateBox
                                            value={tanggal}
                                            onValueChanged={(e) => setTanggal(e.value)}
                                            type="date"
                                            displayFormat="dd/MM/yyyy"
                                            stylingMode="underlined"
                                            height={30}
                                            width={180}
                                        />
                                    ) : (
                                        <span className="readonly-text">{tanggal.toLocaleDateString('id-ID')}</span>
                                    )}
                                </td>
                            </tr>
                            <tr>
                                <td className="info-label">Line / Shift</td>
                                <td className="info-sep">:</td>
                                <td className="info-value">
                                    {editMode ? (
                                        <div className="line-shift-row">
                                            <SelectBox
                                                dataSource={lineList}
                                                valueExpr="id"
                                                displayExpr="name"
                                                value={lineId}
                                                onValueChanged={(e) => handleLineChange(e.value)}
                                                placeholder="Line"
                                                stylingMode="underlined"
                                                height={30}
                                                width={120}
                                                showClearButton={true}
                                            />
                                            <span className="separator">/</span>
                                            <SelectBox
                                                dataSource={shiftList}
                                                valueExpr="id"
                                                displayExpr="name"
                                                value={shiftId}
                                                onValueChanged={(e) => setShiftId(e.value)}
                                                placeholder="Shift"
                                                stylingMode="underlined"
                                                height={30}
                                                width={120}
                                            />
                                        </div>
                                    ) : (
                                        <span className="readonly-text">{lineName} / {shiftName}</span>
                                    )}
                                </td>
                            </tr>
                            <tr>
                                <td className="info-label">Operator</td>
                                <td className="info-sep">:</td>
                                <td className="info-value">
                                    {editMode ? (
                                        <SelectBox
                                            dataSource={operatorList}
                                            displayExpr="name"
                                            valueExpr="empId"
                                            value={operatorEmpId}
                                            onValueChanged={(e) => handleOperatorChange(e.value)}
                                            searchEnabled={true}
                                            searchMode="contains"
                                            placeholder={lineId ? 'Ketik nama operator...' : 'Pilih Line dulu...'}
                                            stylingMode="underlined"
                                            height={30}
                                            width={280}
                                            showClearButton={true}
                                            disabled={!lineId}
                                        />
                                    ) : (
                                        <span className="readonly-text">{operatorName}</span>
                                    )}
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </div>

                {/* ==================== SECTION A: CHECK TABLE ==================== */}
                <div className="section-a">
                    <div className="section-title">A. VALIDASI COS</div>
                    <div className="table-scroll-wrapper">
                        <table className="cos-table">
                            <colgroup>
                                <col style={{ width: '160px' }} />
                                <col style={{ width: '40px' }} />
                                {Array.from({ length: slotCount }, (_, i) => (
                                    <React.Fragment key={i}>
                                        <col style={{ width: '85px' }} />
                                        <col style={{ width: '80px' }} />
                                    </React.Fragment>
                                ))}
                                <col style={{ width: '100px' }} />
                                <col style={{ width: '100px' }} />
                            </colgroup>
                            <thead>
                                <tr>
                                    <th colSpan={2} rowSpan={2} className="header-item">ITEM CHECK</th>
                                    {Array.from({ length: slotCount }, (_, slotIdx) => (
                                        <th colSpan={2} key={`type_${slotIdx}`} className="header-type">
                                            <div className="type-header-label">TYPE BATTERY</div>
                                            {editMode ? (
                                                <SelectBox
                                                    items={batteryTypes.map(t => t.name)}
                                                    value={batterySlots[slotIdx]?.type ?? null}
                                                    onValueChanged={(e) => updateSlot(slotIdx, 'type', e.value)}
                                                    placeholder={`Type ${slotIdx + 1}`}
                                                    stylingMode="underlined"
                                                    height={26}
                                                    showClearButton={true}
                                                    searchEnabled={true}
                                                />
                                            ) : (
                                                <span className="readonly-header-value">{batterySlots[slotIdx]?.type || '—'}</span>
                                            )}
                                        </th>
                                    ))}
                                    <th rowSpan={2} className="header-freq">FREKUENSI</th>
                                    <th rowSpan={2} className="header-ket">KET</th>
                                </tr>
                                <tr>
                                    {Array.from({ length: slotCount }, (_, slotIdx) => (
                                        <React.Fragment key={`hdr_${slotIdx}`}>
                                            <th className="header-std">Standar</th>
                                            <th className="header-set">Setting</th>
                                        </React.Fragment>
                                    ))}
                                </tr>
                                <tr>
                                    <th colSpan={2} className="header-mold-label">NO MOLD</th>
                                    {Array.from({ length: slotCount }, (_, slotIdx) => (
                                        <th colSpan={2} key={`mold_${slotIdx}`} className="header-mold">
                                            {editMode ? (
                                                <SelectBox
                                                    items={getMoldsForSlot(slotIdx)}
                                                    value={batterySlots[slotIdx]?.mold ?? null}
                                                    onValueChanged={(e) => updateSlot(slotIdx, 'mold', e.value)}
                                                    placeholder="Mold"
                                                    stylingMode="underlined"
                                                    height={26}
                                                    disabled={!batterySlots[slotIdx]?.type}
                                                    showClearButton={true}
                                                />
                                            ) : (
                                                <span className="readonly-header-value">{batterySlots[slotIdx]?.mold || '—'}</span>
                                            )}
                                        </th>
                                    ))}
                                    <th></th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody>
                                {checkItems.map(item => renderCheckItemRows(item))}
                            </tbody>
                        </table>
                    </div>
                </div>

                {/* ==================== SECTION B: PROBLEMS ==================== */}
                <div className="section-b">
                    <div className="section-title">B. PROBLEM</div>
                    <table className="problems-table">
                        <thead>
                            <tr>
                                <th style={{ width: '30px' }}>No</th>
                                {problemColumns.map(col => (
                                    <th key={col.columnKey} style={{ width: col.width }}>{col.label}</th>
                                ))}
                                {editMode && <th style={{ width: '40px' }}></th>}
                            </tr>
                        </thead>
                        <tbody>
                            {problems.map((prob, idx) => (
                                <tr key={prob.id}>
                                    <td className="problem-no">{idx + 1}</td>
                                    {problemColumns.map(col => (
                                        <td key={col.columnKey}>
                                            {editMode ? (
                                                col.fieldType === 'number' ? (
                                                    <NumberBox
                                                        value={(prob[col.columnKey] as number) ?? undefined}
                                                        onValueChanged={(e) => updateProblem(prob.id, col.columnKey, e.value)}
                                                        stylingMode="underlined"
                                                        height={28}
                                                        min={0}
                                                    />
                                                ) : (
                                                    <TextBox
                                                        value={(prob[col.columnKey] as string) ?? ''}
                                                        onValueChanged={(e) => updateProblem(prob.id, col.columnKey, e.value)}
                                                        stylingMode="underlined"
                                                        height={28}
                                                    />
                                                )
                                            ) : (
                                                <span className="readonly-value">
                                                    {prob[col.columnKey] != null ? String(prob[col.columnKey]) : ''}
                                                </span>
                                            )}
                                        </td>
                                    ))}
                                    {editMode && (
                                        <td className="problem-action">
                                            <Button
                                                icon="trash"
                                                stylingMode="text"
                                                type="danger"
                                                hint="Hapus baris"
                                                onClick={() => removeProblemRow(prob.id)}
                                                disabled={problems.length <= 1}
                                            />
                                        </td>
                                    )}
                                </tr>
                            ))}
                        </tbody>
                    </table>
                    {editMode && (
                        <div className="add-problem-row">
                            <Button
                                text="+ Tambah Baris Problem"
                                stylingMode="outlined"
                                type="default"
                                onClick={addProblemRow}
                            />
                        </div>
                    )}
                </div>

                {/* ==================== APPROVAL WORKFLOW ==================== */}
                <div className="approval-workflow-section no-print">
                    <div className="section-title">APPROVAL WORKFLOW</div>

                    {hasNg && (
                        <div className="ng-warning-badge">
                            <span className="ng-badge-icon">⚠</span>
                            Terdapat item NG pada form ini
                        </div>
                    )}

                    {/* --- Timeline --- */}
                    <div className="approval-timeline">
                        {/* Operator */}
                        <div className={`timeline-step ${operatorSignedAt ? 'done' : 'pending'}`}>
                            <div className="step-icon">{operatorSignedAt ? '✓' : '○'}</div>
                            <div className="step-content">
                                <div className="step-role">Operator</div>
                                <div className="step-name">{hierarchyNames.operator || operatorName}</div>
                                {operatorSignedAt && <div className="step-time">{formatTimestamp(operatorSignedAt)}</div>}
                            </div>
                        </div>
                        {/* Leader */}
                        <div className={`timeline-step ${leaderSignedAt ? 'done' : status === 'pending_leader' ? 'current' : 'pending'}`}>
                            <div className="step-icon">{leaderSignedAt ? '✓' : status === 'pending_leader' ? '⏳' : '○'}</div>
                            <div className="step-content">
                                <div className="step-role">Leader</div>
                                <div className="step-name">{hierarchyNames.leader || '-'}</div>
                                {leaderSignedAt && <div className="step-time">{formatTimestamp(leaderSignedAt)}</div>}
                                {leaderMemo && <div className="step-memo">Memo: {leaderMemo}</div>}
                            </div>
                        </div>
                        {/* Kasubsie */}
                        <div className={`timeline-step ${kasubsieSignedAt ? 'done' : status === 'pending_kasubsie' ? 'current' : 'pending'}`}>
                            <div className="step-icon">{kasubsieSignedAt ? '✓' : status === 'pending_kasubsie' ? '⏳' : '○'}</div>
                            <div className="step-content">
                                <div className="step-role">Kasubsie</div>
                                <div className="step-name">{hierarchyNames.kasubsie || '-'}</div>
                                {kasubsieSignedAt && <div className="step-time">{formatTimestamp(kasubsieSignedAt)}</div>}
                                {kasubsieMemo && <div className="step-memo">Memo: {kasubsieMemo}</div>}
                            </div>
                        </div>
                        {/* Kasie */}
                        <div className={`timeline-step ${kasieSignedAt ? 'done' : status === 'pending_kasie' ? 'current' : 'pending'}`}>
                            <div className="step-icon">{kasieSignedAt ? '✓' : status === 'pending_kasie' ? '⏳' : '○'}</div>
                            <div className="step-content">
                                <div className="step-role">Kasie</div>
                                <div className="step-name">{hierarchyNames.kasie || '-'}</div>
                                {kasieSignedAt && <div className="step-time">{formatTimestamp(kasieSignedAt)}</div>}
                                {kasieMemo && <div className="step-memo">Memo: {kasieMemo}</div>}
                            </div>
                        </div>
                    </div>

                    {/* --- Approve action (only if not completed) --- */}
                    {currentApprovalRole && (
                        <div className="approve-action-section">
                            <div className="approve-action-title">
                                Approve sebagai <strong>{currentApprovalRole === 'kasubsie' ? 'Kasubsie' : currentApprovalRole === 'kasie' ? 'Kasie' : 'Leader'}</strong>
                                {currentApprovalName && <span> ({currentApprovalName})</span>}
                            </div>

                            {/* Memo */}
                            <div className="approve-memo">
                                <label className="approve-label">
                                    Memo / Catatan {hasNg ? <span className="required-mark">*wajib (ada NG)</span> : <span className="optional-mark">(opsional)</span>}
                                </label>
                                <TextArea
                                    value={approvalMemo}
                                    onValueChanged={(e) => setApprovalMemo(e.value ?? '')}
                                    placeholder="Tulis catatan..."
                                    height={80}
                                    stylingMode="outlined"
                                />
                            </div>

                            {/* Signature */}
                            <div className="approve-signature">
                                <label className="approve-label">Tanda Tangan Digital <span className="required-mark">*wajib</span></label>
                                <SignaturePad
                                    key={`approval-${currentApprovalRole}`}
                                    label="TTD Approval"
                                    name={currentApprovalName}
                                    empId={currentApprovalEmpId}
                                    initialValue={null}
                                    onChange={(d) => setApprovalSig(d)}
                                />
                            </div>

                            {/* Button */}
                            <div className="approve-btn-row">
                                <Button
                                    text="Approve & Submit"
                                    icon="check"
                                    type="success"
                                    stylingMode="contained"
                                    onClick={handleApprove}
                                    disabled={!approvalSig || (hasNg && !approvalMemo.trim())}
                                    width={220}
                                    height={40}
                                />
                            </div>
                        </div>
                    )}

                    {status === 'completed' && (
                        <div className="approval-completed-banner">
                            <span className="completed-icon">✓</span>
                            Semua approval telah selesai
                        </div>
                    )}
                </div>

                {/* ==================== SUBMISSION ID ==================== */}
                <div className="form-footer-info no-print">
                    <span className="submission-id-label">Submission ID: #{id}</span>
                </div>
            </div>
        </div>
    );
}
