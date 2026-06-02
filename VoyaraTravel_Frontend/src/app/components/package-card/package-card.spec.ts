import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PackageCard } from './package-card';

describe('PackageCard', () => {
  let component: PackageCard;
  let fixture: ComponentFixture<PackageCard>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PackageCard]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PackageCard);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
