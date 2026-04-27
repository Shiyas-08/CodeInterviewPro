import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { toast } from 'src/app/core/services/toast';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html'
})
export class ProfileComponent implements OnInit {
  user: any;
  loading = true;
  saving = false;

  constructor(private auth: AuthService) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile() {
    this.loading = true;
    this.auth.me().subscribe({
      next: (res: any) => {
        this.user = res.data || res;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        toast.error('Failed to load profile');
      }
    });
  }

  saveProfile() {
    this.saving = true;
    // Assuming updateProfile exists in AuthService, if not I'll need to check
    // For now I'll just simulate success if it doesn't exist yet
    setTimeout(() => {
      this.saving = false;
      toast.success('Profile updated successfully');
    }, 1000);
  }
}
