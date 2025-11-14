import { Component, OnInit } from '@angular/core';
import { Invoice } from '../../services/invoice';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatListModule } from '@angular/material/list';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatIconModule } from '@angular/material/icon';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { InvoiceCreateDialogComponent } from '../invoice-create-dialog/invoice-create-dialog';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTableModule } from '@angular/material/table';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatSelectModule } from '@angular/material/select';
import { InvoiceAddProductDialogComponent } from '../invoice-add-product-dialog/invoice-add-product-dialog';

@Component({
  selector: 'app-invoice-component',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatCardModule,
    MatListModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatIconModule,
    MatDialogModule,
    MatSnackBarModule,
    MatTableModule,
    MatPaginatorModule,
    MatSelectModule
  ],
  templateUrl: './invoice-component.html',
  styleUrl: './invoice-component.scss',
})
export class InvoiceComponent implements OnInit {
  public currentInvoice: any = null;
  public isLoading: boolean = false;
  public errorMessage: string = '';

  public loadInvoiceId: number | null = null;

  public invoiceList: any[] = [];
  public displayedColumns: string[] = ['invoiceId', 'status', 'totalProducts', 'createdAt', 'actions'];
  public totalInvoices = 0;
  public currentPage = 0;
  public currentPageSize = 5;
  public isListLoading = false;

  public sortColumn: string = 'invoiceId';
  public sortDirection: 'asc' | 'desc' = 'asc'; 
  public sortOptions = [
    { value: 'invoiceId', viewValue: 'Nº Fatura' },
    { value: 'totalProducts', viewValue: 'Qtd. Itens' },
    { value: 'createdAt', viewValue: 'Data' }
  ];

  public itemDisplayedColumns: string[] = ['code', 'description', 'quantity'];

  constructor(
    private invoiceService: Invoice,
    public dialog: MatDialog,
    private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.loadInvoices();
  }

  private getErrorMessage(err: any): string {
    let backendMessage = "Erro desconhecido";
    
    if (typeof err.error === 'string') {
      backendMessage = err.error;
    } else if (err.error && typeof err.error.message === 'string') {
      backendMessage = err.error.message;
    } else if (err.message) {
      backendMessage = err.message;
    } else {
      backendMessage = err.statusText || 'Erro desconhecido';
    }

    try {
      const match = backendMessage.match(/"message":"([^"]*)"/);
      if (match && match[1]) {
        backendMessage = match[1];
      }
    } catch (e) {}

    if (backendMessage.includes('Insufficient balance')) {
      return 'Erro ao emitir: Saldo insuficiente em estoque para um ou mais produtos.';
    }
    if (backendMessage.includes('was not found') || backendMessage.includes('Nota fiscal não encontrada')) {
      return `A nota fiscal com ID ${this.loadInvoiceId} não foi encontrada.`;
    }
    if (backendMessage.includes('Erro ao carregar faturas')) {
      return 'Não foi possível carregar a lista de faturas.';
    }
    if (backendMessage.includes('Erro ao adicionar produto')) {
      return 'Não foi possível adicionar o produto. Verifique os dados e o estoque.';
    }
    if (backendMessage.includes('Erro ao atualizar dados da nota')) {
      return 'Não foi possível atualizar os dados da nota. Tente novamente.';
    }
    
