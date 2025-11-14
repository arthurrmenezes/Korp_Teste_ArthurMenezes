import { Component, Inject } from '@angular/core';
import { Stock } from '../../services/stock';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatDividerModule } from '@angular/material/divider';

@Component({
  selector: 'app-product-adjust-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDividerModule
  ],
  templateUrl: './product-adjust-dialog.html',
  styleUrl: './product-adjust-dialog.scss'
})
export class ProductAdjustDialogComponent {
  public adjustQuantity: number = 0;
  public isAdjusting: boolean = false;
  public errorMessage: string = '';

  constructor(
    private stockService: Stock,
    public dialogRef: MatDialogRef<ProductAdjustDialogComponent>,
    @Inject(MAT_DIALOG_DATA) public product: any
  ) {}

  private translateErrorMessage(rawError: string): string {
    try {
      const match = rawError.match(/"message":"([^"]*)"/);
      if (match && match[1]) rawError = match[1];
    } catch (e) {}

    if (rawError.includes('Insufficient balance')) {
      return 'Saldo insuficiente. A quantidade a ser decrementada é maior que o estoque atual.';
    }

    if (rawError.includes('was not found')) {
      return 'Produto não encontrado. O item pode ter sido excluído.';
    }

    return 'Ocorreu um erro inesperado ao ajustar o estoque.';
  }

  onCancel(): void {
    this.dialogRef.close(false);
  }

  onAdjustBalance(action: 'increment' | 'deduct') {
    if (this.adjustQuantity <= 0) {
      this.errorMessage = 'A quantidade deve ser maior que 0.';
      return;
    }

    this.isAdjusting = true;
    this.errorMessage = '';

    const payload = {
      productList: [
        {
          code: this.product.code,
          quantity: this.adjustQuantity
        }
      ]
    };

    const apiCall =
      action === 'increment'
        ? this.stockService.incrementBalance(payload)
        : this.stockService.deductBalance(payload);

    apiCall.subscribe({
      next: () => {
        this.isAdjusting = false;
        this.dialogRef.close(true);
      },
      error: (err) => {
        this.isAdjusting = false;

        let backendMessage = 'Erro desconhecido';
        if (err.error?.message) {
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
