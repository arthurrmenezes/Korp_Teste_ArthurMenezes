import { Component } from '@angular/core';
import { Stock } from '../../services/stock';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatDialogRef, MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';

@Component({
  selector: 'app-product-create-dialog',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './product-create-dialog.html',
  styleUrl: './product-create-dialog.scss'
})
export class ProductCreateDialogComponent {
  public newProduct = {
    code: '',
    description: '',
    balance: 0
  };

  public isRegistering: boolean = false;
  public registerErrorMessage: string = '';

  constructor(
    private stockService: Stock,
    public dialogRef: MatDialogRef<ProductCreateDialogComponent>
  ) {}

  private translateErrorMessage(rawError: string): string {
    try {
      const match = rawError.match(/"message":"([^"]*)"/);
      if (match && match[1]) {
        rawError = match[1];
      }
    } catch (e) {}

    if (
      rawError.includes('Code already exists') ||
      rawError.includes('already exists')
    ) {
      return 'Este código de produto já existe. Tente outro.';
    }

    return 'Ocorreu um erro inesperado no servidor. Por favor, tente novamente.';
  }

  onCancel(): void {
    this.dialogRef.close();
  }

  onSaveProduct() {
    if (!this.newProduct.code || !this.newProduct.description) {
      this.registerErrorMessage = 'Código e descrição são obrigatórios.';
      return;
    }

    this.isRegistering = true;
    this.registerErrorMessage = '';

    this.stockService.registerProduct(this.newProduct).subscribe({
      next: (response) => {
        this.isRegistering = false;
        this.dialogRef.close(response);
      },
      error: (err) => {
        this.isRegistering = false;

        let backendMessage = 'Erro desconhecido';
        if (err.error && err.error.message) {
          backendMessage = err.error.message;
        } else if (typeof err.error === 'string') {
          backendMessage = err.error;
        } else {
          backendMessage = err.statusText;
        }

        this.registerErrorMessage = this.translateErrorMessage(backendMessage);
      }
    });
  }
}
