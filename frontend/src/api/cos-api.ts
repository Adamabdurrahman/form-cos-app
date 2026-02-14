// Jika diserve dari backend .NET yang sama, cukup pakai relative path '/api'
// Untuk development terpisah (vite dev), ganti ke URL backend langsung
const API_BASE = '/api';

// ── Generic fetch helper ──
async function apiFetch<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE}${url}`, {
    headers: { 'Content-Type': 'application/json' },
    ...options,
  });
  if (!response.ok) {
    const errorBody = await response.text();
    throw new Error(`API Error ${response.status}: ${errorBody}`);
  }
  return response.json() as Promise<T>;
}

// ══════════════════════════════════════════════════
// TYPES
// ══════════════════════════════════════════════════

// ── Personnel (master data, emp_id-based) ──
export interface OperatorDto {
  empId: string;
  name: string;
  empNo: string;
  lgpId: number;
  groupId: number;
}
export interface LeaderDto { empId: string; name: string; empNo?: string; }
export interface KasubsieDto { empId: string; name: string; empNo?: string; }
export interface KasieDto { empId: string; name: string; empNo?: string; }

export interface HierarchyDto {
  operatorEmpId: string;
  operatorName: string;
  leaderEmpId: string | null;
  leaderName: string | null;
  kasubsieEmpId: string | null;
  kasubsieName: string | null;
  kasieEmpId: string | null;
  kasieName: string | null;
}

// ── Battery / Line / Shift / Mold ──
export interface BatteryStandardDto {
  id: number;
  paramKey: string;
  value: string;
  minValue?: number | null;
  maxValue?: number | null;
  batteryTypeId: number;
}

export interface BatteryTypeDto {
  id: number;
  name: string;
  standards: BatteryStandardDto[];
}

export interface LineDto { id: number; name: string; }
export interface ShiftDto { id: number; name: string; code: string; start: string; end: string; }
export interface MoldDto { code: string; description: string; }

// ── Check Items ──
export interface SubRowDto {
  id: number;
  suffix: string;
  label: string;
  fixedStandard?: string | null;
  fixedMin?: number | null;
  fixedMax?: number | null;
  sortOrder: number;
  checkItemId: number;
}

export interface CheckItemDto {
  id: number;
  formId: number;
  itemKey: string;
  label: string;
  type: 'visual' | 'numeric';
  visualStandard?: string | null;
  numericStdKey?: string | null;
  fixedStandard?: string | null;
  fixedMin?: number | null;
  fixedMax?: number | null;
  frequency?: string | null;
  keterangan?: string | null;
  conditionalLabel?: string | null;
  sortOrder: number;
  subRows?: SubRowDto[];
}

// ── Form Definition ──
export interface FormProblemColumnDto {
  id: number;
  formId: number;
  columnKey: string;
  label: string;
  fieldType: string;
  width: string;
  sortOrder: number;
}

export interface FormSignatureSlotDto {
  id: number;
  formId: number;
  roleKey: string;
  label: string;
  sortOrder: number;
}

export interface FormDefinitionDto {
  id: number;
  code: string;
  title: string;
  subtitle: string;
  slotCount: number;
  isActive: boolean;
  createdAt: string;
  checkItems: CheckItemDto[];
  problemColumns: FormProblemColumnDto[];
  signatureSlots: FormSignatureSlotDto[];
}

export interface FormDefinitionListDto {
  id: number;
  code: string;
  title: string;
  subtitle: string;
  slotCount: number;
  isActive: boolean;
  createdAt: string;
}

// ── Form Submission ──
export interface FormSubmissionPayload {
  formId: number;
  tanggal: string;
  lineId: number | null;
  shiftId: number | null;
  operatorEmpId: string | null;
  leaderEmpId?: string | null;
  kasubsieEmpId?: string | null;
  kasieEmpId?: string | null;
  batterySlotsJson?: string | null;
  checkValues: { settingKey: string; value: string | null }[];
  problems: { sortOrder: number; valuesJson: string }[];
  signatures: { roleKey: string; signatureData: string | null }[];
}

export interface FormSubmissionListDto {
  id: number;
  formId: number;
  tanggal: string;
  lineId: number | null;
  shiftId: number | null;
  operatorEmpId: string | null;
  operatorName?: string;
  formCode?: string;
  formTitle?: string;
  createdAt: string;
}

export interface FormSubmissionDetailDto {
  id: number;
  formId: number;
  tanggal: string;
  lineId: number | null;
  shiftId: number | null;
  operatorEmpId: string | null;
  leaderEmpId?: string | null;
  kasubsieEmpId?: string | null;
  kasieEmpId?: string | null;
  operatorName?: string;
  leaderName?: string;
  kasubsieName?: string;
  kasieName?: string;
  batterySlotsJson?: string;
  createdAt: string;
  form?: { id: number; code: string; title: string; subtitle: string };
  checkValues: { id: number; settingKey: string; value: string }[];
  problems: { id: number; sortOrder: number; valuesJson: string }[];
  signatures: { id: number; roleKey: string; signatureData: string }[];
}

// ── Admin types ──
export interface AdminBatteryTypeDto {
  id: number;
  name: string;
  standards: BatteryStandardDto[];
}

// ══════════════════════════════════════════════════
// API FUNCTIONS
// ══════════════════════════════════════════════════

// ── Personnel (read-only, from master data) ──
export const getOperators = () => apiFetch<OperatorDto[]>('/personnel/operators');
export const getLeaders = () => apiFetch<LeaderDto[]>('/personnel/leaders');
export const getKasubsies = () => apiFetch<KasubsieDto[]>('/personnel/kasubsies');
export const getKasies = () => apiFetch<KasieDto[]>('/personnel/kasies');
export const getHierarchy = (empId: string) =>
  apiFetch<HierarchyDto>(`/personnel/hierarchy/${encodeURIComponent(empId)}`);

// ── Battery / Line / Shift / Mold ──
export const getBatteryTypes = () => apiFetch<BatteryTypeDto[]>('/battery/types');
export const getLines = () => apiFetch<LineDto[]>('/battery/lines');
export const getShifts = () => apiFetch<ShiftDto[]>('/battery/shifts');
export const getMolds = () => apiFetch<MoldDto[]>('/battery/molds');

// ── Check Items ──
export const getCheckItems = () => apiFetch<CheckItemDto[]>('/checkitem');

// ── Form Definitions ──
export const getFormDefinitions = () => apiFetch<FormDefinitionListDto[]>('/formdefinition');
export const getFormDefinitionById = (id: number) => apiFetch<FormDefinitionDto>(`/formdefinition/${id}`);
export const getFormDefinitionByCode = (code: string) => apiFetch<FormDefinitionDto>(`/formdefinition/by-code/${code}`);
export const createFormDefinition = (data: Partial<FormDefinitionDto>) =>
  apiFetch<FormDefinitionDto>('/formdefinition', { method: 'POST', body: JSON.stringify(data) });
export const updateFormDefinition = (id: number, data: Partial<FormDefinitionDto>) =>
  apiFetch<FormDefinitionDto>(`/formdefinition/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const deleteFormDefinition = (id: number) =>
  apiFetch<void>(`/formdefinition/${id}`, { method: 'DELETE' });

