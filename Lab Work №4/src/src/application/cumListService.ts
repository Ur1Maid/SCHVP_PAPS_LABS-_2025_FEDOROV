import { cumLists, duesByCumListId, historyByCumListId, rejectionReasons } from '../data/store';
import { HistoryItem } from '../domain/model';

export class CumListService {
  list(filters: Record<string, string | undefined>) {
    const state = filters.state;
    const page = Number(filters.page ?? 1);
    const pageSize = Number(filters.pageSize ?? 20);
    const filtered = state ? cumLists.filter((item) => item.state === state) : cumLists;

    return {
      items: filtered.slice((page - 1) * pageSize, page * pageSize),
      pageInfo: {
        page,
        pageSize,
        total: filtered.length,
      },
    };
  }

  getById(id: number) {
    return cumLists.find((item) => item.id === id) ?? null;
  }

  getDues(id: number) {
    return { items: duesByCumListId[id] ?? [] };
  }

  getHistory(id: number) {
    return { items: historyByCumListId[id] ?? [] };
  }

  getRejectionReasons() {
    return { items: rejectionReasons };
  }

  sign(id: number, userId: number) {
    const item = this.getById(id);
    if (!item) return null;
    item.state = 'SIGNED';
    historyByCumListId[id] = historyByCumListId[id] ?? [];
    historyByCumListId[id].push(this.createHistory('sign', userId));
    return {
      documentId: id,
      operation: 'sign',
      status: 'ACCEPTED',
      message: 'Sign operation has been sent to integration module',
    };
  }

  reject(id: number, userId: number) {
    const item = this.getById(id);
    if (!item) return null;
    item.state = 'REJECTED';
    historyByCumListId[id] = historyByCumListId[id] ?? [];
    historyByCumListId[id].push(this.createHistory('reject', userId));
    return {
      documentId: id,
      operation: 'reject',
      status: 'ACCEPTED',
      message: 'Reject operation has been sent to integration module',
    };
  }

  bulkSign(documentIds: number[]) {
    const accepted: number[] = [];
    const rejected: Array<{ documentId: number; errorCode: string }> = [];

    for (const id of documentIds) {
      const item = this.getById(id);
      if (!item || item.state !== 'READY_FOR_SIGN') {
        rejected.push({ documentId: id, errorCode: 'INVALID_STATE' });
        continue;
      }
      item.state = 'SIGNED';
      accepted.push(id);
    }

    return { accepted, rejected };
  }

  lock(id: number, owner: string) {
    const item = this.getById(id);
    if (!item) return null;
    item.locked = true;
    return { documentId: id, locked: true, owner };
  }

  unlock(id: number) {
    const item = this.getById(id);
    if (!item) return null;
    item.locked = false;
    return { documentId: id, locked: false };
  }

  private createHistory(type: HistoryItem['type'], userId: number): HistoryItem {
    return {
      operationId: Date.now(),
      type,
      result: 'SUCCESS',
      performedBy: userId,
      performedAt: new Date().toISOString(),
    };
  }
}
