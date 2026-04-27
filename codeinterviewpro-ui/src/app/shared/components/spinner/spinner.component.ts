import { Component, Input } from '@angular/core';

@Component({
  selector: 'hlm-spinner',
  template: `
    <div 
      class="inline-block animate-spin rounded-full border-solid border-current border-e-transparent align-[-0.125em] motion-reduce:animate-[spin_1.5s_linear_infinite]"
      [class]="sizeClass"
      role="status">
      <span class="!absolute !-m-px !h-px !w-px !overflow-hidden !whitespace-nowrap !border-0 !p-0 ![clip:rect(0,0,0,0)]">Loading...</span>
    </div>
  `
})
export class HlmSpinnerComponent {
  @Input() size: 'sm' | 'md' | 'lg' | 'xl' = 'md';

  get sizeClass() {
    switch(this.size) {
      case 'sm': return 'h-4 w-4 border-2';
      case 'lg': return 'h-8 w-8 border-[3px]';
      case 'xl': return 'h-12 w-12 border-4';
      case 'md':
      default:
        return 'h-6 w-6 border-2';
    }
  }
}
