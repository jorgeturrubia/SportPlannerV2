import { TestBed, fakeAsync, tick } from '@angular/core/testing';
import { MarketplaceStateService } from './marketplace-state.service';
import { MarketplaceApiService } from './marketplace-api.service';
import { of, throwError } from 'rxjs';
import { PagedResult, Sport } from '../models/marketplace.models';

describe('MarketplaceStateService', () => {
  let service: MarketplaceStateService;
  let apiServiceMock: jasmine.SpyObj<MarketplaceApiService>;

  beforeEach(() => {
    const spy = jasmine.createSpyObj('MarketplaceApiService', [
      'searchMarketplace',
      'getItineraries',
      'downloadFromMarketplace',
    ]);

    TestBed.configureTestingModule({
      providers: [
        MarketplaceStateService,
        { provide: MarketplaceApiService, useValue: spy },
      ],
    });

    service = TestBed.inject(MarketplaceStateService);
    apiServiceMock = TestBed.inject(MarketplaceApiService) as jasmine.SpyObj<MarketplaceApiService>;
  });

  it('should be created with initial state', () => {
    expect(service).toBeTruthy();
    expect(service.items()).toEqual([]);
    expect(service.itineraries()).toEqual([]);
    expect(service.loading()).toBe(false);
    expect(service.error()).toBeNull();
  });

  describe('searchMarketplace', () => {
    it('should set loading to true, fetch items, and update state on success', fakeAsync(() => {
      const mockResult: PagedResult<any> = { items: [{ id: '1', name: 'Test Item' }], pageNumber: 1, pageSize: 1, totalCount: 1, totalPages: 1, hasPreviousPage: false, hasNextPage: false };
      apiServiceMock.searchMarketplace.and.returnValue(of(mockResult));

      service.searchMarketplace({ sport: Sport.General });

      expect(service.loading()).toBe(true);

      tick(); // Resolve the promise

      expect(service.items().length).toBe(1);
      expect(service.items()[0].name).toBe('Test Item');
      expect(service.loading()).toBe(false);
      expect(service.error()).toBeNull();
    }));

    it('should set loading to true, then set error state on failure', fakeAsync(() => {
      const errorResponse = new Error('Failed to fetch');
      apiServiceMock.searchMarketplace.and.returnValue(throwError(() => errorResponse));

      service.searchMarketplace({ sport: Sport.General });

      expect(service.loading()).toBe(true);

      tick(); // Resolve the promise

      expect(service.items().length).toBe(0);
      expect(service.loading()).toBe(false);
      expect(service.error()).toBe('Failed to fetch');
    }));
  });

  describe('loadItineraries', () => {
    it('should set loading to true, fetch itineraries, and update state on success', fakeAsync(() => {
      const mockItineraries = [{ id: 'it-1', name: 'Test Itinerary' }];
      apiServiceMock.getItineraries.and.returnValue(of(mockItineraries as any));

      service.loadItineraries();

      expect(service.loading()).toBe(true);

      tick();

      expect(service.itineraries().length).toBe(1);
      expect(service.itineraries()[0].name).toBe('Test Itinerary');
      expect(service.loading()).toBe(false);
      expect(service.error()).toBeNull();
    }));
  });

  describe('downloadItem', () => {
    it('should call the api service and return success on ok', fakeAsync(() => {
      apiServiceMock.downloadFromMarketplace.and.returnValue(of({ clonedEntityId: 'new-id' }));

      let result: { success: boolean; error?: string } | undefined;
      service.downloadItem('test-id').then(res => result = res);

      tick();

      expect(apiServiceMock.downloadFromMarketplace).toHaveBeenCalledWith('test-id');
      expect(result?.success).toBe(true);
      expect(result?.error).toBeUndefined();
    }));
  });
});