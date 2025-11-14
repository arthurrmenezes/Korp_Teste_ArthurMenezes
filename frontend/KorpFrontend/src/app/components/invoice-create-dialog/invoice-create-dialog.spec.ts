import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InvoiceCreateDialog } from './invoice-create-dialog';

describe('InvoiceCreateDialog', () => {
  let component: InvoiceCreateDialog;
  let fixture: ComponentFixture<InvoiceCreateDialog>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [InvoiceCreateDialog]
    })
    .compileComponents();

    fixture = TestBed.createComponent(InvoiceCreateDialog);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
