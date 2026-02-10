import { useEffect, useState, useCallback } from 'react';
import TabPanel, { Item as TabItem } from 'devextreme-react/tab-panel';
import DataGrid, { Column, Editing, Paging, Lookup } from 'devextreme-react/data-grid';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  adminGetBatteryTypes,
  adminCreateBatteryType, adminUpdateBatteryType, adminDeleteBatteryType,
  adminCreateBatteryMold, adminUpdateBatteryMold, adminDeleteBatteryMold,
  adminCreateBatteryStandard, adminUpdateBatteryStandard, adminDeleteBatteryStandard,
  type AdminBatteryTypeDto, type BatteryStandardDto,
} from '../../api/cos-api';

interface FlatMold {
  id: number;
  name: string;
  batteryTypeId: number;
  batteryTypeName: string;
}

interface FlatStandard extends BatteryStandardDto {
  batteryTypeName: string;
}

export function BatteryTypesPage() {
  const [batteryTypes, setBatteryTypes] = useState<AdminBatteryTypeDto[]>([]);
  const [flatMolds, setFlatMolds] = useState<FlatMold[]>([]);
  const [flatStandards, setFlatStandards] = useState<FlatStandard[]>([]);
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const types = await adminGetBatteryTypes();
      setBatteryTypes(types);

      const molds: FlatMold[] = [];
      const stds: FlatStandard[] = [];
      types.forEach(bt => {
        bt.molds.forEach(m => molds.push({ ...m, batteryTypeName: bt.name }));
        bt.standards.forEach(s => stds.push({ ...s, batteryTypeName: bt.name }));
      });
      setFlatMolds(molds);
      setFlatStandards(stds);
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

  // ═══════════ MOLDS ═══════════
  const onMoldInserted = async (e: { data: FlatMold }) => {
    try {
      await adminCreateBatteryMold({ name: e.data.name, batteryTypeId: e.data.batteryTypeId });
      notify('Mold created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create mold', 'error', 3000); }
  };
  const onMoldUpdated = async (e: { data: FlatMold }) => {
    try {
      await adminUpdateBatteryMold(e.data.id, { name: e.data.name });
      notify('Mold updated', 'success', 1500);
    } catch { notify('Failed to update mold', 'error', 3000); }
  };
  const onMoldRemoved = async (e: { data: FlatMold }) => {
    try {
      await adminDeleteBatteryMold(e.data.id);
      notify('Mold deleted', 'success', 1500);
      loadData();
    } catch { notify('Failed to delete mold', 'error', 3000); }
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
                caption="Jumlah Mold"
                allowEditing={false}
                calculateCellValue={(row: AdminBatteryTypeDto) => row.molds?.length ?? 0}
                width={100}
                alignment="center"
              />
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

        {/* ═══════════ MOLDS TAB ═══════════ */}
        <TabItem title={`Molds (${flatMolds.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={flatMolds}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onMoldInserted}
              onRowUpdated={onMoldUpdated}
              onRowRemoved={onMoldRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={30} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="batteryTypeId" caption="Battery Type">
                <Lookup dataSource={btLookup} valueExpr="id" displayExpr="name" />
              </Column>
              <Column dataField="name" caption="Nama Mold" />
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
