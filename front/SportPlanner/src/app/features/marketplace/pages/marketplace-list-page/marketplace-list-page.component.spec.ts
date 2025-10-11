import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MarketplaceListPageComponent } from './marketplace-list-page.component';
import { MarketplaceStateService } from '../../services/marketplace-state.service';
import { signal } from '@angular/core';
import { MarketplaceItem, PagedResult, Sport } from '../../models/marketplace.models';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';

describe('MarketplaceListPageComponent', () => {
  let component: MarketplaceListPageComponent;
  let fixture: ComponentFixture<MarketplaceListPageComponent>;
  let stateServiceMock: Partial<MarketplaceStateService>;

  const itemsSignal = signal<MarketplaceItem[]>([]);
  const paginationSignal = signal<Partial<PagedResult<any>> | null>(null);
  const loadingSignal = signal<boolean>(false);
  const errorSignal = signal<string | null>(null);

  beforeEach(async () => {
    stateServiceMock = {
      items: itemsSignal,
      pagination: paginationSignal,
      loading: loadingSignal,
      error: errorSignal,
      searchMarketplace: jasmine.createSpy('searchMarketplace').and.resolveTo(),
    };

    await TestBed.configureTestingModule({
      imports: [MarketplaceListPageComponent, NoopAnimationsModule, FormsModule],
      providers: [
        { provide: MarketplaceStateService, useValue: stateServiceMock },
      ],
    }).compileComponents();

    fixture = TestBed.createComponent(MarketplaceListPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges(); // triggers ngOnInit
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should call searchMarketplace on init', () => {
    expect(stateServiceMock.searchMarketplace).toHaveBeenCalled();
  });

  it('should display items from the state service', () => {
    const mockItems: MarketplaceItem[] = [
      { id: '1', name: 'Test Item 1', description: 'Desc 1', sport: Sport.General, type: 'Exercise' } as MarketplaceItem,
      { id: '2', name: 'Test Item 2', description: 'Desc 2', sport: Sport.Football, type: 'TrainingPlan' } as MarketplaceItem,
    ];
    itemsSignal.set(mockItems);
    fixture.detectChanges();

    const cardElements = fixture.nativeElement.querySelectorAll('app-marketplace-item-card');
    expect(cardElements.length).toBe(2);
  });

  it('should show loading indicator when loading is true', () => {
    loadingSignal.set(true);
    fixture.detectChanges();

    const skeletonElements = fixture.nativeElement.querySelectorAll('.animate-pulse');
    expect(skeletonElements.length).toBeGreaterThan(0);
  });

  it('should show error message when error is set', () => {
    const errorMessage = 'Failed to load';
    errorSignal.set(errorMessage);
    fixture.detectChanges();

    const errorElement = fixture.nativeElement.querySelector('.bg-red-50');
    expect(errorElement).toBeTruthy();
    expect(errorElement.textContent).toContain(errorMessage);
  });

  it('should show "No Items Found" message when items array is empty and not loading', () => {
    itemsSignal.set([]);
    loadingSignal.set(false);
    errorSignal.set(null);
    fixture.detectChanges();

    const noItemsElement = fixture.nativeElement.querySelector('.text-center h3');
    expect(noItemsElement.textContent).toContain('No Items Found');
  });
});