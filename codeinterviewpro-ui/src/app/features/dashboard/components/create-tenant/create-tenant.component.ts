import { toast } from 'src/app/core/services/toast';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TenantService } from 'src/app/core/services/tenant.service';

@Component({
  selector: 'app-create-tenant',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-tenant.component.html'
})
export class CreateTenantComponent implements OnInit {
  @Output() onCreated = new EventEmitter<void>();
  @Input() editData: any = null;
  
  tenant: any = {
    name: '',
    domain: ''
  };
  loading = false;
  isEditing = false;

  constructor(private tenantService: TenantService) {}

  ngOnInit(): void {
    if (this.editData) {
      this.tenant = { ...this.editData };
      this.isEditing = true;
    }
  }

  onSubmit() {
    this.loading = true;
    const obs = this.isEditing 
      ? this.tenantService.update(this.tenant.id, this.tenant)
      : this.tenantService.create(this.tenant);

    obs.subscribe({
      next: () => {
        toast.success(this.isEditing ? 'Tenant updated successfully' : 'Tenant created successfully');
        this.loading = false;
        this.onCreated.emit();
        this.tenant = { name: '', domain: '' };
      },
      error: () => {
        this.loading = false;
        toast.error('Operation failed');
      }
    });
  }
}
