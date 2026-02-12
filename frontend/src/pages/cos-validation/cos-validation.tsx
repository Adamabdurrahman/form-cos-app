import React, { useState, useCallback, useMemo, useEffect, useRef } from 'react';
import { useBlocker } from 'react-router-dom';
import DateBox from 'devextreme-react/date-box';
import SelectBox from 'devextreme-react/select-box';
import NumberBox from 'devextreme-react/number-box';
import TextBox from 'devextreme-react/text-box';
import Button from 'devextreme-react/button';
import { custom as dxCustomDialog } from 'devextreme/ui/dialog';
import dxNotify from 'devextreme/ui/notify';
import {
    getOperators,
    getBatteryTypes,
    getLines,
    getShifts,
    getMolds,
    getHierarchy,
    getFormDefinitionByCode,
    submitFormSubmission,
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
import SignaturePad from '../../components/signature-pad/SignaturePad';
import gsLogo from '../../assets/GS.png';
import './cos-validation.scss';

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

export function CosValidation({ formCode = 'COS_VALIDATION' }: { formCode?: string }) {
    // ===== FORM DEFINITION (from DB) =====
    const [formDef, setFormDef] = useState<FormDefinitionDto | null>(null);

    // ===== REFERENCE DATA (from API) =====
    const [operatorList, setOperatorList] = useState<OperatorDto[]>([]);
    const [lineList, setLineList] = useState<LineDto[]>([]);
    const [shiftList, setShiftList] = useState<ShiftDto[]>([]);
    const [batteryTypes, setBatteryTypes] = useState<BatteryTypeDto[]>([]);
    const [allBatteryTypes, setAllBatteryTypes] = useState<BatteryTypeDto[]>([]);
    const [moldList, setMoldList] = useState<MoldDto[]>([]);
    const [loading, setLoading] = useState(true);

    // ===== HEADER STATE =====
    const [tanggal, setTanggal] = useState<Date>(new Date());
    const [lineId, setLineId] = useState<number | null>(null);
    const [shiftId, setShiftId] = useState<number | null>(null);
    const [operatorEmpId, setOperatorEmpId] = useState<string | null>(null);
    const [hierarchyIds, setHierarchyIds] = useState<{
        leaderEmpId?: string | null;
        kasubsieEmpId?: string | null;
        kasieEmpId?: string | null;
    }>({});
    const [hierarchyNames, setHierarchyNames] = useState<Record<string, string>>({});

    // ===== BATTERY SLOTS (dynamic count from formDef) =====
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

    // ===== FORM PROTECTION (prevent data loss) =====
    const STORAGE_KEY = `cos_form_draft_${formCode}`;
    const [formTouched, setFormTouched] = useState(false);
    const initialLoadDone = useRef(false);

    // ===== LOAD DATA =====
    useEffect(() => {
        async function loadData() {
            try {
                const [ops, lines, shifts, btypes, molds, fd] = await Promise.all([
                    getOperators(),
                    getLines(),
                    getShifts(),
                    getBatteryTypes(),
                    getMolds(),
                    getFormDefinitionByCode(formCode),
                ]);
                setOperatorList(ops);
                setLineList(lines);
                setShiftList(shifts);
                setBatteryTypes(btypes);
                setAllBatteryTypes(btypes);
                setMoldList(molds);
                setFormDef(fd);

                // Initialize battery slots based on form definition
                setBatterySlots(Array.from({ length: fd.slotCount }, () => ({ type: null, mold: null })));

                // Initialize problems with one empty row
                const emptyRow: ProblemRow = { id: Date.now() };
                fd.problemColumns.forEach(col => {
                    emptyRow[col.columnKey] = col.fieldType === 'number' ? null : '';
                });
                setProblems([emptyRow]);

                // Initialize signatures
                const sigs: Record<string, string | null> = {};
                fd.signatureSlots.forEach(slot => { sigs[slot.roleKey] = null; });
                setSignatures(sigs);

                // ---- Restore draft from sessionStorage ----
                try {
                    const draftJson = sessionStorage.getItem(`cos_form_draft_${formCode}`);
                    if (draftJson) {
                        const d = JSON.parse(draftJson);
                        if (d.tanggal) setTanggal(new Date(d.tanggal));
                        if (d.lineId != null) {
                            setLineId(d.lineId);
                            // Reload battery types for this line
                            getBatteryTypes(d.lineId).then(setBatteryTypes).catch(() => {});
                        }
                        if (d.shiftId != null) setShiftId(d.shiftId);
                        if (d.lineId != null) {
                            getOperators(d.lineId).then(setOperatorList).catch(() => {});
                        }
                        if (d.operatorEmpId) {
                            setOperatorEmpId(d.operatorEmpId);
                            getHierarchy(d.operatorEmpId).then(h => {
                                setHierarchyIds({
                                    leaderEmpId: h.leaderEmpId,
                                    kasubsieEmpId: h.kasubsieEmpId,
                                    kasieEmpId: h.kasieEmpId,
                                });
                                setHierarchyNames({
                                    operator: h.operatorName ?? '',
                                    leader: h.leaderName ?? '',
                                    kasubsie: h.kasubsieName ?? '',
                                    kasie: h.kasieName ?? '',
                                });
                            }).catch(() => {});
                        }
                        if (d.batterySlots?.length) setBatterySlots(d.batterySlots);
                        if (d.settings && Object.keys(d.settings).length) setSettings(d.settings);
                        if (d.problems?.length) setProblems(d.problems);
                        if (d.signatures) setSignatures(prev => ({ ...prev, ...d.signatures }));
                    }
                } catch { /* ignore corrupt data */ }
            } catch (err) {
                console.error('Failed to load data:', err);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, [formCode]);

    // Mark initial load as done (delay to let restored state settle)
    useEffect(() => {
        if (!loading && formDef) {
            const t = setTimeout(() => { initialLoadDone.current = true; }, 600);
            return () => clearTimeout(t);
        }
    }, [loading, formDef]);

    // Track dirty state — any form field change after initial load
    useEffect(() => {
        if (initialLoadDone.current) setFormTouched(true);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [tanggal, lineId, shiftId, operatorEmpId, batterySlots, settings, problems, signatures]);

    // 1️⃣ Warn on browser refresh / close tab (beforeunload)
    useEffect(() => {
        if (!formTouched) return;
        const handler = (e: BeforeUnloadEvent) => { e.preventDefault(); e.returnValue = ''; };
        window.addEventListener('beforeunload', handler);
        return () => window.removeEventListener('beforeunload', handler);
    }, [formTouched]);

    // 2️⃣ Block in-app navigation (react-router)
    const blocker = useBlocker(formTouched);
    useEffect(() => {
        if (blocker.state === 'blocked') {
            const leave = window.confirm('Data form belum disimpan. Yakin ingin pindah halaman?');
            if (leave) blocker.proceed();
            else blocker.reset();
        }
    }, [blocker]);

    // 3️⃣ Auto-save to sessionStorage (debounced 400ms)
    useEffect(() => {
        if (!initialLoadDone.current || !formDef) return;
        const t = setTimeout(() => {
            const draft = {
                tanggal: tanggal.toISOString(),
                lineId, shiftId, operatorEmpId,
                batterySlots, settings, problems, signatures,
            };
            sessionStorage.setItem(STORAGE_KEY, JSON.stringify(draft));
        }, 400);
        return () => clearTimeout(t);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [tanggal, lineId, shiftId, operatorEmpId, batterySlots, settings, problems, signatures, formDef, STORAGE_KEY]);

    // ===== HANDLERS (chain select) =====
    const handleLineChange = useCallback((e: { value?: number | null }) => {
        const newLineId = e.value ?? null;
        setLineId(newLineId);
        // Reset downstream
        setOperatorEmpId(null);
        setHierarchyIds({});
        setHierarchyNames({});
        // Clear battery slots (types change per line)
        setBatterySlots(prev => prev.map(() => ({ type: null, mold: null })));
        setSettings({});
        // Reload battery types + operators for this line
        if (newLineId) {
            getBatteryTypes(newLineId).then(setBatteryTypes).catch(() => setBatteryTypes(allBatteryTypes));
            getOperators(newLineId).then(setOperatorList).catch(() => {});
        } else {
            setBatteryTypes(allBatteryTypes);
            getOperators().then(setOperatorList).catch(() => {});
        }
    }, [allBatteryTypes]);

    const handleOperatorChange = useCallback((e: { value?: string }) => {
        const empId = e.value ?? null;
        setOperatorEmpId(empId);
        if (empId) {
            getHierarchy(empId).then(h => {
                setHierarchyIds({
                    leaderEmpId: h.leaderEmpId,
                    kasubsieEmpId: h.kasubsieEmpId,
                    kasieEmpId: h.kasieEmpId,
                });
                setHierarchyNames({
                    operator: h.operatorName ?? '',
                    leader: h.leaderName ?? '',
                    kasubsie: h.kasubsieName ?? '',
                    kasie: h.kasieName ?? '',
                });
            }).catch(() => {
                setHierarchyIds({});
                setHierarchyNames({});
            });
        } else {
            setHierarchyIds({});
            setHierarchyNames({});
        }
    }, []);

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

    const removeProblemRow = useCallback((id: number) => {
        setProblems(prev => prev.length > 1 ? prev.filter(p => p.id !== id) : prev);
    }, []);

    const updateProblem = useCallback((id: number, field: string, value: unknown) => {
        setProblems(prev =>
            prev.map(p => (p.id === id ? { ...p, [field]: value } : p))
        );
    }, []);

    const updateSignature = useCallback((key: string, dataUrl: string | null) => {
        setSignatures(prev => ({ ...prev, [key]: dataUrl }));
    }, []);

    // ===== COMPUTED =====
    const getMoldsForSlot = useCallback((_slotIdx: number) => {
        // Molds are from master data (TlkpMolds), available for all battery types
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

    // Get min/max for a check item at a specific slot
    const getMinMax = useCallback(
        (item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number): { min?: number; max?: number } => {
            // Sub-row level fixed min/max takes priority
            if (subRow?.fixedMin != null || subRow?.fixedMax != null) {
                return { min: subRow?.fixedMin ?? undefined, max: subRow?.fixedMax ?? undefined };
            }
            // Item level fixed min/max
            if (item.fixedMin != null || item.fixedMax != null) {
                return { min: item.fixedMin ?? undefined, max: item.fixedMax ?? undefined };
            }
            // Battery-type-dependent standards — look up from batteryTypes
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
        return operatorList.find(o => o.empId === operatorEmpId)?.name ?? '';
    }, [operatorEmpId, operatorList]);

    /** Map signature roleKey → empId for DB-based signature persistence */
    const getEmpIdForRole = useCallback((roleKey: string): string | null => {
        if (roleKey === 'operator') return operatorEmpId;
        if (roleKey === 'leader') return hierarchyIds.leaderEmpId ?? null;
        if (roleKey === 'kasubsie') return hierarchyIds.kasubsieEmpId ?? null;
        if (roleKey === 'kasie') return hierarchyIds.kasieEmpId ?? null;
        return null;
    }, [operatorEmpId, hierarchyIds]);

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
            if (settings[key] === undefined) {
                updateSetting(key, 'ng');
            }
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

        // Numeric input with out-of-range validation
        const numVal = settings[key] as number | null | undefined;
        const { min, max } = getMinMax(item, subRow, slotIdx);
        const outOfRange = isOutOfRange(numVal ?? null, min, max);

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

    function renderCheckItemRows(item: CheckItemDto): React.ReactNode {
        const rows = item.subRows && item.subRows.length > 0
            ? item.subRows
            : null;
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
                {/* Dynamic battery slots */}
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

    // ===== SUBMIT =====
    const handleSubmit = useCallback(async () => {
        if (!formDef || !operatorEmpId) {
            dxNotify('Harap lengkapi data operator terlebih dahulu.', 'warning', 3000);
            return;
        }

        // Check for NG items
        const hasNgItems = Object.values(settings).some(v => v === 'ng');

        if (hasNgItems) {
            const dialog = dxCustomDialog({
                title: '⚠️ Ditemukan Item NG',
                messageHtml: '<div style="text-align:center;font-size:14px;"><p>Terdapat item <b style="color:#dc3545;">NG (Not Good)</b> pada form ini.</p><p>Yakin ingin tetap submit?</p></div>',
                buttons: [
                    { text: 'Batal', onClick: () => false },
                    { text: 'Ya, Submit', onClick: () => true, type: 'danger' as unknown as string },
                ],
            });
            const confirmed = await dialog.show();
            if (!confirmed) return;
        }

        const payload: FormSubmissionPayload = {
            formId: formDef.id,
            tanggal: tanggal.toISOString(),
            lineId: lineId,
            shiftId: shiftId,
            operatorEmpId: operatorEmpId,
            leaderEmpId: hierarchyIds.leaderEmpId,
            kasubsieEmpId: hierarchyIds.kasubsieEmpId,
            kasieEmpId: hierarchyIds.kasieEmpId,
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
            const result = await submitFormSubmission(payload);
            sessionStorage.removeItem(STORAGE_KEY);
            setFormTouched(false);

            if (hasNgItems) {
                // Show SCW warning notification
                dxNotify({
                    message: '⚠️ Lakukan SCW (Stop Call Wait)!',
                    type: 'warning',
                    displayTime: 6000,
                    width: 380,
                    position: { my: 'top center', at: 'top center', offset: '0 60' },
                } as never);
            }

            dxNotify(`Check Sheet berhasil disimpan! (ID: ${result.id})`, 'success', 3000);
        } catch (err) {
            console.error('Submit error:', err);
            dxNotify('Gagal menyimpan. Lihat console untuk detail.', 'error', 3000);
        }
    }, [formDef, tanggal, lineId, shiftId, operatorEmpId, hierarchyIds, batterySlots, settings, problems, problemColumns, signatures, signatureSlots, STORAGE_KEY]);
    // ===== CLEAR FORM =====
    const handleClearForm = useCallback(() => {
        if (!window.confirm('Yakin ingin menghapus semua data form?\nData yang belum disimpan akan hilang.')) return;
        setTanggal(new Date());
        setLineId(null);
        setShiftId(null);
        setOperatorEmpId(null);
        setBatteryTypes(allBatteryTypes);
        getOperators().then(setOperatorList).catch(() => {});
        setHierarchyIds({});
        setHierarchyNames({});
        setBatterySlots(Array.from({ length: slotCount }, () => ({ type: null, mold: null })));
        setSettings({});
        const emptyRow: ProblemRow = { id: Date.now() };
        problemColumns.forEach(col => {
            emptyRow[col.columnKey] = col.fieldType === 'number' ? null : '';
        });
        setProblems([emptyRow]);
        const sigs: Record<string, string | null> = {};
        signatureSlots.forEach(slot => { sigs[slot.roleKey] = null; });
        setSignatures(sigs);
        sessionStorage.removeItem(STORAGE_KEY);
        setFormTouched(false);
    }, [slotCount, problemColumns, signatureSlots, allBatteryTypes, STORAGE_KEY]);
    // ===== MAIN RENDER =====
    if (loading || !formDef) {
        return (
            <div className="cos-form-page" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
                <p>Memuat data...</p>
            </div>
        );
    }

    return (
        <React.Fragment>
            <div className="content-block">
                <div className="cos-form-page">
                    <div className="cos-form">
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
                                            {slot.roleKey === 'operator' ? (
                                                <SignaturePad
                                                    label={slot.label}
                                                    name={hierarchyNames[slot.roleKey] || operatorName || slot.roleKey}
                                                    empId={getEmpIdForRole(slot.roleKey)}
                                                    onChange={(d) => updateSignature(slot.roleKey, d)}
                                                />
                                            ) : (
                                                <div className="signature-display-only">
                                                    <div className="signature-display-label">{slot.label}</div>
                                                    <div className="signature-display-placeholder">—</div>
                                                    <div className="signature-display-name">{hierarchyNames[slot.roleKey] || '-'}</div>
                                                </div>
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
                                            <DateBox
                                                value={tanggal}
                                                onValueChanged={(e) => setTanggal(e.value)}
                                                type="date"
                                                displayFormat="dd/MM/yyyy"
                                                stylingMode="underlined"
                                                height={30}
                                                width={180}
                                            />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td className="info-label">Line / Shift</td>
                                        <td className="info-sep">:</td>
                                        <td className="info-value">
                                            <div className="line-shift-row">
                                                <SelectBox
                                                    dataSource={lineList}
                                                    valueExpr="id"
                                                    displayExpr="name"
                                                    value={lineId}
                                                    onValueChanged={handleLineChange}
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
                                        </td>
                                    </tr>
                                    <tr>
                                        <td className="info-label">Operator</td>
                                        <td className="info-sep">:</td>
                                        <td className="info-value">
                                            <SelectBox
                                                dataSource={operatorList}
                                                displayExpr="name"
                                                valueExpr="empId"
                                                value={operatorEmpId}
                                                onValueChanged={handleOperatorChange}
                                                searchEnabled={true}
                                                searchMode="contains"
                                                placeholder={lineId ? 'Pilih operator...' : 'Pilih Line dulu...'}
                                                stylingMode="underlined"
                                                height={30}
                                                width={280}
                                                showClearButton={true}
                                                disabled={!lineId}
                                            />
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

                        {/* ==================== SECTION B: PROBLEM (DYNAMIC COLUMNS) ==================== */}
                        <div className="section-b">
                            <div className="section-title">B. PROBLEM</div>
                            <table className="problems-table">
                                <thead>
                                    <tr>
                                        <th style={{ width: '30px' }}>No</th>
                                        {problemColumns.map(col => (
                                            <th key={col.columnKey} style={{ width: col.width }}>{col.label}</th>
                                        ))}
                                        <th style={{ width: '40px' }}></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {problems.map((prob, idx) => (
                                        <tr key={prob.id}>
                                            <td className="problem-no">{idx + 1}</td>
                                            {problemColumns.map(col => (
                                                <td key={col.columnKey}>
                                                    {col.fieldType === 'number' ? (
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
                                                    )}
                                                </td>
                                            ))}
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
                                        </tr>
                                    ))}
                                </tbody>
                            </table>
                            <div className="add-problem-row">
                                <Button
                                    text="+ Tambah Baris Problem"
                                    stylingMode="outlined"
                                    type="default"
                                    onClick={addProblemRow}
                                />
                            </div>
                        </div>

                        {/* ==================== FOOTER / SUBMIT ==================== */}
                        <div className="form-footer">
                            <Button
                                text="Clear Form"
                                icon="revert"
                                type="danger"
                                stylingMode="outlined"
                                height={36}
                                onClick={handleClearForm}
                            />
                            <Button
                                text="Simpan Check Sheet"
                                type="default"
                                stylingMode="contained"
                                width={200}
                                height={36}
                                onClick={handleSubmit}
                            />
                            <a href="/admin" className="admin-link" style={{ marginLeft: 16, fontSize: 12, opacity: 0.6 }}>
                                Admin Panel &rarr;
                            </a>
                        </div>
                    </div>
                </div>
            </div>
        </React.Fragment>
    );
}
