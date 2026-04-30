import { toast } from 'src/app/core/services/toast';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AdminService } from 'src/app/core/services/admin.service';

@Component({
  selector: 'app-create-hr',
  templateUrl: './create-hr.component.html'
})
export class CreateHrComponent {
  @Input() tenantId = '';
  @Output() onCreated = new EventEmitter<void>();

  form = {
    fullName: '',
    email: '',
    password: ''
  };
  loading = false;

  constructor(private adminService: AdminService) {}

  onSubmit() {
    if (!this.tenantId) return;

    this.loading = true;
    const payload = {
      ...this.form,
      tenantId: this.tenantId
    };

    this.adminService.createHr(payload).subscribe({
      next: () => {
        toast.success('HR User created successfully');
        this.loading = false;
        this.onCreated.emit();
        this.form = { fullName: '', email: '', password: '' };
      },
      error: (err) => {
        this.loading = false;
        toast.error(err?.error?.message || 'Failed to create HR user');
      }
    });
  }
}
