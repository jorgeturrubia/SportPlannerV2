import { ComponentFixture, TestBed, fakeAsync, tick } from '@angular/core/testing';
import { ActivatedRoute } from '@angular/router';
import { of, throwError } from 'rxjs';
import { MarketplaceDetailPageComponent } from './marketplace-detail-page.component';
import { MarketplaceApiService } from '../../services/marketplace-api.service';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { NotificationService } from '../../../../shared/notifications/notification.service';
import { MarketplaceItem, Sport } from '../../models/marketplace.models';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('MarketplaceDetailPageComponent', () => {
  let component: MarketplaceDetailPageComponent;
  let fixture: ComponentFixture<MarketplaceDetailPageComponent>;
  let apiServiceMock: jasmine.SpyObj<MarketplaceApiService>;
  let stateServiceMock: jasmine.SpyObj<MarketplaceStateService>;
  let notificationServiceMock: jasmine.SpyObj<NotificationService>;

  const mockItem: MarketplaceItem = {
    id: 'test-id',
    name: 'Test Item Detail',
    description: 'Detailed description',
    sport: Sport.Basketball,
    type: 'Exercise',
    ratings: []
  } as MarketplaceItem;

  beforeEach(async () => {
    const apiSpy = jasmine.createSpyObj('MarketplaceApiService', ['getMarketplaceItemById']);
    const stateSpy = jasmine.createSpyObj('MarketplaceStateService', ['downloadItem']);
    const notificationSpy = jasmine.createSpyObj('NotificationService', ['success', 'error']);

    await TestBed.configureTestingModule({
      imports: [MarketplaceDetailPageComponent, NoopAnimationsModule],
      providers: [
        {
          provide: ActivatedRoute,
          useValue: {
            snapshot: {
              paramMap: {
                get: (key: string) => (key === 'id' ? 'test-id' : null),
              },
            },
          },
        },
        { provide: MarketplaceApiService, useValue: apiSpy },
        { provide: MarketplaceStateService, useValue: stateSpy },
        { provide: NotificationService, useValue: notificationSpy },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(MarketplaceDetailPageComponent);
    component = fixture.componentInstance;
    apiServiceMock = TestBed.inject(MarketplaceApiService) as jasmine.SpyObj<MarketplaceApiService>;
    stateServiceMock = TestBed.inject(MarketplaceStateService) as jasmine.SpyObj<MarketplaceStateService>;
    notificationServiceMock = TestBed.inject(NotificationService) as jasmine.SpyObj<NotificationService>;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load item on init', fakeAsync(() => {
    apiServiceMock.getMarketplaceItemById.and.returnValue(of(mockItem));

    fixture.detectChanges(); // ngOnInit
    tick(); // resolve promise

    expect(apiServiceMock.getMarketplaceItemById).toHaveBeenCalledWith('test-id');
    expect(component.item()).toEqual(mockItem);
    expect(component.loading()).toBe(false);
  }));

  it('should handle error on load item failure', fakeAsync(() => {
    apiServiceMock.getMarketplaceItemById.and.returnValue(throwError(() => new Error('Not Found')));

    fixture.detectChanges();
    tick();

    expect(component.item()).toBeNull();
    expect(component.loading()).toBe(false);
    expect(component.error()).toBe('Not Found');
  }));

  it('should call downloadItem and show success notification', fakeAsync(() => {
    // First, load the item
    apiServiceMock.getMarketplaceItemById.and.returnValue(of(mockItem));
    fixture.detectChanges();
    tick();

    // Then, test the download
    stateServiceMock.downloadItem.and.resolveTo({ success: true });

    component.downloadItem();
    expect(component.isDownloading()).toBe(true);

    tick(); // resolve promise from downloadItem

    expect(stateServiceMock.downloadItem).toHaveBeenCalledWith('test-id');
    expect(notificationServiceMock.success).toHaveBeenCalled();
    expect(notificationServiceMock.error).not.toHaveBeenCalled();
    expect(component.isDownloading()).toBe(false);
  }));
});