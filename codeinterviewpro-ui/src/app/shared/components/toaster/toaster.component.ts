import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';

interface Toast {
  id: string;
  type: string;
  message: string;
  options?: any;
}

@Component({
  selector: 'app-toaster',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="fixed bottom-8 right-8 z-[9999] flex flex-col gap-3 pointer-events-none">
      <div *ngFor="let t of toasts" 
           class="pointer-events-auto px-6 py-4 rounded-2xl shadow-[0_20px_50px_rgba(0,0,0,0.3)] backdrop-blur-xl border border-white/10 min-w-[320px] transform transition-all duration-500 animate-in fade-in slide-in-from-right-10 bg-slate-900/95 text-white">
        <div class="flex items-center gap-4">
          <div *ngIf="t.type === 'loading'" class="animate-spin h-5 w-5 border-2 border-white/30 border-t-white rounded-full"></div>
          <div *ngIf="t.type === 'success'" class="text-emerald-400">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M5 13l4 4L19 7"></path></svg>
          </div>
          <div *ngIf="t.type === 'error'" class="text-rose-400">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M6 18L18 6M6 6l12 12"></path></svg>
          </div>
          <div *ngIf="t.type === 'info'" class="text-indigo-400">
            <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2.5" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"></path></svg>
          </div>
          <p class="text-sm font-semibold tracking-tight">{{ t.message }}</p>
        </div>
      </div>
    </div>
  `
})
export class ToasterComponent implements OnInit, OnDestroy {
  toasts: Toast[] = [];

  ngOnInit() {
    window.addEventListener('app-toast', this.handleToast as any);
    window.addEventListener('app-toast-promise-start', this.handlePromiseStart as any);
    window.addEventListener('app-toast-promise-resolve', this.handlePromiseResolve as any);
  }

  ngOnDestroy() {
    window.removeEventListener('app-toast', this.handleToast as any);
    window.removeEventListener('app-toast-promise-start', this.handlePromiseStart as any);
    window.removeEventListener('app-toast-promise-resolve', this.handlePromiseResolve as any);
  }

  handleToast = (e: CustomEvent) => {
    const toast = e.detail;
    this.addToast(toast);
  }

  handlePromiseStart = (e: CustomEvent) => {
    const { id, message } = e.detail;
    this.addToast({ id, type: 'loading', message });
  }

  handlePromiseResolve = (e: CustomEvent) => {
    const { id, type, message } = e.detail;
    const index = this.toasts.findIndex(t => t.id === id);
    if (index > -1) {
      this.toasts[index].type = type;
      this.toasts[index].message = message;
      setTimeout(() => this.removeToast(id), 3000);
    }
  }

  addToast(toast: Toast) {
    this.toasts.push(toast);
    if (toast.type !== 'loading') {
      setTimeout(() => this.removeToast(toast.id), 5000);
    }
  }

  removeToast(id: string) {
    this.toasts = this.toasts.filter(t => t.id !== id);
  }
}