    return 'Ocorreu um erro inesperado. Por favor, tente novamente.';
  }

  loadInvoices() {
    this.isListLoading = true;
    this.invoiceList = [];

    this.invoiceService.getAllInvoices(this.currentPage + 1, this.currentPageSize).subscribe({
      next: (response) => {
        this.invoiceList = response.invoices;
        this.totalInvoices = response.totalInvoices;
        this.sortInvoiceList();
        this.isListLoading = false;
      },
      error: (err) => {
        this.errorMessage = this.getErrorMessage(err);
        this.isListLoading = false;
      }
    });
  }

  onPageChange(event: PageEvent) {
    this.currentPage = event.pageIndex;
    this.currentPageSize = event.pageSize;
    this.loadInvoices();
  }

  viewInvoice(invoice: any) {
    this.loadInvoiceId = invoice.invoiceId;
    this.onLoadInvoice();
  }

  onCreateInvoice() {
    this.errorMessage = '';
    
    const dialogRef = this.dialog.open(InvoiceCreateDialogComponent, {
      width: '600px', 
      disableClose: true 
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.currentInvoice = result; 
        this.loadInvoices();
        this.snackBar.open(
          `Nota Fiscal Nº ${result.invoiceId} criada com sucesso!`,
          'Fechar',
          {
            duration: 3000,
            horizontalPosition: 'center',
            verticalPosition: 'top',
          }
        );
      }
    });
  }

  onLoadInvoice() {
    if (!this.loadInvoiceId || this.loadInvoiceId <= 0) {
      this.errorMessage = 'Por favor, insira um ID de nota fiscal válido.';
      return;
    }

    this.isLoading = true;
    this.errorMessage = '';
    this.currentInvoice = null;

    this.invoiceService.getInvoiceById(this.loadInvoiceId).subscribe({
      next: (response) => {
        this.currentInvoice = response;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = this.getErrorMessage(err);
        this.isLoading = false;
      }
    });
  }

  onPrintInvoice() {
    this.isLoading = true;
    this.errorMessage = ''; 

    this.invoiceService.printInvoiceById(this.currentInvoice.invoiceId).subscribe({
      next: () => {
        this.snackBar.open('Nota Fiscal emitida com sucesso!', 'Fechar', {
          duration: 3000,
          verticalPosition: 'top'
        });
        
        this.refreshCurrentInvoice(); 
      },
      error: (err) => { 
        this.isLoading = false;
        this.errorMessage = this.getErrorMessage(err);
      }
    });
  }

  refreshCurrentInvoice() {
    if (!this.currentInvoice?.invoiceId) return;

    const id = this.currentInvoice.invoiceId;
    this.isLoading = true;

    this.invoiceService.getInvoiceById(id).subscribe({
      next: (response) => {
        this.currentInvoice = response;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = this.getErrorMessage('Erro ao atualizar dados da nota.');
        this.isLoading = false;
      }
    });
  }

  clearAll() {
    this.currentInvoice = null;
    this.loadInvoiceId = null;
    this.errorMessage = '';
    this.loadInvoices();
  }

  onSortChange() {
    this.sortInvoiceList();
  }
  
  toggleSortDirection() {
    this.sortDirection = this.sortDirection === 'asc' ? 'desc' : 'asc';
    this.sortInvoiceList();
  }

  sortInvoiceList() {
    if (!this.invoiceList || this.invoiceList.length === 0) return;

    const direction = this.sortDirection === 'asc' ? 1 : -1;
    const column = this.sortColumn;

    this.invoiceList.sort((a: any, b: any) => {
      const valA = a[column];
      const valB = b[column];
      let comparison = 0;

      if (column === 'invoiceId' || column === 'totalProducts') {
        comparison = valA - valB;
      } else if (column === 'createdAt') {
        comparison = new Date(valA).getTime() - new Date(valB).getTime();
      } else {
        comparison = valA.localeCompare(valB);
      }

      return comparison * direction;
    });

    this.invoiceList = [...this.invoiceList];
  }

  openEditInvoiceDialog(): void {
    const dialogRef = this.dialog.open(InvoiceAddProductDialogComponent, {
      width: '600px',
      disableClose: true,
      data: { invoiceId: this.currentInvoice.invoiceId } 
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result === true) {
        this.snackBar.open("Produtos adicionados com sucesso!", 'Fechar', {
          duration: 3000,
          verticalPosition: 'top'
        });
        this.refreshCurrentInvoice();
      }
    });
  }
}