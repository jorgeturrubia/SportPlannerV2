import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CardsShowcase } from './cards-showcase';
import { CardsMockService } from '../../services/cards-mock.service';
import { CommonModule } from '@angular/common';

describe('CardsShowcase', () => {
  let fixture: ComponentFixture<CardsShowcase>;
  let component: CardsShowcase;
  let mockService: CardsMockService<any>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CommonModule, CardsShowcase],
      providers: [CardsMockService]
    }).compileComponents();

    fixture = TestBed.createComponent(CardsShowcase as any);
    component = fixture.componentInstance as unknown as CardsShowcase;
    mockService = TestBed.inject(CardsMockService) as CardsMockService<any>;
    mockService.init(component['athletes']());
    fixture.detectChanges();
  });

  it('emits before and after for update', () => {
    const spy = jasmine.createSpy('cardEvent');
    component.cardEvent.subscribe(spy as any);

    const first = component['athletes']()[0];
    component.startEdit(first);
    component.editCopy.set({ name: 'Edited' });
    component.saveEdit();

    // expect at least two events (before & after)
    expect(spy.calls.allArgs().some(a => a[0].phase === 'before' && a[0].action === 'update')).toBeTrue();
    expect(spy.calls.allArgs().some(a => a[0].phase === 'after' && a[0].action === 'update')).toBeTrue();
  });

  it('emits before and after for remove and undo restores', () => {
    const spy = jasmine.createSpy('cardEvent');
    component.cardEvent.subscribe(spy as any);

    const first = component['athletes']()[0];
    component.onDelete(first);
    // simulate confirm
    component.confirmRemove();

    expect(spy.calls.allArgs().some(a => a[0].phase === 'before' && a[0].action === 'remove')).toBeTrue();
    expect(spy.calls.allArgs().some(a => a[0].phase === 'after' && a[0].action === 'remove')).toBeTrue();

    // undo
    component.undoLast();
    expect(component['athletes']().find((x: any) => x.id === first.id)).toBeDefined();
  });
});
