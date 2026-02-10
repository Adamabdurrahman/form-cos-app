import { useEffect, useState, useCallback } from 'react';
import TabPanel, { Item as TabItem } from 'devextreme-react/tab-panel';
import DataGrid, { Column, Editing, Paging, Lookup } from 'devextreme-react/data-grid';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  adminGetPersonnel,
  adminCreateKasie, adminUpdateKasie, adminDeleteKasie,
  adminCreateKasubsie, adminUpdateKasubsie, adminDeleteKasubsie,
  adminCreateLeader, adminUpdateLeader, adminDeleteLeader,
  adminCreateOperator, adminUpdateOperator, adminDeleteOperator,
  type KasieDto, type KasubsieDto, type LeaderDto, type OperatorDto,
} from '../../api/cos-api';

export function PersonnelPage() {
  const [kasies, setKasies] = useState<KasieDto[]>([]);
  const [kasubsies, setKasubsies] = useState<KasubsieDto[]>([]);
  const [leaders, setLeaders] = useState<LeaderDto[]>([]);
  const [operators, setOperators] = useState<OperatorDto[]>([]);
  const [loading, setLoading] = useState(true);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const data = await adminGetPersonnel();
      setKasies(data.kasies);
      setKasubsies(data.kasubsies);
      setLeaders(data.leaders);
      setOperators(data.operators);
    } catch (e) {
      console.error(e);
      notify('Failed to load personnel', 'error', 3000);
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadData(); }, [loadData]);

  // ═══════════ KASIE ═══════════
  const onKasieInserted = async (e: { data: KasieDto }) => {
    try {
      await adminCreateKasie({ name: e.data.name });
      notify('Kasie created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create Kasie', 'error', 3000); }
  };
  const onKasieUpdated = async (e: { data: KasieDto }) => {
    try {
      await adminUpdateKasie(e.data.id, { name: e.data.name });
      notify('Kasie updated', 'success', 1500);
    } catch { notify('Failed to update Kasie', 'error', 3000); }
  };
  const onKasieRemoved = async (e: { data: KasieDto }) => {
    try {
      await adminDeleteKasie(e.data.id);
      notify('Kasie deleted', 'success', 1500);
    } catch { notify('Failed to delete Kasie', 'error', 3000); }
  };

  // ═══════════ KASUBSIE ═══════════
  const onKasubsieInserted = async (e: { data: KasubsieDto }) => {
    try {
      await adminCreateKasubsie({ name: e.data.name, kasieId: e.data.kasieId });
      notify('Kasubsie created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create Kasubsie', 'error', 3000); }
  };
  const onKasubsieUpdated = async (e: { data: KasubsieDto }) => {
    try {
      await adminUpdateKasubsie(e.data.id, { name: e.data.name, kasieId: e.data.kasieId });
      notify('Kasubsie updated', 'success', 1500);
    } catch { notify('Failed to update Kasubsie', 'error', 3000); }
  };
  const onKasubsieRemoved = async (e: { data: KasubsieDto }) => {
    try {
      await adminDeleteKasubsie(e.data.id);
      notify('Kasubsie deleted', 'success', 1500);
    } catch { notify('Failed to delete Kasubsie', 'error', 3000); }
  };

  // ═══════════ LEADER ═══════════
  const onLeaderInserted = async (e: { data: LeaderDto }) => {
    try {
      await adminCreateLeader({ name: e.data.name, kasubsieId: e.data.kasubsieId });
      notify('Leader created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create Leader', 'error', 3000); }
  };
  const onLeaderUpdated = async (e: { data: LeaderDto }) => {
    try {
      await adminUpdateLeader(e.data.id, { name: e.data.name, kasubsieId: e.data.kasubsieId });
      notify('Leader updated', 'success', 1500);
    } catch { notify('Failed to update Leader', 'error', 3000); }
  };
  const onLeaderRemoved = async (e: { data: LeaderDto }) => {
    try {
      await adminDeleteLeader(e.data.id);
      notify('Leader deleted', 'success', 1500);
    } catch { notify('Failed to delete Leader', 'error', 3000); }
  };

  // ═══════════ OPERATOR ═══════════
  const onOperatorInserted = async (e: { data: OperatorDto }) => {
    try {
      await adminCreateOperator({ name: e.data.name, leaderId: e.data.leaderId });
      notify('Operator created', 'success', 1500);
      loadData();
    } catch { notify('Failed to create Operator', 'error', 3000); }
  };
  const onOperatorUpdated = async (e: { data: OperatorDto }) => {
    try {
      await adminUpdateOperator(e.data.id, { name: e.data.name, leaderId: e.data.leaderId });
      notify('Operator updated', 'success', 1500);
    } catch { notify('Failed to update Operator', 'error', 3000); }
  };
  const onOperatorRemoved = async (e: { data: OperatorDto }) => {
    try {
      await adminDeleteOperator(e.data.id);
      notify('Operator deleted', 'success', 1500);
    } catch { notify('Failed to delete Operator', 'error', 3000); }
  };

  return (
    <div>
      <h2 style={{ marginBottom: 16 }}>Personnel Management</h2>
      <LoadPanel visible={loading} />

      <TabPanel>
        {/* ═══════════ KASIE TAB ═══════════ */}
        <TabItem title={`Kasie (${kasies.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={kasies}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onKasieInserted}
              onRowUpdated={onKasieUpdated}
              onRowRemoved={onKasieRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={20} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="name" caption="Nama Kasie" />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ KASUBSIE TAB ═══════════ */}
        <TabItem title={`Kasubsie (${kasubsies.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={kasubsies}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onKasubsieInserted}
              onRowUpdated={onKasubsieUpdated}
              onRowRemoved={onKasubsieRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={20} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="name" caption="Nama Kasubsie" />
              <Column dataField="kasieId" caption="Kasie">
                <Lookup dataSource={kasies} valueExpr="id" displayExpr="name" />
              </Column>
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ LEADER TAB ═══════════ */}
        <TabItem title={`Leader (${leaders.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={leaders}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onLeaderInserted}
              onRowUpdated={onLeaderUpdated}
              onRowRemoved={onLeaderRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={20} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="name" caption="Nama Leader" />
              <Column dataField="kasubsieId" caption="Kasubsie">
                <Lookup dataSource={kasubsies} valueExpr="id" displayExpr="name" />
              </Column>
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ OPERATOR TAB ═══════════ */}
        <TabItem title={`Operator (${operators.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid
              dataSource={operators}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onOperatorInserted}
              onRowUpdated={onOperatorUpdated}
              onRowRemoved={onOperatorRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={20} />
              <Column dataField="id" caption="ID" width={60} allowEditing={false} />
              <Column dataField="name" caption="Nama Operator" />
              <Column dataField="leaderId" caption="Leader">
                <Lookup dataSource={leaders} valueExpr="id" displayExpr="name" />
              </Column>
            </DataGrid>
          </div>
        </TabItem>
      </TabPanel>
    </div>
  );
}
