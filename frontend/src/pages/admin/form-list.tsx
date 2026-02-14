import { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import DataGrid, { Column, FilterRow, Sorting, Paging } from 'devextreme-react/data-grid';
import LoadPanel from 'devextreme-react/load-panel';
import { getFormDefinitions, type FormDefinitionListDto } from '../../api/cos-api';

export function FormList() {
  const navigate = useNavigate();
  const [forms, setForms] = useState<FormDefinitionListDto[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getFormDefinitions()
      .then(setForms)
      .catch((e) => console.error('Error loading forms:', e))
      .finally(() => setLoading(false));
  }, []);

  return (
    <div>
      <LoadPanel visible={loading} />
      <h2 style={{ marginTop: 0 }}>Form Definitions</h2>

      <DataGrid
        dataSource={forms}
        keyExpr="id"
        showBorders
        columnAutoWidth
        hoverStateEnabled
        onRowClick={(e) => navigate(`/admin/forms/${e.data.id}`)}
        style={{ cursor: 'pointer' }}
      >
        <FilterRow visible />
        <Sorting mode="multiple" />
        <Paging defaultPageSize={20} />
        <Column dataField="id" caption="ID" width={70} />
        <Column dataField="code" caption="Code" />
        <Column dataField="title" caption="Title" />
        <Column dataField="subtitle" caption="Subtitle" />
        <Column dataField="slotCount" caption="Slot Count" width={100} />
        <Column dataField="isActive" caption="Active" width={80} dataType="boolean" />
        <Column dataField="createdAt" caption="Created At" dataType="datetime" width={180} />
      </DataGrid>
    </div>
  );
}
