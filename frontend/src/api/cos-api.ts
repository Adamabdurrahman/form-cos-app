const API_BASE = 'http://localhost:5131/api';

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

export interface OperatorDto { id: number; name: string; leaderId: number; }
export interface LeaderDto { id: number; name: string; kasubsieId: number; }
export interface KasubsieDto { id: number; name: string; kasieId: number; }
export interface KasieDto { id: number; name: string; }

export interface HierarchyDto {
  operator: { id: number; name: string };
  leader: { id: number; name: string } | null;
  kasubsie: { id: number; name: string } | null;
  kasie: { id: number; name: string } | null;
}

export interface BatteryTypeDto {
  name: string;
  molds: string[];
  standards: Record<string, string>;
}

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

export interface FormSubmissionPayload {
  formId: number;
  tanggal: string;
  line: number;
  shift: number;
  operatorId: number;
  leaderId?: number | null;
  kasubsieId?: number | null;
  kasieId?: number | null;
  batterySlotsJson?: string | null;
  checkValues: { settingKey: string; value: string | null }[];
  problems: { sortOrder: number; valuesJson: string }[];
  signatures: { roleKey: string; signatureData: string | null }[];
}

export interface FormSubmissionListDto {
  id: number;
  formId: number;
  tanggal: string;
  line: number;
  shift: number;
  operatorId: number;
  operatorName?: string;
  formCode?: string;
  formTitle?: string;
  createdAt: string;
}

export interface FormSubmissionDetailDto {
  id: number;
  formId: number;
  tanggal: string;
  line: number;
  shift: number;
  operatorId: number;
  leaderId?: number;
  kasubsieId?: number;
  kasieId?: number;
  batterySlotsJson?: string;
  operator?: { id: number; name: string };
  leader?: { id: number; name: string };
  kasubsie?: { id: number; name: string };
  kasie?: { id: number; name: string };
  form?: FormDefinitionListDto;
  checkValues: { id: number; settingKey: string; value: string }[];
  problems: { id: number; sortOrder: number; valuesJson: string }[];
  signatures: { id: number; roleKey: string; signatureData: string }[];
}

export interface BatteryStandardDto {
  id: number;
  paramKey: string;
  value: string;
  minValue?: number | null;
  maxValue?: number | null;
  batteryTypeId: number;
}

// ══════════════════════════════════════════════════
// API FUNCTIONS
// ══════════════════════════════════════════════════

// ── Personnel ──
export const getOperators = () => apiFetch<OperatorDto[]>('/personnel/operators');
export const getLeaders = () => apiFetch<LeaderDto[]>('/personnel/leaders');
export const getKasubsies = () => apiFetch<KasubsieDto[]>('/personnel/kasubsies');
export const getKasies = () => apiFetch<KasieDto[]>('/personnel/kasies');
export const getHierarchy = (operatorId: number) =>
  apiFetch<HierarchyDto>(`/personnel/hierarchy/${operatorId}`);

// ── Battery ──
export const getBatteryTypes = () => apiFetch<BatteryTypeDto[]>('/battery/types');
export const getLines = () => apiFetch<number[]>('/battery/lines');
export const getShifts = () => apiFetch<number[]>('/battery/shifts');

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
export const deleteFormSubmission = (id: number) =>
  apiFetch<void>(`/formsubmission/${id}`, { method: 'DELETE' });

// ── Admin APIs ──
export const adminGetCheckItems = (formId: number) =>
  apiFetch<CheckItemDto[]>(`/admin/checkitems?formId=${formId}`);
export const adminCreateCheckItem = (data: Partial<CheckItemDto>) =>
  apiFetch<CheckItemDto>('/admin/checkitems', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateCheckItem = (id: number, data: Partial<CheckItemDto>) =>
  apiFetch<CheckItemDto>(`/admin/checkitems/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteCheckItem = (id: number) =>
  apiFetch<void>(`/admin/checkitems/${id}`, { method: 'DELETE' });

// ── Admin: Battery Types / Standards / Molds ──
export interface AdminBatteryTypeDto {
  id: number;
  name: string;
  molds: { id: number; name: string; batteryTypeId: number }[];
  standards: BatteryStandardDto[];
}
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

export const adminCreateBatteryMold = (data: { name: string; batteryTypeId: number }) =>
  apiFetch<{ id: number; name: string; batteryTypeId: number }>('/admin/batterymolds', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateBatteryMold = (id: number, data: { name: string }) =>
  apiFetch<{ id: number; name: string }>(`/admin/batterymolds/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteBatteryMold = (id: number) =>
  apiFetch<void>(`/admin/batterymolds/${id}`, { method: 'DELETE' });

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

// ── Admin: Personnel CRUD ──
export const adminGetPersonnel = () =>
  apiFetch<{ kasies: KasieDto[]; kasubsies: KasubsieDto[]; leaders: LeaderDto[]; operators: OperatorDto[] }>('/admin/personnel');

export const adminCreateOperator = (data: { name: string; leaderId: number }) =>
  apiFetch<OperatorDto>('/admin/operators', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateOperator = (id: number, data: { name: string; leaderId: number }) =>
  apiFetch<OperatorDto>(`/admin/operators/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteOperator = (id: number) =>
  apiFetch<void>(`/admin/operators/${id}`, { method: 'DELETE' });

export const adminCreateLeader = (data: { name: string; kasubsieId: number }) =>
  apiFetch<LeaderDto>('/admin/leaders', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateLeader = (id: number, data: { name: string; kasubsieId: number }) =>
  apiFetch<LeaderDto>(`/admin/leaders/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteLeader = (id: number) =>
  apiFetch<void>(`/admin/leaders/${id}`, { method: 'DELETE' });

export const adminCreateKasubsie = (data: { name: string; kasieId: number }) =>
  apiFetch<KasubsieDto>('/admin/kasubsies', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateKasubsie = (id: number, data: { name: string; kasieId: number }) =>
  apiFetch<KasubsieDto>(`/admin/kasubsies/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteKasubsie = (id: number) =>
  apiFetch<void>(`/admin/kasubsies/${id}`, { method: 'DELETE' });

export const adminCreateKasie = (data: { name: string }) =>
  apiFetch<KasieDto>('/admin/kasies', { method: 'POST', body: JSON.stringify(data) });
export const adminUpdateKasie = (id: number, data: { name: string }) =>
  apiFetch<KasieDto>(`/admin/kasies/${id}`, { method: 'PUT', body: JSON.stringify(data) });
export const adminDeleteKasie = (id: number) =>
  apiFetch<void>(`/admin/kasies/${id}`, { method: 'DELETE' });
