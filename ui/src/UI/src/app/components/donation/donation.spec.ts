import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DonationsComponent } from './donation';

describe('DonationsComponent', () => {
  let component: DonationsComponent;
  let fixture: ComponentFixture<DonationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DonationsComponent],
    }).compileComponents();

    fixture = TestBed.createComponent(DonationsComponent);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
