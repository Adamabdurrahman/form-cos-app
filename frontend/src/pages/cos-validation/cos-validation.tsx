import React, { useState, useCallback, useMemo, useEffect } from 'react';
import DateBox from 'devextreme-react/date-box';
import SelectBox from 'devextreme-react/select-box';
import NumberBox from 'devextreme-react/number-box';
import TextBox from 'devextreme-react/text-box';
import Button from 'devextreme-react/button';
import {
    getOperators,
    getBatteryTypes,
    getLines,
    getShifts,
    getHierarchy,
    getFormDefinitionByCode,
    submitFormSubmission,
    type OperatorDto,
    type BatteryTypeDto,
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
    const [lineList, setLineList] = useState<number[]>([]);
    const [shiftList, setShiftList] = useState<number[]>([]);
    const [batteryTypes, setBatteryTypes] = useState<BatteryTypeDto[]>([]);
    const [loading, setLoading] = useState(true);

    // ===== HEADER STATE =====
    const [tanggal, setTanggal] = useState<Date>(new Date());
    const [line, setLine] = useState<number | null>(null);
    const [shift, setShift] = useState<number | null>(null);
    const [operatorId, setOperatorId] = useState<number | null>(null);
    const [hierarchyIds, setHierarchyIds] = useState<{ leaderId?: number; kasubsieId?: number; kasieId?: number }>({});
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

    // ===== LOAD DATA =====
    useEffect(() => {
        async function loadData() {
            try {
                const [ops, lines, shifts, btypes, fd] = await Promise.all([
                    getOperators(),
                    getLines(),
                    getShifts(),
                    getBatteryTypes(),
                    getFormDefinitionByCode(formCode),
                ]);
                setOperatorList(ops);
                setLineList(lines);
                setShiftList(shifts);
                setBatteryTypes(btypes);
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
            } catch (err) {
                console.error('Failed to load data:', err);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, [formCode]);

    // ===== HANDLERS =====
    const handleOperatorChange = useCallback((e: { value?: number }) => {
        const opId = e.value ?? null;
        setOperatorId(opId);
        if (opId) {
            getHierarchy(opId).then(h => {
                setHierarchyIds({
                    leaderId: h.leader?.id,
                    kasubsieId: h.kasubsie?.id,
                    kasieId: h.kasie?.id,
                });
                setHierarchyNames({
                    operator: h.operator?.name ?? '',
                    leader: h.leader?.name ?? '',
                    kasubsie: h.kasubsie?.name ?? '',
                    kasie: h.kasie?.name ?? '',
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
    const getMoldsForSlot = useCallback((slotIdx: number) => {
        const bt = batterySlots[slotIdx]?.type;
        if (!bt) return [];
        return batteryTypes.find(t => t.name === bt)?.molds ?? [];
    }, [batterySlots, batteryTypes]);

    const getStandard = useCallback(
        (item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number): string => {
            if (subRow?.fixedStandard) return subRow.fixedStandard;
            if (item.type === 'visual') return item.visualStandard ?? '';
            if (item.fixedStandard) return item.fixedStandard;
            const bt = batterySlots[slotIdx]?.type;
            if (!bt || !item.numericStdKey) return '';
            const typeData = batteryTypes.find(t => t.name === bt);
            return typeData?.standards[item.numericStdKey] ?? '';
        },
        [batterySlots, batteryTypes]
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
                const bt = batterySlots[slotIdx]?.type;
                if (bt) {
                    const typeData = batteryTypes.find(t => t.name === bt);
                    const stdStr = typeData?.standards[item.numericStdKey] ?? '';
                    if (stdStr && stdStr !== '-') {
                        const match = stdStr.match(/([\d.]+)\s*-\s*([\d.]+)/);
                        if (match) return { min: parseFloat(match[1]), max: parseFloat(match[2]) };
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
        if (!operatorId) return '';
        return operatorList.find(o => o.id === operatorId)?.name ?? '';
    }, [operatorId, operatorList]);

    // ===== RENDER HELPERS =====

    function renderSettingInput(item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number) {
        const key = subRow?.suffix
            ? `${item.itemKey}_${subRow.suffix}_${slotIdx}`
            : `${item.itemKey}_${slotIdx}`;

        if (!batterySlots[slotIdx]?.type) {
            return <span className="empty-cell">—</span>;
        }

        if (item.type === 'visual') {
            const val = (settings[key] as ('ok' | 'ng' | undefined)) ?? 'ok';
            if (settings[key] === undefined) {
                updateSetting(key, 'ok');
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
        if (!formDef || !operatorId) {
            alert('Harap lengkapi data operator terlebih dahulu.');
            return;
        }

        const payload: FormSubmissionPayload = {
            formId: formDef.id,
            tanggal: tanggal.toISOString(),
            line: line ?? 0,
            shift: shift ?? 0,
            operatorId: operatorId,
            leaderId: hierarchyIds.leaderId,
            kasubsieId: hierarchyIds.kasubsieId,
            kasieId: hierarchyIds.kasieId,
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
            alert(`Check Sheet berhasil disimpan! (ID: ${result.id})`);
        } catch (err) {
            console.error('Submit error:', err);
            alert('Gagal menyimpan. Lihat console untuk detail.');
        }
    }, [formDef, tanggal, line, shift, operatorId, hierarchyIds, batterySlots, settings, problems, problemColumns, signatures, signatureSlots]);

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
                                            <SignaturePad
                                                label={slot.label}
                                                name={hierarchyNames[slot.roleKey] || operatorName || slot.roleKey}
                                                onChange={(d) => updateSignature(slot.roleKey, d)}
                                            />
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
                                                    items={lineList}
                                                    value={line}
                                                    onValueChanged={(e) => setLine(e.value)}
                                                    placeholder="Line"
                                                    stylingMode="underlined"
                                                    height={30}
                                                    width={90}
                                                />
                                                <span className="separator">/</span>
                                                <SelectBox
                                                    items={shiftList}
                                                    value={shift}
                                                    onValueChanged={(e) => setShift(e.value)}
                                                    placeholder="Shift"
                                                    stylingMode="underlined"
                                                    height={30}
                                                    width={90}
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
                                                valueExpr="id"
                                                value={operatorId}
                                                onValueChanged={handleOperatorChange}
                                                searchEnabled={true}
                                                searchMode="contains"
                                                placeholder="Ketik nama operator..."
                                                stylingMode="underlined"
                                                height={30}
                                                width={280}
                                                showClearButton={true}
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
