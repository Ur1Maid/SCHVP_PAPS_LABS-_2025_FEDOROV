import { CumList, DueItem, HistoryItem } from '../domain/model';

export const cumLists: CumList[] = [
  {
    id: 101,
    documentId: 50101,
    cumNumber: 'НВ-2026-000101',
    state: 'READY_FOR_SIGN',
    payerId: 7701,
    clientId: 7710,
    stationId: 2001,
    payFormId: 1,
    payPlaceId: 2,
    totalAmount: 125000.45,
    createdAt: '2026-03-01T09:00:00Z',
    needForEcp: true,
    locked: false
  },
  {
    id: 102,
    documentId: 50102,
    cumNumber: 'НВ-2026-000102',
    state: 'ON_REVIEW',
    payerId: 7701,
    clientId: 7711,
    stationId: 2002,
    payFormId: 1,
    payPlaceId: 2,
    totalAmount: 84000.00,
    createdAt: '2026-03-03T09:00:00Z',
    needForEcp: true,
    locked: false
  }
];

export const duesByCumListId: Record<number, DueItem[]> = {
  101: [
    { id: 9001, docId: 50101, amount: 12000, taxValue: 2400, note: 'Маневровая работа' }
  ],
  102: [
    { id: 9002, docId: 50102, amount: 3000, taxValue: 600, note: 'Подача вагонов' }
  ]
};

export const historyByCumListId: Record<number, HistoryItem[]> = {
  101: [
    {
      operationId: 7001,
      type: 'sign',
      result: 'SUCCESS',
      performedBy: 15,
      performedAt: '2026-03-02T11:45:00Z'
    }
  ],
  102: []
};

export const rejectionReasons = [
  { id: 1, code: 'DATA_ERROR', name: 'Ошибка в данных документа' },
  { id: 2, code: 'RULE_CONFLICT', name: 'Конфликт по результатам правила' }
];
