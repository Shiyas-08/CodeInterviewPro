import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TenantListComponent } from '../../components/tenant-list/tenant-list.component';

@Component({
  selector: 'app-tenant-management',
  standalone: true,
  imports: [CommonModule, TenantListComponent],
  templateUrl: './tenant-management.component.html',
  styleUrls: ['./tenant-management.component.css']
})
export class TenantManagementComponent {

}
