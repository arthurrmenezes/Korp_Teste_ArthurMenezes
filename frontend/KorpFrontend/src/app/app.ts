import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatTabsModule } from '@angular/material/tabs';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatSidenavModule } from '@angular/material/sidenav';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { StockComponent } from './components/stock-component/stock-component';
import { InvoiceComponent } from './components/invoice-component/invoice-component';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { ChatbotService } from './services/chatbot';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    MatToolbarModule,
    MatButtonModule,
    MatSidenavModule,
    MatListModule,
    MatIconModule,
    StockComponent,
    InvoiceComponent,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatProgressSpinnerModule
  ],
  templateUrl: './app.html',
  styleUrl: './app.scss',
  host: {
    '[class.sidenav-minimized]': 'isSidenavMinimized'
  }
})
export class AppComponent {
  title = 'Sistema de emissão de Notas Fiscais';

  public isSidenavMinimized = false;
  public activePage: 'products' | 'invoices' = 'products';

  public isChatOpen = false;
  public isChatLoading = false;
  public currentUserQuestion = '';
  public chatMessages: { role: 'user' | 'bot'; text: string }[] = [];

  constructor(private chatbotService: ChatbotService) {}

  toggleSidenav() {
    this.isSidenavMinimized = !this.isSidenavMinimized;
  }

  setActivePage(page: 'products' | 'invoices') {
    this.activePage = page;
  }

  toggleChat() {
    this.isChatOpen = !this.isChatOpen;
    if (this.isChatOpen && this.chatMessages.length === 0) {
      this.chatMessages.push({
        role: 'bot',
        text: 'Olá! Sou seu assistente. Como posso te ajudar?'
      });
    }
  }

  sendChatMessage() {
    const question = this.currentUserQuestion.trim();
    if (!question || this.isChatLoading) return;

    this.chatMessages.push({ role: 'user', text: question });
    this.currentUserQuestion = '';
    this.isChatLoading = true;

    this.chatbotService.ask(question).subscribe({
      next: (response) => {
        this.chatMessages.push({ role: 'bot', text: response.answer });
        this.isChatLoading = false;
      },
      error: () => {
        this.chatMessages.push({
          role: 'bot',
          text: 'Desculpe, não consegui processar sua pergunta. Tente novamente.'
        });
        this.isChatLoading = false;
      }
    });
  }
}
