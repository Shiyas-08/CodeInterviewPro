import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from 'src/app/core/services/admin.service';
import { TenantService } from 'src/app/core/services/tenant.service';
import { toast } from 'src/app/core/services/toast';

@Component({
  selector: 'app-create-hr-page',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './create-hr.component.html',
  styleUrls: ['./create-hr.component.css']
})
export class CreateHrPageComponent implements OnInit {

  tenants: any[] = [];
  loadingTenants = true;

  form = {
    fullName: '',
    email: '',
    password: '',
    tenantId: ''
  };
  loading = false;

  constructor(
    private adminService: AdminService,
    private tenantService: TenantService
  ) {}

  ngOnInit(): void {
    this.tenantService.getAll().subscribe({
      next: (res: any) => {
        this.tenants = res.data || res;
        this.loadingTenants = false;
      },
      error: () => { 
        toast.error('Failed to load organizations.');
        this.loadingTenants = false; 
      }
    });
  }

  onSubmit() {
    if (!this.form.tenantId) {
      toast.warning('Please select a tenant organization.');
      return;
    }
    this.loading = true;

    this.adminService.createHr(this.form).subscribe({
      next: () => {
        toast.success(`HR user ${this.form.email} created successfully!`);
        this.loading = false;
        this.form = { fullName: '', email: '', password: '', tenantId: '' };
      },
      error: (err) => {
        toast.error(err?.error?.message || 'Failed to create HR user.');
        this.loading = false;
      }
    });
  }
}
