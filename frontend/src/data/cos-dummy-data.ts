// ====================== PERSONNEL HIERARCHY ======================

export interface Kasie {
  id: number;
  name: string;
}

export interface Kasubsie {
  id: number;
  name: string;
  kasieId: number;
}

export interface Leader {
  id: number;
  name: string;
  kasubsieId: number;
}

export interface Operator {
  id: number;
  name: string;
  leaderId: number;
}

export const kasieList: Kasie[] = [
  { id: 1, name: 'Kurniawan Adi' },
  { id: 2, name: 'Sutrisno Hadi' },
];

export const kasubsieList: Kasubsie[] = [
  { id: 1, name: 'Joko Prasetyo', kasieId: 1 },
  { id: 2, name: 'Mulyono Slamet', kasieId: 1 },
  { id: 3, name: 'Teguh Wibowo', kasieId: 2 },
];

export const leaderList: Leader[] = [
  { id: 1, name: 'Hendra Wijaya', kasubsieId: 1 },
  { id: 2, name: 'Irfan Hakim', kasubsieId: 1 },
  { id: 3, name: 'Fajar Nugroho', kasubsieId: 2 },
  { id: 4, name: 'Dedi Kurniawan', kasubsieId: 3 },
];

export const operatorList: Operator[] = [
  { id: 1, name: 'Ahmad Rizky', leaderId: 1 },
  { id: 2, name: 'Budi Santoso', leaderId: 1 },
  { id: 3, name: 'Cahya Dewi', leaderId: 2 },
  { id: 4, name: 'Dian Permata', leaderId: 2 },
  { id: 5, name: 'Eko Prasetyo', leaderId: 3 },
  { id: 6, name: 'Faisal Rahman', leaderId: 3 },
  { id: 7, name: 'Gunawan Putra', leaderId: 4 },
  { id: 8, name: 'Haris Munandar', leaderId: 4 },
];

// ====================== PRODUCTION CONFIG ======================

export const lineList: number[] = [2, 3, 4, 5, 6, 7, 8];
export const shiftList: number[] = [1, 2, 3];

export interface BatteryType {
  name: string;
  molds: string[];
  standards: Record<string, string>;
}

