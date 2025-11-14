import { Component, OnInit } from '@angular/core';
import { Stock } from '../../services/stock';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { ProductCreateDialogComponent } from '../product-create-dialog/product-create-dialog';
import { ProductAdjustDialogComponent } from '../product-adjust-dialog/product-adjust-dialog';

@Component({
  selector: 'app-stock-component',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatTableModule,
    MatPaginatorModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatDividerModule
  ],
  templateUrl: './stock-component.html',
  styleUrl: './stock-component.scss'
})
export class StockComponent implements OnInit {
  public productList: any[] = [];
  public displayedColumns: string[] = ['code', 'description', 'balance', 'createdAt', 'actions'];
  public totalProducts = 0;
  public currentPage = 0;
  public currentPageSize = 5;
  public isListLoading = false;

  public sortColumn: string = 'code';
  public sortDirection: 'asc' | 'desc' = 'asc';
  public sortOptions = [
    { value: 'code', viewValue: 'ID (Código)' },
    { value: 'description', viewValue: 'Descrição (A-Z)' },
    { value: 'balance', viewValue: 'Estoque (Saldo)' },
    { value: 'createdAt', viewValue: 'Data de Criação' }
  ];

  public productCode: string = '';
  public foundProduct: any = null;
  public isLoading: boolean = false;
  public errorMessage: string = '';

  constructor(
    private stockService: Stock,
    public dialog: MatDialog,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.loadProducts();
  }

  private translateErrorMessage(rawError: string): string {
    const notFoundMatch = rawError.match(/Product with code (.*?) was not found/);
    if (notFoundMatch && notFoundMatch[1]) {
      return `Produto com código ${notFoundMatch[1]} não foi encontrado.`;
    }
    if (rawError.includes('Insufficient balance')) {
      return 'Saldo insuficiente em estoque.';
    }
    if (rawError.includes('Code already exists')) {
      return 'Este código de produto já existe. Tente outro.';
    }
    return 'Ocorreu um erro inesperado. Por favor, tente novamente.';
  }

  loadProducts() {
    this.isListLoading = true;
    this.productList = [];

    this.stockService.getAllProducts(this.currentPage + 1, this.currentPageSize).subscribe({
      next: (response) => {
        this.totalProducts = response.totalProducts;
        this.productList = response.products || [];
        this.sortProductList();
        this.isListLoading = false;
      },
      error: (err) => {
        const backendMessage = err.error?.message || err.error || 'Erro ao carregar produtos.';
        this.errorMessage = this.translateErrorMessage(backendMessage);

        this.isListLoading = false;
        this.productList = [];
        this.totalProducts = 0;
      }
    });
  }

  openCreateProductDialog(): void {
    const dialogRef = this.dialog.open(ProductCreateDialogComponent, {
      width: '500px',
      disableClose: true
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.snackBar.open(`Produto "${result.description}" cadastrado com sucesso!`, 'Fechar', {
          duration: 3000,
          verticalPosition: 'top'
        });
        this.loadProducts();
      }
    });
  }

  onSearch() {
    if (!this.productCode) return;

    this.isLoading = true;
    this.foundProduct = null;
    this.errorMessage = '';

    this.stockService.getProductByCode(this.productCode).subscribe({
      next: (product) => {
        this.foundProduct = product;
        this.isLoading = false;
      },
      error: (err) => {
        this.isLoading = false;
        this.foundProduct = null;

        const backendMessage = err.error?.message || err.error || err.statusText;
        this.errorMessage = this.translateErrorMessage(backendMessage);
      }
    });
  }

  clearSearch() {
    this.foundProduct = null;
    this.productCode = '';
    this.errorMessage = '';
  }

  onPageChange(event: PageEvent) {
    this.currentPage = event.pageIndex;
    this.currentPageSize = event.pageSize;
    this.loadProducts();
  }

  onSortChange() {
    this.sortProductList();
  }

  toggleSortDirection() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortProductList();
  }

  sortProductList() {
    if (!this.productList || this.productList.length === 0) return;

    const direction = this.sortDirection === 'asc' ? 1 : -1;
    const column = this.sortColumn;

    this.productList.sort((a: any, b: any) => {
      const valA = a[column];
      const valB = b[column];
      let comparison = 0;

      if (column === 'description' || column === 'code') {
        comparison = valA.localeCompare(valB, undefined, { numeric: true, sensitivity: 'base' });
      } else if (column === 'balance') {
        comparison = valA - valB;
      } else if (column === 'createdAt') {
        comparison = new Date(valA).getTime() - new Date(valB).getTime();
      }

      return comparison * direction;
    });

    this.productList = [...this.productList];
  }

  openAdjustStockDialog(product: any): void {
    const dialogRef = this.dialog.open(ProductAdjustDialogComponent, {
      width: '450px',
      disableClose: true,
      data: product
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.snackBar.open(`Estoque de "${product.description}" ajustado!`, 'Fechar', {
          duration: 3000,
          verticalPosition: 'top'
        });
        this.loadProducts();
      }
    });
  }
}
