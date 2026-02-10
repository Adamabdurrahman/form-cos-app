import { useEffect, useState, useCallback } from 'react';
import TabPanel, { Item as TabItem } from 'devextreme-react/tab-panel';
import DataGrid, { Column, Editing, Paging, Lookup, FilterRow } from 'devextreme-react/data-grid';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  adminGetBatteryTypes,
  adminCreateBatteryType, adminUpdateBatteryType, adminDeleteBatteryType,
  adminCreateBatteryStandard, adminUpdateBatteryStandard, adminDeleteBatteryStandard,
  adminGetMolds,
  type AdminBatteryTypeDto, type BatteryStandardDto,
} from '../../api/cos-api';

interface FlatStandard extends BatteryStandardDto {
  batteryTypeName: string;
}

interface MoldRow {
  moldCode: string;
  moldDescription: string;
  moldStatus: string;
  idSection: number | null;
}

export function BatteryTypesPage() {
  const [batteryTypes, setBatteryTypes] = useState<AdminBatteryTypeDto[]>([]);
  const [flatStandards, setFlatStandards] = useState<FlatStandard[]>([]);
  const [molds, setMolds] = useState<MoldRow[]>([]);
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [types, moldData] = await Promise.all([
        adminGetBatteryTypes(),
        adminGetMolds(),
      ]);
      setBatteryTypes(types);

      const stds: FlatStandard[] = [];
      types.forEach(bt => {
        bt.standards.forEach(s => stds.push({ ...s, batteryTypeName: bt.name }));
      });
      setFlatStandards(stds);
      setMolds(moldData);
    } catch (e) {
      console.error(e);
      notify('Failed to load battery types', 'error', 3000);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  // ═══════════ BATTERY TYPE ═══════════
  const onTypeInserted = async (e: { data: AdminBatteryTypeDto }) => {
    try {
      await adminCreateBatteryType({ name: e.data.name });
      notify('Battery type created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create battery type', 'error', 3000); }
  };
  const onTypeUpdated = async (e: { data: AdminBatteryTypeDto }) => {
    try {
      await adminUpdateBatteryType(e.data.id, { name: e.data.name });
      notify('Battery type updated', 'success', 1500);
    } catch { notify('Failed to update battery type', 'error', 3000); }
  };
  const onTypeRemoved = async (e: { data: AdminBatteryTypeDto }) => {
    try {
      await adminDeleteBatteryType(e.data.id);
      notify('Battery type deleted', 'success', 1500);
      loadData();
    } catch { notify('Failed to delete battery type', 'error', 3000); }
  };

  // ═══════════ STANDARDS ═══════════
  const onStdInserted = async (e: { data: FlatStandard }) => {
    try {
      await adminCreateBatteryStandard({
        paramKey: e.data.paramKey,
        value: e.data.value,
        minValue: e.data.minValue,
        maxValue: e.data.maxValue,
        batteryTypeId: e.data.batteryTypeId,
      });
      notify('Standard created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create standard', 'error', 3000); }
  };
  const onStdUpdated = async (e: { data: FlatStandard }) => {
    try {
      await adminUpdateBatteryStandard(e.data.id, {
        paramKey: e.data.paramKey,
        value: e.data.value,
        minValue: e.data.minValue,
        maxValue: e.data.maxValue,
      });
      notify('Standard updated', 'success', 1500);
    } catch { notify('Failed to update standard', 'error', 3000); }
  };
  const onStdRemoved = async (e: { data: FlatStandard }) => {
    try {
      await adminDeleteBatteryStandard(e.data.id);
      notify('Standard deleted', 'success', 1500);
      loadData();
    } catch { notify('Failed to delete standard', 'error', 3000); }
  };

  const btLookup = batteryTypes.map(bt => ({ id: bt.id, name: bt.name }));

  return (
    <div>
      <h2 style={{ marginBottom: 16 }}>Battery Types Management</h2>
      <LoadPanel visible={loading} />

      <TabPanel>
        {/* ═══════════ BATTERY TYPES TAB ═══════════ */}
        <TabItem title={`Types (${batteryTypes.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={batteryTypes}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onTypeInserted}
              onRowUpdated={onTypeUpdated}
              onRowRemoved={onTypeRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={20} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="name" caption="Nama Type Battery" />
              <Column
                caption="Jumlah Standard"
                allowEditing={false}
                calculateCellValue={(row: AdminBatteryTypeDto) => row.standards?.length ?? 0}
                width={120}
                alignment="center"
              />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ MOLDS TAB (read-only, master data) ═══════════ */}
        <TabItem title={`Molds (${molds.length})`}>
          <div style={{ padding: 16 }}>
            <p style={{ color: '#888', fontSize: 13, marginBottom: 12 }}>
              Data mold berasal dari master data (tlkp_mold) — read-only.
            </p>
            <DataGrid dataSource={molds} keyExpr="moldCode" showBorders columnAutoWidth>
              <FilterRow visible />
              <Paging defaultPageSize={30} />
              <Column dataField="moldCode" caption="Kode Mold" width={120} />
              <Column dataField="moldDescription" caption="Deskripsi" />
              <Column dataField="moldStatus" caption="Status" width={80} />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ STANDARDS TAB ═══════════ */}
        <TabItem title={`Standards (${flatStandards.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={flatStandards}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onStdInserted}
              onRowUpdated={onStdUpdated}
              onRowRemoved={onStdRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <FilterRow visible />
              <Paging defaultPageSize={50} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="batteryTypeId" caption="Battery Type">
                <Lookup dataSource={btLookup} valueExpr="id" displayExpr="name" />
              </Column>
              <Column dataField="paramKey" caption="Parameter Key" />
              <Column dataField="value" caption="Value (Standar)" />
              <Column dataField="minValue" caption="Min Value" dataType="number" width={100} />
              <Column dataField="maxValue" caption="Max Value" dataType="number" width={100} />
            </DataGrid>
          </div>
        </TabItem>
      </TabPanel>
    </div>
  );
}
