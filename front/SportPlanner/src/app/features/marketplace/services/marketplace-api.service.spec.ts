import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { MarketplaceApiService, MarketplaceSearchParams } from './marketplace-api.service';
import { PagedResult, Sport } from '../models/marketplace.models';

describe('MarketplaceApiService', () => {
  let service: MarketplaceApiService;
  let httpTestingController: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [MarketplaceApiService],
    });
    service = TestBed.inject(MarketplaceApiService);
    httpTestingController = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpTestingController.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  describe('searchMarketplace', () => {
    it('should make a GET request with the correct parameters', () => {
      const searchParams: MarketplaceSearchParams = { sport: Sport.General, page: 1, pageSize: 10 };
      const mockResult: PagedResult<any> = { items: [], pageNumber: 1, pageSize: 10, totalCount: 0, totalPages: 0, hasPreviousPage: false, hasNextPage: false };

      service.searchMarketplace(searchParams).subscribe(result => {
        expect(result).toEqual(mockResult);
      });

      const req = httpTestingController.expectOne(
        r => r.url === '/api/planning/marketplace' && r.method === 'GET'
      );

      expect(req.request.params.get('sport')).toBe(Sport.General);
      expect(req.request.params.get('page')).toBe('1');
      expect(req.request.params.get('pageSize')).toBe('10');

      req.flush(mockResult);
    });
  });

  describe('getMarketplaceItemById', () => {
    it('should make a GET request to the correct item URL', () => {
      const itemId = 'test-id';
      const mockItem = { id: itemId, name: 'Test Item' };

      service.getMarketplaceItemById(itemId).subscribe(item => {
        expect(item).toEqual(mockItem as any);
      });

      const req = httpTestingController.expectOne(`/api/planning/marketplace/${itemId}`);
      expect(req.request.method).toBe('GET');

      req.flush(mockItem);
    });
  });

  describe('downloadFromMarketplace', () => {
    it('should make a POST request to the correct download URL', () => {
      const itemId = 'test-id';
      const mockResponse = { clonedEntityId: 'new-id' };

      service.downloadFromMarketplace(itemId).subscribe(response => {
        expect(response).toEqual(mockResponse);
      });

      const req = httpTestingController.expectOne(`/api/planning/marketplace/${itemId}/download`);
      expect(req.request.method).toBe('POST');
      expect(req.request.body).toEqual({});

      req.flush(mockResponse);
    });
  });

  describe('getItineraries', () => {
    it('should make a GET request to the itineraries URL', () => {
      const mockItineraries = [{ id: 'it-id', name: 'Test Itinerary' }];

      service.getItineraries().subscribe(itineraries => {
        expect(itineraries).toEqual(mockItineraries as any);
      });

      const req = httpTestingController.expectOne('/api/planning/itineraries');
      expect(req.request.method).toBe('GET');

      req.flush(mockItineraries);
    });
  });
});