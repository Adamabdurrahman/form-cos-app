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
    getCheckItems,
    getHierarchy,
    submitCosValidation,
    type OperatorDto,
    type BatteryTypeDto,
    type CheckItemDto,
    type SubRowDto,
} from '../../api/cos-api';
import SignaturePad from '../../components/signature-pad/SignaturePad';
import gsLogo from '../../assets/GS.png';
import './cos-validation.scss';

// ====================== TYPES ======================

interface ProblemRow {
    id: number;
    item: string;
    masalah: string;
    tindakan: string;
    waktu: string;
    menit: number | null;
    pic: string;
}

interface BatterySlot {
    type: string | null;
    mold: string | null;
}

// ====================== COMPONENT ======================

export function CosValidation() {
    // ===== REFERENCE DATA (from API) =====
    const [operatorList, setOperatorList] = useState<OperatorDto[]>([]);
    const [lineList, setLineList] = useState<number[]>([]);
    const [shiftList, setShiftList] = useState<number[]>([]);
    const [batteryTypes, setBatteryTypes] = useState<BatteryTypeDto[]>([]);
    const [checkItemsList, setCheckItemsList] = useState<CheckItemDto[]>([]);
    const [loading, setLoading] = useState(true);

    // ===== HEADER STATE =====
    const [tanggal, setTanggal] = useState<Date>(new Date());
    const [line, setLine] = useState<number | null>(null);
    const [shift, setShift] = useState<number | null>(null);
    const [operatorId, setOperatorId] = useState<number | null>(null);
    const [leaderName, setLeaderName] = useState('');
    const [kasubsieName, setKasubsieName] = useState('');
    const [kasieName, setKasieName] = useState('');

    // ===== BATTERY SLOTS =====
    const [batterySlots, setBatterySlots] = useState<BatterySlot[]>([
        { type: null, mold: null },
        { type: null, mold: null },
        { type: null, mold: null },
    ]);

    // ===== SETTINGS (check item values) =====
    const [settings, setSettings] = useState<Record<string, unknown>>({});

    // ===== PROBLEMS =====
    const [problems, setProblems] = useState<ProblemRow[]>([
        { id: 1, item: '', masalah: '', tindakan: '', waktu: '', menit: null, pic: '' },
    ]);

    // ===== SIGNATURES =====
    const [signatures, setSignatures] = useState<Record<string, string | null>>({
        operator: null,
        leader: null,
        kasubsie: null,
        kasie: null,
    });

    // ===== LOAD REFERENCE DATA FROM API =====
    useEffect(() => {
        async function loadData() {
            try {
                const [ops, lines, shifts, btypes, citems] = await Promise.all([
                    getOperators(),
                    getLines(),
                    getShifts(),
                    getBatteryTypes(),
                    getCheckItems(),
                ]);
                setOperatorList(ops);
                setLineList(lines);
                setShiftList(shifts);
                setBatteryTypes(btypes);
                setCheckItemsList(citems);
            } catch (err) {
                console.error('Failed to load reference data:', err);
            } finally {
                setLoading(false);
            }
        }
        loadData();
    }, []);

    // ===== HANDLERS =====
    const handleOperatorChange = useCallback((e: { value?: number }) => {
        const opId = e.value ?? null;
        setOperatorId(opId);
        if (opId) {
            getHierarchy(opId).then(h => {
                setLeaderName(h.leader?.name ?? '');
                setKasubsieName(h.kasubsie?.name ?? '');
                setKasieName(h.kasie?.name ?? '');
            }).catch(() => {
                setLeaderName('');
                setKasubsieName('');
                setKasieName('');
            });
        } else {
            setLeaderName('');
            setKasubsieName('');
            setKasieName('');
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
        setProblems(prev => [
            ...prev,
            { id: Date.now(), item: '', masalah: '', tindakan: '', waktu: '', menit: null, pic: '' },
        ]);
    }, []);

    const removeProblemRow = useCallback((id: number) => {
        setProblems(prev => prev.length > 1 ? prev.filter(p => p.id !== id) : prev);
    }, []);

    const updateProblem = useCallback((id: number, field: keyof ProblemRow, value: unknown) => {
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
    }, [batterySlots]);

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

    const operatorName = useMemo(() => {
        if (!operatorId) return '';
        return operatorList.find(o => o.id === operatorId)?.name ?? '';
    }, [operatorId]);

    // ===== RENDER HELPERS =====

    function renderSettingInput(item: CheckItemDto, subRow: SubRowDto | null, slotIdx: number) {
        const key = subRow?.suffix
            ? `${item.id}_${subRow.suffix}_${slotIdx}`
            : `${item.id}_${slotIdx}`;

        if (!batterySlots[slotIdx]?.type) {
            return <span className="empty-cell">—</span>;
        }

        if (item.type === 'visual') {
            const val = settings[key] as ('ok' | 'ng' | undefined);
            const cycle = () => {
                if (!val) updateSetting(key, 'ok');
                else if (val === 'ok') updateSetting(key, 'ng');
                else updateSetting(key, undefined);
            };
            return (
                <div className={`visual-check-btn ${val ?? 'empty'}`} onClick={cycle} title="Klik untuk ubah">
                    {val === 'ok' ? '✓' : val === 'ng' ? '✗' : '—'}
                </div>
            );
        }

        return (
            <NumberBox
                value={(settings[key] as number) ?? undefined}
                onValueChanged={(e) => updateSetting(key, e.value)}
                stylingMode="underlined"
                height={26}
                showSpinButtons={false}
            />
        );
    }

    function renderCheckItemRows(item: CheckItemDto): React.ReactNode {
        const rows = item.subRows && item.subRows.length > 0
            ? item.subRows
            : null;
        const hasSubRows = !!rows;
        const rowList = rows ?? [null];
        const rowCount = rowList.length;

        return rowList.map((subRow, subIdx) => (
            <tr key={`${item.id}_${subRow?.suffix ?? 'main'}`} className="check-row">
                {/* Item name */}
                {subIdx === 0 && hasSubRows && (
                    <td rowSpan={rowCount} className="item-name-cell">{item.label}</td>
                )}
                {!hasSubRows && (
                    <td colSpan={2} className="item-name-cell">{item.label}</td>
                )}
                {/* Sub-row label */}
                {hasSubRows && (
                    <td className="sub-label-cell">{subRow?.label}</td>
                )}
                {/* 3 battery slots: Standar + Setting */}
                {[0, 1, 2].map(slotIdx => (
                    <React.Fragment key={slotIdx}>
                        <td className="standard-cell">{getStandard(item, subRow, slotIdx)}</td>
                        <td className="setting-cell">{renderSettingInput(item, subRow, slotIdx)}</td>
                    </React.Fragment>
                ))}
                {/* Frequency + Keterangan (only first sub-row) */}
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

    // ===== MAIN RENDER =====
    if (loading) {
        return (
            <div className="cos-form-page" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '50vh' }}>
                <div style={{ textAlign: 'center' }}>
                    <p>Memuat data...</p>
                </div>
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
                                <h3 className="form-title">VALIDASI PROSES COS</h3>
                                <div className="form-id">Form-A2 1-K.051-5-2</div>
                            </div>
                            <div className="header-right">
                                <div className="approval-section">
                                    <div className="approval-box">
                                        <SignaturePad
                                            label="Dibuat"
                                            name={operatorName || 'Operator'}
                                            onChange={(d) => updateSignature('operator', d)}
                                        />
                                    </div>
                                    <div className="approval-box">
                                        <SignaturePad
                                            label="Diperiksa"
                                            name={leaderName || 'Leader'}
                                            onChange={(d) => updateSignature('leader', d)}
                                        />
                                    </div>
                                    <div className="approval-box">
                                        <SignaturePad
                                            label="Diketahui"
                                            name={kasubsieName || 'Kasubsie'}
                                            onChange={(d) => updateSignature('kasubsie', d)}
                                        />
                                    </div>
                                    <div className="approval-box">
                                        <SignaturePad
                                            label="Disetujui"
                                            name={kasieName || 'Kasie'}
                                            onChange={(d) => updateSignature('kasie', d)}
                                        />
                                    </div>
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

                        {/* ==================== SECTION A: VALIDASI COS ==================== */}
                        <div className="section-a">
                            <div className="section-title">A. VALIDASI COS</div>
                            <div className="table-scroll-wrapper">
                                <table className="cos-table">
                                    <colgroup>
                                        <col style={{ width: '160px' }} /> {/* Item Check */}
                                        <col style={{ width: '40px' }} />  {/* Sub-label */}
                                        <col style={{ width: '85px' }} />   {/* Std 1 */}
                                        <col style={{ width: '80px' }} />   {/* Set 1 */}
                                        <col style={{ width: '85px' }} />   {/* Std 2 */}
                                        <col style={{ width: '80px' }} />   {/* Set 2 */}
                                        <col style={{ width: '85px' }} />   {/* Std 3 */}
                                        <col style={{ width: '80px' }} />   {/* Set 3 */}
                                        <col style={{ width: '100px' }} />  {/* Frekuensi */}
                                        <col style={{ width: '100px' }} />  {/* Ket */}
                                    </colgroup>
                                    <thead>
                                        {/* Row 1: TYPE BATTERY + Battery selectors */}
                                        <tr>
                                            <th colSpan={2} rowSpan={2} className="header-item">ITEM CHECK</th>
                                            {[0, 1, 2].map(slotIdx => (
                                                <th colSpan={2} key={`type_${slotIdx}`} className="header-type">
                                                    <div className="type-header-label">TYPE BATTERY</div>
                                                    <SelectBox
                                                        items={batteryTypes.map(t => t.name)}
                                                        value={batterySlots[slotIdx].type}
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
                                        {/* Row 2: Standar | Setting headers */}
                                        <tr>
                                            {[0, 1, 2].map(slotIdx => (
                                                <React.Fragment key={`hdr_${slotIdx}`}>
                                                    <th className="header-std">Standar</th>
                                                    <th className="header-set">Setting</th>
                                                </React.Fragment>
                                            ))}
                                        </tr>
                                        {/* Row 3: NO MOLD + Mold selectors */}
                                        <tr>
                                            <th colSpan={2} className="header-mold-label">NO MOLD</th>
                                            {[0, 1, 2].map(slotIdx => (
                                                <th colSpan={2} key={`mold_${slotIdx}`} className="header-mold">
                                                    <SelectBox
                                                        items={getMoldsForSlot(slotIdx)}
                                                        value={batterySlots[slotIdx].mold}
                                                        onValueChanged={(e) => updateSlot(slotIdx, 'mold', e.value)}
                                                        placeholder="Mold"
                                                        stylingMode="underlined"
                                                        height={26}
                                                        disabled={!batterySlots[slotIdx].type}
                                                        showClearButton={true}
                                                    />
                                                </th>
                                            ))}
                                            <th></th>
                                            <th></th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {checkItemsList.map(item => renderCheckItemRows(item))}
                                    </tbody>
                                </table>
                            </div>
                        </div>

                        {/* ==================== SECTION B: PROBLEM ==================== */}
                        <div className="section-b">
                            <div className="section-title">B. PROBLEM</div>
                            <table className="problems-table">
                                <thead>
                                    <tr>
                                        <th style={{ width: '30px' }}>No</th>
                                        <th style={{ width: '120px' }}>ITEM</th>
                                        <th style={{ width: '200px' }}>MASALAH</th>
                                        <th style={{ width: '200px' }}>TINDAKAN</th>
                                        <th style={{ width: '80px' }}>WAKTU</th>
                                        <th style={{ width: '60px' }}>MENIT</th>
                                        <th style={{ width: '100px' }}>PIC</th>
                                        <th style={{ width: '40px' }}></th>
                                    </tr>
                                </thead>
                                <tbody>
                                    {problems.map((prob, idx) => (
                                        <tr key={prob.id}>
                                            <td className="problem-no">{idx + 1}</td>
                                            <td>
                                                <TextBox
                                                    value={prob.item}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'item', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                />
                                            </td>
                                            <td>
                                                <TextBox
                                                    value={prob.masalah}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'masalah', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                />
                                            </td>
                                            <td>
                                                <TextBox
                                                    value={prob.tindakan}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'tindakan', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                />
                                            </td>
                                            <td>
                                                <TextBox
                                                    value={prob.waktu}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'waktu', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                    placeholder="HH:mm"
                                                />
                                            </td>
                                            <td>
                                                <NumberBox
                                                    value={prob.menit ?? undefined}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'menit', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                    min={0}
                                                />
                                            </td>
                                            <td>
                                                <TextBox
                                                    value={prob.pic}
                                                    onValueChanged={(e) => updateProblem(prob.id, 'pic', e.value)}
                                                    stylingMode="underlined"
                                                    height={28}
                                                />
                                            </td>
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
                                onClick={async () => {
                                    // Find leader/kasubsie/kasie IDs from hierarchy
                                    let leaderId: number | undefined;
                                    let kasubsieId: number | undefined;
                                    let kasieId: number | undefined;
                                    if (operatorId) {
                                        try {
                                            const h = await getHierarchy(operatorId);
                                            leaderId = h.leader?.id;
                                            kasubsieId = h.kasubsie?.id;
                                            kasieId = h.kasie?.id;
                                        } catch { /* ignore */ }
                                    }

                                    const payload = {
                                        tanggal: tanggal.toISOString(),
                                        line: line ?? 0,
                                        shift: shift ?? 0,
                                        operatorId: operatorId ?? 0,
                                        leaderId,
                                        kasubsieId,
                                        kasieId,
                                        batteryType1: batterySlots[0]?.type ?? undefined,
                                        mold1: batterySlots[0]?.mold ?? undefined,
                                        batteryType2: batterySlots[1]?.type ?? undefined,
                                        mold2: batterySlots[1]?.mold ?? undefined,
                                        batteryType3: batterySlots[2]?.type ?? undefined,
                                        mold3: batterySlots[2]?.mold ?? undefined,
                                        settings: Object.fromEntries(
                                            Object.entries(settings)
                                                .filter(([, v]) => v !== undefined)
                                                .map(([k, v]) => [k, v != null ? String(v) : null])
                                        ),
                                        problems: problems.map(p => ({
                                            problem: p.masalah || undefined,
                                            action: p.tindakan || undefined,
                                        })),
                                        signatures,
                                    };

                                    try {
                                        const result = await submitCosValidation(payload);
                                        alert(`Check Sheet berhasil disimpan! (ID: ${result.id})`);
                                    } catch (err) {
                                        console.error('Submit error:', err);
                                        alert('Gagal menyimpan. Lihat console untuk detail.');
                                    }
                                }}
                            />
                        </div>
                    </div>
                </div>
            </div>
        </React.Fragment>
    );
}
