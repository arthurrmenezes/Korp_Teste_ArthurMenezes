import { Component, Inject, OnInit } from '@angular/core';
import { Invoice } from '../../services/invoice';
import { Stock } from '../../services/stock';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';

interface InvoiceItem {
  productCode: string;
  quantity: number;
  description: string;
}

@Component({
  selector: 'app-invoice-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatListModule,
    MatIconModule,
    MatProgressSpinnerModule,
    MatSelectModule
  ],
  templateUrl: './invoice-create-dialog.html',
  styleUrl: './invoice-create-dialog.scss'
})
export class InvoiceCreateDialogComponent implements OnInit {
  public selectedProduct: any = null;
  public newProductQuantity: number = 0;
  public items: InvoiceItem[] = [];
  public isSaving: boolean = false;
  public errorMessage: string = '';

  public allProducts: any[] = [];
  public isLoadingProducts = false;

  constructor(
    private invoiceService: Invoice,
    private stockService: Stock,
    public dialogRef: MatDialogRef<InvoiceCreateDialogComponent>
  ) {}

  ngOnInit(): void {
    this.loadAllProducts();
  }

  private translateErrorMessage(rawError: string): string {
    try {
      const match = rawError.match(/"message":"([^"]*)"/);
      if (match && match[1]) {
        rawError = match[1];
      }
    } catch (e) {}

    if (rawError.includes("Insufficient balance")) {
      return "Erro ao salvar: O estoque de um ou mais produtos é insuficiente.";
    }

    if (rawError.includes("was not found")) {
      return "Erro ao salvar: Um ou mais produtos não foram encontrados e podem ter sido excluídos.";
    }

    return "Ocorreu um erro inesperado ao salvar a nota. Por favor, tente novamente.";
  }

  loadAllProducts() {
    this.isLoadingProducts = true;
    this.stockService.getAllProducts(1, 9999).subscribe({
      next: (response) => {
        this.allProducts = response.products || [];
        this.isLoadingProducts = false;
      },
      error: () => {
        this.errorMessage = "Erro ao carregar lista de produtos.";
        this.isLoadingProducts = false;
      }
    });
  }

  addItem() {
    if (!this.selectedProduct || this.newProductQuantity <= 0) {
      this.errorMessage = "Selecione um produto e insira uma quantidade maior que zero.";
      return;
    }

    this.errorMessage = '';

    const quantityToAdd = this.newProductQuantity;
    const availableStock = this.selectedProduct.balance;

    const existingItem = this.items.find(
      item => item.productCode === this.selectedProduct.code
    );

    if (existingItem) {
      const newTotalQuantity = existingItem.quantity + quantityToAdd;

      if (newTotalQuantity > availableStock) {
        this.errorMessage = `Erro ao adicionar, foi excedido o estoque disponível (${availableStock}). Você já adicionou ${existingItem.quantity} items na lista.`;
        return;
      }

      existingItem.quantity = newTotalQuantity;

    } else {
      if (quantityToAdd > availableStock) {
        this.errorMessage = `Quantidade indisponível. Saldo em estoque: ${availableStock}`;
        return;
      }

      this.items.push({
        productCode: this.selectedProduct.code,
        quantity: quantityToAdd,
        description: this.selectedProduct.description
      });
    }

    this.selectedProduct = null;
    this.newProductQuantity = 0;
  }

  removeItem(index: number) {
    this.items.splice(index, 1);
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSave() {
    if (this.items.length === 0) {
      this.errorMessage = 'Adicione pelo menos um produto para salvar a nota.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    const payload = {
      items: this.items.map(item => ({
        productCode: item.productCode,
        quantity: item.quantity
      }))
    };

    this.invoiceService.createInvoice(payload).subscribe({
      next: (response) => {
        this.isSaving = false;
        this.dialogRef.close(response);
      },
      error: (err) => {
        this.isSaving = false;

        let backendMessage = "Erro desconhecido";
        if (err.error && err.error.message) {
          backendMessage = err.error.message;
        } else if (typeof err.error === 'string') {
          backendMessage = err.error;
        } else {
          backendMessage = err.statusText;
        }

        this.errorMessage = this.translateErrorMessage(backendMessage);
      }
    });
  }
}
