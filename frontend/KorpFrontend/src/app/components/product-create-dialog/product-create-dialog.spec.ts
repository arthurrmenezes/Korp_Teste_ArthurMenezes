import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductCreateDialog } from './product-create-dialog';

describe('ProductCreateDialog', () => {
  let component: ProductCreateDialog;
  let fixture: ComponentFixture<ProductCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ProductCreateDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(ProductCreateDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
