import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class Stock {
  private stockApiUrl = '/api/v1/products';

  constructor(private http: HttpClient) { }

  registerProduct(productData: any): Observable<any> {
    return this.http.post(this.stockApiUrl, productData);
  }

  getProductByCode(code: string): Observable<any> {
    return this.http.get(`${this.stockApiUrl}/code/${code}`);
  }

  incrementBalance(payload: { productList: any[] }): Observable<any> {
    return this.http.post(`${this.stockApiUrl}/increment-balance`, payload);
  }

  deductBalance(payload: { productList: any[] }): Observable<any> {
    return this.http.post(`${this.stockApiUrl}/deduct-balance`, payload);
  }

  getAllProducts(pageNumber: number, pageSize: number): Observable<any> {
    const params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get(this.stockApiUrl, { params });
  }
}
