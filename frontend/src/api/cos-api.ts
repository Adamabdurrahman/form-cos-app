const API_BASE = 'http://10.160.54.67:5000/api';

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
// TYPES — mirroring the backend response shapes
// ══════════════════════════════════════════════════

export interface OperatorDto {
  id: number;
  name: string;
  leaderId: number;
}

export interface LeaderDto {
  id: number;
  name: string;
  kasubsieId: number;
}

export interface KasubsieDto {
  id: number;
  name: string;
  kasieId: number;
}

export interface KasieDto {
  id: number;
  name: string;
}

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
  suffix: string;
  label: string;
  fixedStandard?: string;
}

export interface CheckItemDto {
  id: string;
  label: string;
  type: 'visual' | 'numeric';
  visualStandard?: string;
  numericStdKey?: string;
  fixedStandard?: string;
  frequency?: string;
  keterangan?: string;
  conditionalLabel?: string;
  subRows?: SubRowDto[];
}

export interface CosValidationSubmitPayload {
  tanggal: string;
  line: number;
  shift: number;
  operatorId: number;
  leaderId?: number;
  kasubsieId?: number;
  kasieId?: number;
  batteryType1?: string;
  mold1?: string;
  batteryType2?: string;
  mold2?: string;
  batteryType3?: string;
  mold3?: string;
  settings: Record<string, string | null>;
  problems: { problem?: string; action?: string }[];
  signatures: Record<string, string | null>;
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

// ── COS Validation Form ──
export const submitCosValidation = (data: CosValidationSubmitPayload) =>
  apiFetch<{ id: number }>('/cosvalidation', {
    method: 'POST',
    body: JSON.stringify(data),
  });

export const getCosValidation = (id: number) =>
  apiFetch<Record<string, unknown>>(`/cosvalidation/${id}`);

export const deleteCosValidation = (id: number) =>
  apiFetch<void>(`/cosvalidation/${id}`, { method: 'DELETE' });
