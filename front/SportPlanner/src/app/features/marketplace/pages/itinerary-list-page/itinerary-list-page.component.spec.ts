import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ItineraryListPageComponent } from './itinerary-list-page.component';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { signal } from '@angular/core';
import { Itinerary } from '../../models/marketplace.models';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

describe('ItineraryListPageComponent', () => {
  let component: ItineraryListPageComponent;
  let fixture: ComponentFixture<ItineraryListPageComponent>;
  let stateServiceMock: Partial<MarketplaceStateService>;

  const itinerariesSignal = signal<Itinerary[]>([]);
  const loadingSignal = signal<boolean>(false);
  const errorSignal = signal<string | null>(null);

  beforeEach(async () => {
    stateServiceMock = {
      itineraries: itinerariesSignal,
      loading: loadingSignal,
      error: errorSignal,
      loadItineraries: jasmine.createSpy('loadItineraries').and.resolveTo(),
    };

    await TestBed.configureTestingModule({
      imports: [ItineraryListPageComponent, NoopAnimationsModule],
      providers: [
        { provide: MarketplaceStateService, useValue: stateServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(ItineraryListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call loadItineraries on init', () => {
    expect(stateServiceMock.loadItineraries).toHaveBeenCalled();
  });

  it('should display itinerary cards from the state service', () => {
    const mockItineraries: Itinerary[] = [
      { id: '1', name: 'Beginner Basketball', items: [] } as Itinerary,
      { id: '2', name: 'Advanced Football', items: [] } as Itinerary,
    ];
    itinerariesSignal.set(mockItineraries);
    fixture.detectChanges();

    const cardElements = fixture.nativeElement.querySelectorAll('app-itinerary-card');
    expect(cardElements.length).toBe(2);
  });

  it('should show loading indicator when loading is true', () => {
    loadingSignal.set(true);
    fixture.detectChanges();

    const skeletonElements = fixture.nativeElement.querySelectorAll('.animate-pulse');
    expect(skeletonElements.length).toBeGreaterThan(0);
  });
});