export const batteryTypes: BatteryType[] = [
  {
    name: 'NS40ZL',
    molds: ['COS-A01', 'COS-A02', 'COS-A03'],
    standards: {
      pourWait: '-',
      pourTime: '',
      dipTime2: '',
      dumpTime: '',
      lugDryTime: '',
      largeVibratorTime: '',
      smallVibratorTime: '',
      coolingTime: '',
      leadPumpSpeed: '',
      tempAirDryer: '300 - 400',
      tempPot: '470 - 490',
      tempPipe: '410 - 430',
      tempCrossBlock: '390 - 410',
      tempElbow: '370 - 390',
      tempMold: '160 - 190',
      coolingFlowRate: '6 - 10',
    },
  },
  {
    name: 'NS60L',
    molds: ['COS-B01', 'COS-B02'],
    standards: {
      pourWait: '-',
      pourTime: '2.5 - 4.0',
      dipTime2: '2.0 - 3.5',
      dumpTime: '1.5 - 3.0',
      lugDryTime: '3.5 - 5.5',
      largeVibratorTime: '1.5 - 3.5',
      smallVibratorTime: '1.5 - 3.5',
      coolingTime: '25 - 35',
      leadPumpSpeed: '45 - 65',
      tempAirDryer: '310 - 410',
      tempPot: '475 - 495',
      tempPipe: '415 - 435',
      tempCrossBlock: '395 - 415',
      tempElbow: '375 - 395',
      tempMold: '165 - 195',
      coolingFlowRate: '7 - 11',
    },
  },
  {
    name: '34B19LS',
    molds: ['COS-C01', 'COS-C02', 'COS-C03'],
    standards: {
      pourWait: '1.0 - 2.0',
      pourTime: '1.5 - 3.0',
      dipTime2: '1.0 - 2.5',
      dumpTime: '0.8 - 2.0',
      lugDryTime: '2.5 - 4.5',
      largeVibratorTime: '0.8 - 2.5',
      smallVibratorTime: '0.8 - 2.5',
      coolingTime: '18 - 28',
      leadPumpSpeed: '35 - 55',
      tempAirDryer: '290 - 390',
      tempPot: '465 - 485',
      tempPipe: '405 - 425',
      tempCrossBlock: '385 - 405',
      tempElbow: '365 - 385',
      tempMold: '155 - 185',
      coolingFlowRate: '5 - 9',
    },
  },
  {
    name: 'N50Z',
    molds: ['COS-D01', 'COS-D02'],
    standards: {
      pourWait: '-',
      pourTime: '3.0 - 5.0',
      dipTime2: '2.5 - 4.0',
      dumpTime: '2.0 - 3.5',
      lugDryTime: '4.0 - 6.0',
      largeVibratorTime: '2.0 - 4.0',
      smallVibratorTime: '2.0 - 4.0',
      coolingTime: '28 - 38',
      leadPumpSpeed: '50 - 70',
      tempAirDryer: '320 - 420',
      tempPot: '480 - 500',
      tempPipe: '420 - 440',
      tempCrossBlock: '400 - 420',
      tempElbow: '380 - 400',
      tempMold: '170 - 200',
      coolingFlowRate: '8 - 12',
    },
  },
  {
    name: 'N70Z',
    molds: ['COS-E01', 'COS-E02', 'COS-E03'],
    standards: {
      pourWait: '-',
      pourTime: '3.5 - 5.5',
      dipTime2: '3.0 - 4.5',
      dumpTime: '2.5 - 4.0',
      lugDryTime: '4.5 - 6.5',
      largeVibratorTime: '2.5 - 4.5',
      smallVibratorTime: '2.5 - 4.5',
      coolingTime: '30 - 42',
      leadPumpSpeed: '55 - 75',
      tempAirDryer: '330 - 430',
      tempPot: '485 - 505',
      tempPipe: '425 - 445',
      tempCrossBlock: '405 - 425',
      tempElbow: '385 - 405',
      tempMold: '175 - 205',
      coolingFlowRate: '9 - 13',
    },
  },
  {
    name: '34B19LS OE TYT',
    molds: ['COS-F01', 'COS-F02'],
    standards: {
      pourWait: '1.0 - 2.0',
      pourTime: '1.5 - 3.0',
      dipTime2: '1.0 - 2.5',
      dumpTime: '0.8 - 2.0',
      lugDryTime: '2.5 - 4.5',
      largeVibratorTime: '0.8 - 2.5',
      smallVibratorTime: '0.8 - 2.5',
      coolingTime: '18 - 28',
      leadPumpSpeed: '35 - 55',
      tempAirDryer: '290 - 390',
      tempPot: '465 - 485',
      tempPipe: '405 - 425',
      tempCrossBlock: '385 - 405',
      tempElbow: '365 - 385',
      tempMold: '155 - 185',
      coolingFlowRate: '5 - 9',
    },
  },
];

// ====================== HIERARCHY HELPERS ======================

export function getHierarchyByOperator(operatorId: number) {
  const operator = operatorList.find(o => o.id === operatorId);
  if (!operator) return { leader: undefined, kasubsie: undefined, kasie: undefined };

  const leader = leaderList.find(l => l.id === operator.leaderId);
  const kasubsie = leader ? kasubsieList.find(k => k.id === leader.kasubsieId) : undefined;
  const kasie = kasubsie ? kasieList.find(ka => ka.id === kasubsie.kasieId) : undefined;

  return { leader, kasubsie, kasie };
}

// ====================== CHECK ITEM DEFINITIONS ======================

export interface SubRowDef {
  suffix: string;
  label: string;
  fixedStandard?: string;
}

export interface CheckItemDef {
  id: string;
  label: string;
  type: 'visual' | 'numeric';
  visualStandard?: string;
  numericStdKey?: string;
  fixedStandard?: string;
  frequency: string;
  keterangan?: string;
  subRows?: SubRowDef[];
  conditionalLabel?: string;
}

