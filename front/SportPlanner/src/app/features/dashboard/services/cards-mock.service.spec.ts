import { TestBed } from '@angular/core/testing';
import { CardsMockService } from './cards-mock.service';

describe('CardsMockService', () => {
  interface Item { id: number; name: string }
  let service: CardsMockService<Item>;
  const seed = [ { id: 1, name: 'A' }, { id: 2, name: 'B' } ];

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [CardsMockService] });
    service = TestBed.inject(CardsMockService) as CardsMockService<Item>;
    service.init(seed);
  });

  it('should return all items after init', () => {
    const all = service.getAll();
    expect(all.length).toBe(2);
  });

  it('should update an item and push undo', () => {
    const updated = { id: 1, name: 'A-up' };
    const res = service.update(updated);
    expect(res.success).toBeTrue();
    expect(service.getAll().find(i => i.id === 1)?.name).toBe('A-up');
    // undo
    const undo = service.undoLast();
    expect(undo.success).toBeTrue();
    expect(service.getAll().find(i => i.id === 1)?.name).toBe('A');
  });

  it('should remove an item and allow undo', () => {
    const res = service.remove(2);
    expect(res.success).toBeTrue();
    expect(service.getAll().find(i => i.id === 2)).toBeUndefined();
    const undo = service.undoLast();
    expect(undo.success).toBeTrue();
    expect(service.getAll().find(i => i.id === 2)).toBeDefined();
  });
});
