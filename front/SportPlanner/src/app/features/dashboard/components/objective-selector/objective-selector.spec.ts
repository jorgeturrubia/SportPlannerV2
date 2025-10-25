import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ObjectiveSelector } from './objective-selector';

describe('ObjectiveSelector', () => {
  let component: ObjectiveSelector;
  let fixture: ComponentFixture<ObjectiveSelector>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ObjectiveSelector]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ObjectiveSelector);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