export const checkItems: CheckItemDef[] = [
  {
    id: 'kekuatanCastingStrap',
    label: 'Kekuatan Casting Strap',
    type: 'visual',
    visualStandard: 'Ditarik tidak lepas',
    frequency: '1 batt / shift / ganti type',
    subRows: [
      { suffix: 'plus', label: '+' },
      { suffix: 'minus', label: '−' },
    ],
  },
  {
    id: 'meniscus',
    label: 'Meniscus',
    type: 'visual',
    visualStandard: 'Positif',
    frequency: '1 batt / shift / ganti type',
    subRows: [
      { suffix: 'plus', label: '+' },
      { suffix: 'minus', label: '−' },
    ],
  },
  {
    id: 'hasilCastingStrap',
    label: 'Hasil Casting Strap',
    type: 'visual',
    visualStandard: 'Tidak ada flash',
    frequency: '1 Batt / shift / ganti type',
  },
  {
    id: 'levelFlux',
    label: 'Level Flux',
    type: 'visual',
    visualStandard: 'Terisi Flux',
    frequency: '',
  },
  {
    id: 'pourWait',
    label: 'Pour Wait (Khusus Line 8)',
    type: 'numeric',
    numericStdKey: 'pourWait',
    frequency: '2 x / Shift / ganti type',
    conditionalLabel: 'Khusus Line 8',
  },
  {
    id: 'pourTime',
    label: 'Pour Time',
    type: 'numeric',
    numericStdKey: 'pourTime',
    frequency: '',
  },
  {
    id: 'dipTime2',
    label: 'Dip Time 2',
    type: 'numeric',
    numericStdKey: 'dipTime2',
    frequency: '',
  },
  {
    id: 'dumpTime',
    label: 'Dump Time (Drain back time)',
    type: 'numeric',
    numericStdKey: 'dumpTime',
    frequency: '',
  },
  {
    id: 'lugDryTime',
    label: 'Lug Dry Time',
    type: 'numeric',
    numericStdKey: 'lugDryTime',
    frequency: '2 x / Shift / ganti type',
    keterangan: 'untuk 34B19LS OE TYT',
  },
  {
    id: 'largeVibratorTime',
    label: 'Large Vibrator Time',
    type: 'numeric',
    numericStdKey: 'largeVibratorTime',
    frequency: '',
  },
  {
    id: 'smallVibratorTime',
    label: 'Small Vibrator Time',
    type: 'numeric',
    numericStdKey: 'smallVibratorTime',
    frequency: '',
  },
  {
    id: 'coolingTime',
    label: 'Cooling Time',
    type: 'numeric',
    numericStdKey: 'coolingTime',
    frequency: '',
  },
  {
    id: 'leadPumpSpeed',
    label: 'Lead Pump Speed',
    type: 'numeric',
    numericStdKey: 'leadPumpSpeed',
    frequency: '',
  },
  {
    id: 'checkAlignment',
    label: 'Check Alignment',
    type: 'visual',
    visualStandard: 'Bergerak',
    frequency: '1 x / shift',
  },
  {
    id: 'checkDatumTable',
    label: 'Check Datum Table Alignment',
    type: 'visual',
    visualStandard: 'Bersih',
    frequency: '1 x / shift',
    keterangan: 'Tidak ada ceceran pasta',
  },
  {
    id: 'cleaningNozzle',
    label: 'Cleaning of Nozzle Lug Dry',
    type: 'visual',
    visualStandard: 'Bersih',
    frequency: '1 x / shift',
    keterangan: 'Spray dengan udara',
  },
  {
    id: 'tempAirNozzleLugDry',
    label: 'Temp Air Nozzle Lug Dry',
    type: 'numeric',
    fixedStandard: '> 275° C',
    frequency: '2 x / shift',
    keterangan: 'Cek dgn Thermocouple',
  },
  {
    id: 'tempAirDryer',
    label: 'Temp Air Dryer (hot air)',
    type: 'numeric',
    numericStdKey: 'tempAirDryer',
    frequency: '2 x / shift',
  },
  {
    id: 'blowerPipeTemp',
    label: 'Blower Pipe Temp (Khusus Line 7)',
    type: 'numeric',
    fixedStandard: '> 300° C',
    frequency: '2 x / shift',
    conditionalLabel: 'Khusus Line 7',
  },
  {
    id: 'blowerNozzle1Temp',
    label: 'Blower Nozzle 1 Temp (Khusus Line 7)',
    type: 'numeric',
    fixedStandard: '> 200° C',
    frequency: '2 x / shift',
    conditionalLabel: 'Khusus Line 7',
  },
  {
    id: 'blowerNozzle2Temp',
    label: 'Blower Nozzle 2 Temp (Khusus Line 7)',
    type: 'numeric',
    fixedStandard: '> 200° C',
    frequency: '2 x / shift',
    conditionalLabel: 'Khusus Line 7',
  },
  {
    id: 'tempPot',
    label: 'Temperatur Pot',
    type: 'numeric',
    numericStdKey: 'tempPot',
    frequency: '2 x / shift',
  },
  {
    id: 'tempPipe',
    label: 'Temperatur Pipe',
    type: 'numeric',
    numericStdKey: 'tempPipe',
    frequency: '2 x / shift',
    subRows: [
      { suffix: 'L', label: 'L' },
      { suffix: 'R', label: 'R' },
    ],
  },
  {
    id: 'tempCrossBlock',
    label: 'Temp. Cross Block',
    type: 'numeric',
    numericStdKey: 'tempCrossBlock',
    frequency: '2 x / shift',
  },
  {
    id: 'tempElbow',
    label: 'Temp. Elbow (Lead Lump)',
    type: 'numeric',
    numericStdKey: 'tempElbow',
    frequency: '2 x / shift',
  },
  {
    id: 'tempMold',
    label: 'Temperatur Mold',
    type: 'numeric',
    numericStdKey: 'tempMold',
    frequency: '2 x / shift',
    subRows: [
      { suffix: 'mold1', label: 'Mold 1' },
      { suffix: 'mold2', label: 'Mold 2' },
      { suffix: 'post1', label: 'Post 1' },
      { suffix: 'post2', label: 'Post 2' },
    ],
  },
  {
    id: 'coolingFlowRate',
    label: 'Cooling Water Flow Rate',
    type: 'numeric',
    numericStdKey: 'coolingFlowRate',
    frequency: '2 x / shift',
    subRows: [
      { suffix: 'mold1', label: 'Mold 1' },
      { suffix: 'mold2', label: 'Mold 2' },
    ],
  },
  {
    id: 'coolingWaterTemp',
    label: 'Cooling Water Temperature',
    type: 'numeric',
    fixedStandard: '28 ± 2 °C',
    frequency: '2 x / shift',
  },
  {
    id: 'sprueBrush',
    label: 'Sprue Brush',
    type: 'visual',
    visualStandard: 'Berfungsi dengan baik',
    frequency: '2 x / Shift',
  },
  {
    id: 'cleaningCavityMold',
    label: 'Cleaning Cavity Mold COS',
    type: 'visual',
    visualStandard: 'Tidak tersumbat dross',
    frequency: '3 x / Shift',
  },
  {
    id: 'fluxTime',
    label: 'Flux Time',
    type: 'numeric',
    frequency: '1 batt / shift / ganti type',
    subRows: [
      { suffix: 'line6', label: 'Line 6', fixedStandard: '1 - 3 detik' },
      { suffix: 'lineOther', label: 'Line 2,3,4,5,7&8', fixedStandard: '0.1 - 1 detik' },
    ],
  },
  {
    id: 'overflowHydrazine',
    label: 'Overflow Hydrazine',
    type: 'numeric',
    frequency: '1 batt / shift / ganti type',
    subRows: [
      { suffix: 'line2', label: 'Line 2', fixedStandard: '10 detik' },
      { suffix: 'line7', label: 'Line 7', fixedStandard: '5 detik' },
    ],
  },
];
