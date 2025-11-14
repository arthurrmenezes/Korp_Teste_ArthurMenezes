import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ChatbotService {
  private apiUrl = '/api/v1/ai/chat'; 

  constructor(private http: HttpClient) { }

  ask(question: string): Observable<any> {
    const payload = { question: question };
    return this.http.post<any>(this.apiUrl, payload);
  }
}