import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductAdjustDialog } from './product-adjust-dialog';

describe('ProductAdjustDialog', () => {
  let component: ProductAdjustDialog;
  let fixture: ComponentFixture<ProductAdjustDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductAdjustDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductAdjustDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
