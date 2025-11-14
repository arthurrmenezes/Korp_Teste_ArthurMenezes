import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceAddProductDialog } from './invoice-add-product-dialog';

describe('InvoiceAddProductDialog', () => {
  let component: InvoiceAddProductDialog;
  let fixture: ComponentFixture<InvoiceAddProductDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceAddProductDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoiceAddProductDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
