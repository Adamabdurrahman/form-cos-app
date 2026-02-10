import { useEffect, useState, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import TabPanel, { Item as TabItem } from 'devextreme-react/tab-panel';
import DataGrid, { Column, Editing, Paging } from 'devextreme-react/data-grid';
import { TextBox } from 'devextreme-react/text-box';
import { NumberBox } from 'devextreme-react/number-box';
import Button from 'devextreme-react/button';
import { CheckBox } from 'devextreme-react/check-box';
import LoadPanel from 'devextreme-react/load-panel';
import notify from 'devextreme/ui/notify';
import {
  getFormDefinitionById,
  updateFormDefinition,
  adminGetCheckItems,
  adminUpdateCheckItem,
  adminCreateCheckItem,
  adminDeleteCheckItem,
  adminGetBatteryTypes,
  adminUpdateBatteryStandard,
  adminGetProblemColumns,
  adminCreateProblemColumn,
  adminUpdateProblemColumn,
  adminDeleteProblemColumn,
  adminGetSignatureSlots,
  adminCreateSignatureSlot,
  adminUpdateSignatureSlot,
  adminDeleteSignatureSlot,
  type FormDefinitionDto,
  type CheckItemDto,
  type BatteryStandardDto,
  type FormProblemColumnDto,
  type FormSignatureSlotDto,
} from '../../api/cos-api';
import './form-editor.scss';

interface BatteryTypeWithStandards {
  id: number;
  name: string;
  molds: { id: number; name: string }[];
  standards: BatteryStandardDto[];
}

interface FlatStandard extends BatteryStandardDto {
  batteryTypeName: string;
}

export function FormEditor() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const formId = Number(id);

  const [form, setForm] = useState<FormDefinitionDto | null>(null);
  const [checkItems, setCheckItems] = useState<CheckItemDto[]>([]);
  const [flatStandards, setFlatStandards] = useState<FlatStandard[]>([]);
  const [problemColumns, setProblemColumns] = useState<FormProblemColumnDto[]>([]);
  const [signatureSlots, setSignatureSlots] = useState<FormSignatureSlotDto[]>([]);
  const [loading, setLoading] = useState(true);

  // Editable header fields
  const [title, setTitle] = useState('');
  const [code, setCode] = useState('');
  const [subtitle, setSubtitle] = useState('');
  const [slotCount, setSlotCount] = useState(0);
  const [isActive, setIsActive] = useState(true);

  const loadData = useCallback(async () => {
    try {
      setLoading(true);
      const [formData, items, batteryTypes, probCols, sigSlots] = await Promise.all([
        getFormDefinitionById(formId),
        adminGetCheckItems(formId),
        adminGetBatteryTypes(),
        adminGetProblemColumns(formId),
        adminGetSignatureSlots(formId),
      ]);
      setForm(formData);
      setTitle(formData.title);
      setCode(formData.code);
      setSubtitle(formData.subtitle);
      setSlotCount(formData.slotCount);
      setIsActive(formData.isActive);
      setCheckItems(items);
      setProblemColumns(probCols);
      setSignatureSlots(sigSlots);

      // Flatten battery standards for grid
      const flat: FlatStandard[] = [];
      batteryTypes.forEach((bt: BatteryTypeWithStandards) => {
        bt.standards.forEach((std) => {
          flat.push({ ...std, batteryTypeName: bt.name });
        });
      });
      setFlatStandards(flat);
    } catch (e) {
      console.error('Load error:', e);
      notify('Failed to load form data', 'error', 3000);
    } finally {
      setLoading(false);
    }
  }, [formId]);

  useEffect(() => {
    loadData();
  }, [loadData]);

  const handleSave = async () => {
    try {
      await updateFormDefinition(formId, { title, code, subtitle, slotCount, isActive });
      notify('Form saved successfully', 'success', 2000);
    } catch (e) {
      console.error('Save error:', e);
      notify('Failed to save form', 'error', 3000);
    }
  };

  const onCheckItemRowUpdated = async (e: { data: CheckItemDto }) => {
    try {
      await adminUpdateCheckItem(e.data.id, e.data);
      notify('Check item updated', 'success', 1500);
    } catch (err) {
      console.error(err);
      notify('Failed to update check item', 'error', 3000);
    }
  };

  const onCheckItemRowInserted = async (e: { data: CheckItemDto }) => {
    try {
      await adminCreateCheckItem({ ...e.data, formId });
      await loadData();
      notify('Check item created', 'success', 1500);
    } catch (err) {
      console.error(err);
      notify('Failed to create check item', 'error', 3000);
    }
  };

  const onCheckItemRowRemoved = async (e: { data: CheckItemDto }) => {
    try {
      await adminDeleteCheckItem(e.data.id);
      notify('Check item deleted', 'success', 1500);
    } catch (err) {
      console.error(err);
      notify('Failed to delete check item', 'error', 3000);
    }
  };

  const onBatteryStandardUpdated = async (e: { data: FlatStandard }) => {
    try {
      await adminUpdateBatteryStandard(e.data.id, {
        value: e.data.value,
        minValue: e.data.minValue,
        maxValue: e.data.maxValue,
      });
      notify('Battery standard updated', 'success', 1500);
    } catch (err) {
      console.error(err);
      notify('Failed to update battery standard', 'error', 3000);
    }
  };

  // ═══════════ PROBLEM COLUMNS ═══════════
  const onProbColInserted = async (e: { data: FormProblemColumnDto }) => {
    try {
      await adminCreateProblemColumn({ ...e.data, formId });
      notify('Problem column created', 'success', 1500);
      loadData();
    } catch (err) { console.error(err); notify('Failed to create problem column', 'error', 3000); }
  };
  const onProbColUpdated = async (e: { data: FormProblemColumnDto }) => {
    try {
      await adminUpdateProblemColumn(e.data.id, e.data);
      notify('Problem column updated', 'success', 1500);
    } catch (err) { console.error(err); notify('Failed to update problem column', 'error', 3000); }
  };
  const onProbColRemoved = async (e: { data: FormProblemColumnDto }) => {
    try {
      await adminDeleteProblemColumn(e.data.id);
      notify('Problem column deleted', 'success', 1500);
    } catch (err) { console.error(err); notify('Failed to delete problem column', 'error', 3000); }
  };

  // ═══════════ SIGNATURE SLOTS ═══════════
  const onSigSlotInserted = async (e: { data: FormSignatureSlotDto }) => {
    try {
      await adminCreateSignatureSlot({ ...e.data, formId });
      notify('Signature slot created', 'success', 1500);
      loadData();
    } catch (err) { console.error(err); notify('Failed to create signature slot', 'error', 3000); }
  };
  const onSigSlotUpdated = async (e: { data: FormSignatureSlotDto }) => {
    try {
      await adminUpdateSignatureSlot(e.data.id, e.data);
      notify('Signature slot updated', 'success', 1500);
    } catch (err) { console.error(err); notify('Failed to update signature slot', 'error', 3000); }
  };
  const onSigSlotRemoved = async (e: { data: FormSignatureSlotDto }) => {
    try {
      await adminDeleteSignatureSlot(e.data.id);
      notify('Signature slot deleted', 'success', 1500);
    } catch (err) { console.error(err); notify('Failed to delete signature slot', 'error', 3000); }
  };

  if (!form && !loading) {
    return (
      <div>
        <h2>Form not found</h2>
        <Button text="Back to Forms" onClick={() => navigate('/admin/forms')} />
      </div>
    );
  }

  return (
    <div className="form-editor">
      <LoadPanel visible={loading} />

      <div className="form-editor-header">
        <div className="field-group">
          <label>Code</label>
          <TextBox value={code} onValueChanged={(e) => setCode(e.value ?? '')} width={120} />
        </div>
        <div className="field-group">
          <label>Title</label>
          <TextBox value={title} onValueChanged={(e) => setTitle(e.value ?? '')} width={300} />
        </div>
        <div className="field-group">
          <label>Subtitle</label>
          <TextBox value={subtitle} onValueChanged={(e) => setSubtitle(e.value ?? '')} width={300} />
        </div>
        <div className="field-group">
          <label>Slot Count</label>
          <NumberBox value={slotCount} onValueChanged={(e) => setSlotCount(e.value ?? 0)} width={80} />
        </div>
        <div className="field-group">
          <label>Active</label>
          <CheckBox value={isActive} onValueChanged={(e) => setIsActive(e.value ?? true)} />
        </div>
        <div className="header-actions">
          <Button text="Save" icon="save" type="success" stylingMode="contained" onClick={handleSave} />
          <Button text="Back" icon="back" stylingMode="outlined" onClick={() => navigate('/admin/forms')} />
        </div>
      </div>

      <TabPanel>
        <TabItem title="Check Items">
          <div className="tab-content">
            <DataGrid
              dataSource={checkItems}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowUpdated={onCheckItemRowUpdated}
              onRowInserted={onCheckItemRowInserted}
              onRowRemoved={onCheckItemRowRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={50} />
              <Column dataField="itemKey" caption="Item Key" />
              <Column dataField="label" caption="Label" />
              <Column dataField="type" caption="Type" width={100} />
              <Column dataField="visualStandard" caption="Visual Std" />
              <Column dataField="numericStdKey" caption="Numeric Std Key" />
              <Column dataField="fixedStandard" caption="Fixed Standard" />
              <Column dataField="fixedMin" caption="Fixed Min" dataType="number" width={90} />
              <Column dataField="fixedMax" caption="Fixed Max" dataType="number" width={90} />
              <Column dataField="frequency" caption="Frequency" width={120} />
              <Column dataField="keterangan" caption="Keterangan" />
              <Column dataField="conditionalLabel" caption="Conditional Label" visible={false} />
              <Column dataField="sortOrder" caption="Sort Order" dataType="number" width={100} />
            </DataGrid>
          </div>
        </TabItem>

        <TabItem title="Battery Standards">
          <div className="tab-content">
            <DataGrid
              dataSource={flatStandards}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowUpdated={onBatteryStandardUpdated}
            >
              <Editing mode="cell" allowUpdating />
              <Paging defaultPageSize={50} />
              <Column dataField="batteryTypeName" caption="Battery Type" groupIndex={0} />
              <Column dataField="paramKey" caption="Parameter" allowEditing={false} />
              <Column dataField="value" caption="Value" />
              <Column dataField="minValue" caption="Min Value" dataType="number" />
              <Column dataField="maxValue" caption="Max Value" dataType="number" />
            </DataGrid>
          </div>
        </TabItem>

        <TabItem title="Problem Columns">
          <div className="tab-content">
            <DataGrid
              dataSource={problemColumns}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onProbColInserted}
              onRowUpdated={onProbColUpdated}
              onRowRemoved={onProbColRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={50} />
              <Column dataField="columnKey" caption="Column Key" />
              <Column dataField="label" caption="Label" />
              <Column dataField="fieldType" caption="Field Type" width={120} />
              <Column dataField="width" caption="Width" width={100} />
              <Column dataField="sortOrder" caption="Sort Order" dataType="number" width={100} />
            </DataGrid>
          </div>
        </TabItem>

        <TabItem title="Signature Slots">
          <div className="tab-content">
            <DataGrid
              dataSource={signatureSlots}
              keyExpr="id"
              showBorders
              columnAutoWidth
              onRowInserted={onSigSlotInserted}
              onRowUpdated={onSigSlotUpdated}
              onRowRemoved={onSigSlotRemoved}
            >
              <Editing mode="row" allowUpdating allowAdding allowDeleting />
              <Paging defaultPageSize={50} />
              <Column dataField="roleKey" caption="Role Key" />
              <Column dataField="label" caption="Label" />
              <Column dataField="sortOrder" caption="Sort Order" dataType="number" width={100} />
            </DataGrid>
          </div>
        </TabItem>
      </TabPanel>
    </div>
  );
}
