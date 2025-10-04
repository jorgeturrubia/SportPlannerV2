import { Injectable } from '@angular/core';

/**
 * Simple in-memory mock persistence with an undo stack.
 * Not asynchronous on purpose: keeps the demo simple and deterministic.
 */
export interface MockAction<T> {
  type: 'update' | 'remove';
  before?: T;
  after?: T;
}

@Injectable({ providedIn: 'root' })
export class CardsMockService<T extends { id: number }> {
  private items: T[] = [];
  private undoStack: MockAction<T>[] = [];

  init(items: T[]) {
    // shallow copy to avoid external mutations
    this.items = items.map(i => ({ ...i }));
    this.undoStack = [];
  }

  getAll(): T[] {
    return this.items.map(i => ({ ...i }));
  }

  update(updated: T): { success: boolean; before?: T; after?: T } {
    const idx = this.items.findIndex(i => i.id === updated.id);
    if (idx === -1) return { success: false };
    const before = { ...this.items[idx] };
    this.items[idx] = { ...updated };
    this.undoStack.push({ type: 'update', before, after: { ...updated } });
    return { success: true, before, after: { ...updated } };
  }

  remove(id: number): { success: boolean; removed?: T } {
    const idx = this.items.findIndex(i => i.id === id);
    if (idx === -1) return { success: false };
    const removed = { ...this.items[idx] };
    this.items.splice(idx, 1);
    this.undoStack.push({ type: 'remove', before: removed });
    return { success: true, removed };
  }

  undoLast(): { success: boolean; action?: MockAction<T> } {
    const action = this.undoStack.pop();
    if (!action) return { success: false };
    if (action.type === 'remove' && action.before) {
      // restore removed
      this.items.push({ ...action.before });
      // keep stable order by id (simple heuristic)
      this.items.sort((a, b) => a.id - b.id);
      return { success: true, action };
    }
    if (action.type === 'update' && action.before) {
      const idx = this.items.findIndex(i => i.id === action.before!.id);
      if (idx !== -1) {
        this.items[idx] = { ...action.before };
        return { success: true, action };
      }
      // if not found, insert it back
      this.items.push({ ...action.before });
      this.items.sort((a, b) => a.id - b.id);
      return { success: true, action };
    }
    return { success: false };
  }

  clearUndo() {
    this.undoStack = [];
  }
}
