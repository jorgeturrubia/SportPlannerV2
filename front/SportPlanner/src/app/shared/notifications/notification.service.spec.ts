import { TestBed } from '@angular/core/testing';
import { NotificationService } from './notification.service';

describe('NotificationService', () => {
  let svc: NotificationService;

  beforeEach(() => {
    TestBed.configureTestingModule({ providers: [NotificationService] });
    svc = TestBed.inject(NotificationService);
  });

  it('agrega y elimina notificación manualmente', () => {
    const id = svc.success('mensaje test', 't', true, 10000);
    expect(svc.notifications().length).toBe(1);
    svc.remove(id);
    expect(svc.notifications().length).toBe(0);
  });

  it('clear elimina todas', () => {
    svc.success('a', undefined, true, 10000);
    svc.info('b', undefined, true, 10000);
    expect(svc.notifications().length).toBe(2);
    svc.clear();
    expect(svc.notifications().length).toBe(0);
  });
});
