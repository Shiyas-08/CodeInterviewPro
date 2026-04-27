import { Component, OnInit } from '@angular/core';
import { TenantService } from 'src/app/core/services/tenant.service';

@Component({
  selector: 'app-tenant-list',
  templateUrl: './tenant-list.component.html'
})
export class TenantListComponent implements OnInit {
  tenants: any[] = [];
  loading = true;
  showCreate = false;
  selectedTenantForHr: string | null = null;
  editingTenant: any = null;

  constructor(private tenantService: TenantService) {}

  ngOnInit(): void {
    this.fetchTenants();
  }

  fetchTenants() {
    this.loading = true;
    this.editingTenant = null;
    this.tenantService.getAll().subscribe({
      next: (res: any) => {
        this.tenants = res.data || res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  deleteTenant(id: string) {
    if (confirm('Are you sure you want to deactivate this tenant?')) {
      this.tenantService.delete(id).subscribe(() => {
        this.fetchTenants();
      });
    }
  }

  toggleAddHr(tenantId: string) {
    if (this.selectedTenantForHr === tenantId) {
      this.selectedTenantForHr = null;
    } else {
      this.selectedTenantForHr = tenantId;
    }
  }

  editTenant(t: any) {
    this.editingTenant = t;
  }

  cancelEdit() {
    this.editingTenant = null;
  }
}
