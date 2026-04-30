import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { CreateHrComponent } from '../../components/create-hr/create-hr.component';
import { TenantService } from 'src/app/core/services/tenant.service';
import { AdminService } from 'src/app/core/services/admin.service';
import { toast } from 'src/app/core/services/toast';

@Component({
  selector: 'app-tenant-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, CreateHrComponent],
  templateUrl: './tenant-detail.component.html'
})
export class TenantDetailComponent implements OnInit {
  tenantId: string = '';
  tenant: any;
  hrUsers: any[] = [];
  loading = true;
  showAddHr = false;

  constructor(
    private route: ActivatedRoute,
    private tenantService: TenantService,
    private adminService: AdminService
  ) {}

  ngOnInit(): void {
    this.tenantId = this.route.snapshot.paramMap.get('id') || '';
    if (this.tenantId) {
      this.fetchTenantDetails();
    }
  }

  fetchTenantDetails() {
    this.loading = true;
    this.tenantService.getById(this.tenantId).subscribe({
      next: (res: any) => {
        this.tenant = res.data || res;
        // In a real app, this might include HR users or we fetch them separately
        // For now, I'll assume they come with the tenant or we fetch them if API exists
        this.hrUsers = this.tenant.hrUsers || [];
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        toast.error('Failed to load tenant details');
      }
    });
  }

  onHrCreated() {
    this.showAddHr = false;
    this.fetchTenantDetails();
    toast.success('HR user added successfully');
  }
}
