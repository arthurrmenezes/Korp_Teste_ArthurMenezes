import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Invoice {
  private billingApiUrl = '/api/v1/invoices';

  constructor(private http: HttpClient) { }

  createInvoice(invoiceData: any): Observable<any> {
    return this.http.post(this.billingApiUrl, invoiceData);
  }

  getInvoiceById(invoiceId: number): Observable<any> {
    return this.http.get(`${this.billingApiUrl}/${invoiceId}`);
  }

  addProductToInvoice(invoiceId: number, payload: { items: any[] }): Observable<any> {
    return this.http.post(`${this.billingApiUrl}/${invoiceId}/add-product`, payload);
  }

  printInvoiceById(invoiceId: number): Observable<any> {
    return this.http.post(`${this.billingApiUrl}/${invoiceId}/print`, null);
  }

  getAllInvoices(pageNumber: number, pageSize: number): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

      return this.http.get(this.billingApiUrl, { params });
  }
}