// ── Form Submissions ──
export const getFormSubmissions = (formId?: number) =>
  apiFetch<FormSubmissionListDto[]>(`/formsubmission${formId ? `?formId=${formId}` : ''}`);
export const getFormSubmissionById = (id: number) =>
  apiFetch<FormSubmissionDetailDto>(`/formsubmission/${id}`);
export const submitFormSubmission = (data: FormSubmissionPayload) =>
  apiFetch<{ id: number }>('/formsubmission', { method: 'POST', body: JSON.stringify(data) });
export const updateFormSubmission = (id: number, data: FormSubmissionPayload) =>
  apiFetch<{ id: number }>(`/formsubmission/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const deleteFormSubmission = (id: number) =>
  apiFetch<void>(`/formsubmission/${id}`, { method: 'DELETE' });

// ── Admin: Check Items ──
export const adminGetCheckItems = (formId: number) =>
  apiFetch<CheckItemDto[]>(`/admin/checkitems?formId=${formId}`);
export const adminCreateCheckItem = (data: Partial<CheckItemDto>) =>
  apiFetch<CheckItemDto>('/admin/checkitems', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateCheckItem = (id: number, data: Partial<CheckItemDto>) =>
  apiFetch<CheckItemDto>(`/admin/checkitems/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteCheckItem = (id: number) =>
  apiFetch<void>(`/admin/checkitems/${id}`, { method: 'DELETE' });

// ── Admin: Battery Types / Standards ──
export const adminGetBatteryTypes = () =>
  apiFetch<AdminBatteryTypeDto[]>('/admin/batterytypes');
export const adminCreateBatteryType = (data: { name: string }) =>
  apiFetch<{ id: number; name: string }>('/admin/batterytypes', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateBatteryType = (id: number, data: { name: string }) =>
  apiFetch<{ id: number; name: string }>(`/admin/batterytypes/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteBatteryType = (id: number) =>
  apiFetch<void>(`/admin/batterytypes/${id}`, { method: 'DELETE' });

export const adminCreateBatteryStandard = (data: Partial<BatteryStandardDto>) =>
  apiFetch<BatteryStandardDto>('/admin/batterystandards', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateBatteryStandard = (id: number, data: Partial<BatteryStandardDto>) =>
  apiFetch<BatteryStandardDto>(`/admin/batterystandards/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteBatteryStandard = (id: number) =>
  apiFetch<void>(`/admin/batterystandards/${id}`, { method: 'DELETE' });

// ── Admin: Problem Columns ──
export const adminGetProblemColumns = (formId: number) =>
  apiFetch<FormProblemColumnDto[]>(`/admin/problemcolumns?formId=${formId}`);
export const adminCreateProblemColumn = (data: Partial<FormProblemColumnDto>) =>
  apiFetch<FormProblemColumnDto>('/admin/problemcolumns', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateProblemColumn = (id: number, data: Partial<FormProblemColumnDto>) =>
  apiFetch<FormProblemColumnDto>(`/admin/problemcolumns/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteProblemColumn = (id: number) =>
  apiFetch<void>(`/admin/problemcolumns/${id}`, { method: 'DELETE' });

// ── Admin: Signature Slots ──
export const adminGetSignatureSlots = (formId: number) =>
  apiFetch<FormSignatureSlotDto[]>(`/admin/signatureslots?formId=${formId}`);
export const adminCreateSignatureSlot = (data: Partial<FormSignatureSlotDto>) =>
  apiFetch<FormSignatureSlotDto>('/admin/signatureslots', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateSignatureSlot = (id: number, data: Partial<FormSignatureSlotDto>) =>
  apiFetch<FormSignatureSlotDto>(`/admin/signatureslots/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteSignatureSlot = (id: number) =>
  apiFetch<void>(`/admin/signatureslots/${id}`, { method: 'DELETE' });

// ── Admin: Personnel (read-only from master data) ──
export const adminGetPersonnel = () =>
  apiFetch<{
    operators: OperatorDto[];
    leaders: LeaderDto[];
    kasubsies: KasubsieDto[];
    kasies: KasieDto[];
  }>('/admin/personnel');

// ── Admin: Molds (read-only from master data) ──
export const adminGetMolds = () =>
  apiFetch<{ moldCode: string; moldDescription: string; moldStatus: string; idSection: number | null }[]>('/admin/molds');

// ── Employee Signatures (persistent per-employee) ──
export interface EmployeeSignatureDto {
  empId: string;
  signatureData: string | null;
}
export const getEmployeeSignature = (empId: string) =>
  apiFetch<EmployeeSignatureDto>(`/employeesignature/${encodeURIComponent(empId)}`);
export const getEmployeeSignaturesBatch = (empIds: string[]) => {
  const params = empIds.map(id => `empIds=${encodeURIComponent(id)}`).join('&');
  return apiFetch<EmployeeSignatureDto[]>(`/employeesignature/batch?${params}`);
};
export const saveEmployeeSignature = (empId: string, signatureData: string | null) =>
  apiFetch<{ empId: string; saved: boolean }>(`/employeesignature/${encodeURIComponent(empId)}`, {
    method: 'PUT',
    body: JSON.stringify({ signatureData }),
  });
