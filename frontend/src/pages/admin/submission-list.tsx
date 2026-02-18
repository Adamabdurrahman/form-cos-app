import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import DataGrid, { Column, FilterRow, Sorting, Paging } from 'devextreme-react/data-grid';
import SelectBox from 'devextreme-react/select-box';
import Button from 'devextreme-react/button';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  getFormSubmissions,
  getFormDefinitions,
  deleteFormSubmission,
  type FormSubmissionListDto,
  type FormDefinitionListDto,
} from '../../api/cos-api';

export function SubmissionList() {
  const navigate = useNavigate();
  const [submissions, setSubmissions] = useState<FormSubmissionListDto[]>([]);
  const [forms, setForms] = useState<FormDefinitionListDto[]>([]);
  const [selectedFormId, setSelectedFormId] = useState<number | undefined>(undefined);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getFormDefinitions().then(setForms).catch(console.error);
  }, []);

  useEffect(() => {
    setLoading(true);
    getFormSubmissions(selectedFormId)
      .then(setSubmissions)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [selectedFormId]);

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this submission?')) return;
    try {
      await deleteFormSubmission(id);
      setSubmissions((prev) => prev.filter((s) => s.id !== id));
      notify('Submission deleted', 'success', 2000);
    } catch (e) {
      console.error(e);
      notify('Failed to delete submission', 'error', 3000);
    }
  };

  return (
    <div>
      <LoadPanel visible={loading} />
      <h2 style={{ marginTop: 0 }}>Form Submissions</h2>

      <div style={{ marginBottom: 16, display: 'flex', alignItems: 'center', gap: 12 }}>
        <span style={{ fontWeight: 600 }}>Filter by Form:</span>
        <SelectBox
          dataSource={forms}
          displayExpr="title"
          valueExpr="id"
          value={selectedFormId}
          onValueChanged={(e) => setSelectedFormId(e.value)}
          showClearButton
          placeholder="All Forms"
          width={300}
        />
      </div>

      <DataGrid
        dataSource={submissions}
        keyExpr="id"
        showBorders
        columnAutoWidth
        hoverStateEnabled
        onRowClick={(e) => {
          if (e.rowType === 'data') navigate(`/admin/submissions/${e.data.id}`);
        }}
        style={{ cursor: 'pointer' }}
      >
        <FilterRow visible />
        <Sorting mode="multiple" />
        <Paging defaultPageSize={20} />
        <Column dataField="id" caption="ID" width={70} />
        <Column dataField="formTitle" caption="Form" />
        <Column dataField="tanggal" caption="Tanggal" dataType="date" width={120} />
        <Column dataField="lineId" caption="Line" width={70} />
        <Column dataField="shiftId" caption="Shift" width={70} />
        <Column dataField="operatorName" caption="Operator" />
        <Column
          dataField="status"
          caption="Status"
          width={150}
          cellRender={(cellData) => {
            const s = cellData.data.status as string | undefined;
            const hasNgFlag = cellData.data.hasNg;
            let label = 'Pending Leader';
            let bg = '#fff3cd';
            let color = '#856404';
            if (s === 'pending_kasubsie') { label = 'Pending Kasubsie'; bg = '#cce5ff'; color = '#004085'; }
            else if (s === 'pending_kasie') { label = 'Pending Kasie'; bg = '#e2d5f1'; color = '#5a2d82'; }
            else if (s === 'completed') { label = 'Completed'; bg = '#d4edda'; color = '#155724'; }
            return (
              <span style={{
                display: 'inline-flex', alignItems: 'center', gap: 4,
                padding: '2px 10px', borderRadius: 12, fontSize: 11, fontWeight: 600,
                background: bg, color: color, whiteSpace: 'nowrap',
              }}>
                {label}
                {hasNgFlag && <span title="Has NG" style={{ color: '#dc3545', fontWeight: 700 }}>⚠</span>}
              </span>
            );
          }}
        />
        <Column dataField="createdAt" caption="Created At" dataType="datetime" width={180} />
        <Column
          caption="Actions"
          width={160}
          cellRender={(cellData) => (
            <div style={{ display: 'flex', gap: 4 }}>
              <Button
                icon="doc"
                stylingMode="text"
                hint="View Form"
                onClick={(e) => {
                  e.event?.stopPropagation();
                  navigate(`/admin/submissions/${cellData.data.id}/view`);
                }}
              />
              <Button
                icon="trash"
                type="danger"
                stylingMode="text"
                hint="Delete"
                onClick={(e) => {
                  e.event?.stopPropagation();
                  handleDelete(cellData.data.id);
                }}
              />
            </div>
          )}
        />
      </DataGrid>
    </div>
  );
}
