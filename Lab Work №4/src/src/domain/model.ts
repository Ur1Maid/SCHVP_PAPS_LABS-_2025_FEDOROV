export type CumListState =
  | 'DRAFT'
  | 'ON_REVIEW'
  | 'READY_FOR_SIGN'
  | 'SIGNED'
  | 'REJECTED';

export interface CumList {
  id: number;
  documentId: number;
  cumNumber: string;
  state: CumListState;
  payerId: number;
  clientId: number;
  stationId: number;
  payFormId: number;
  payPlaceId: number;
  totalAmount: number;
  createdAt: string;
  needForEcp: boolean;
  locked: boolean;
}

export interface DueItem {
  id: number;
  docId: number;
  amount: number;
  taxValue: number;
  note: string;
}

export interface HistoryItem {
  operationId: number;
  type: 'sign' | 'reject' | 'lock' | 'unlock';
  result: 'SUCCESS' | 'ERROR';
  performedBy: number;
  performedAt: string;
}
