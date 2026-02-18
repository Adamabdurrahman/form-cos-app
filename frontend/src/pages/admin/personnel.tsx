import { useEffect, useState, useCallback } from 'react';
import TabPanel, { Item as TabItem } from 'devextreme-react/tab-panel';
import DataGrid, { Column, Paging, FilterRow } from 'devextreme-react/data-grid';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  adminGetPersonnel,
  type OperatorDto, type LeaderDto, type KasubsieDto, type KasieDto,
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

  return (
    <div>
      <h2 style={{ marginBottom: 8 }}>Personnel (Master Data)</h2>
      <p style={{ color: '#888', fontSize: 13, marginBottom: 16 }}>
        Data personel berasal dari database master data perusahaan (read-only).
      </p>
      <LoadPanel visible={loading} />

      <TabPanel>
        {/* ═══════════ KASIE TAB ═══════════ */}
        <TabItem title={`Kasie (${kasies.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid dataSource={kasies} keyExpr="empId" showBorders columnAutoWidth>
              <FilterRow visible />
              <Paging defaultPageSize={20} />
              <Column dataField="empId" caption="Emp ID" width={140} />
              <Column dataField="name" caption="Nama Kasie" />
              <Column dataField="empNo" caption="Emp No" width={100} />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ KASUBSIE TAB ═══════════ */}
        <TabItem title={`Kasubsie (${kasubsies.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid dataSource={kasubsies} keyExpr="empId" showBorders columnAutoWidth>
              <FilterRow visible />
              <Paging defaultPageSize={20} />
              <Column dataField="empId" caption="Emp ID" width={140} />
              <Column dataField="name" caption="Nama Kasubsie" />
              <Column dataField="empNo" caption="Emp No" width={100} />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ LEADER TAB ═══════════ */}
        <TabItem title={`Leader (${leaders.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid dataSource={leaders} keyExpr="empId" showBorders columnAutoWidth>
              <FilterRow visible />
              <Paging defaultPageSize={20} />
              <Column dataField="empId" caption="Emp ID" width={140} />
              <Column dataField="name" caption="Nama Leader" />
              <Column dataField="empNo" caption="Emp No" width={100} />
            </DataGrid>
          </div>
        </TabItem>

        {/* ═══════════ OPERATOR TAB ═══════════ */}
        <TabItem title={`Operator (${operators.length})`}>
          <div style={{ padding: 16 }}>
            <DataGrid dataSource={operators} keyExpr="empId" showBorders columnAutoWidth>
              <FilterRow visible />
              <Paging defaultPageSize={20} />
              <Column dataField="empId" caption="Emp ID" width={140} />
              <Column dataField="name" caption="Nama Operator" />
              <Column dataField="empNo" caption="Emp No" width={100} />
              <Column dataField="lgpId" caption="Line Group" width={100} />
              <Column dataField="groupId" caption="Group" width={80} />
            </DataGrid>
          </div>
        </TabItem>
      </TabPanel>
    </div>
  );
}
