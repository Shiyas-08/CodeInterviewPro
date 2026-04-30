import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-hr-interviews',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './hr-interviews.component.html',
  styleUrls: ['./hr-interviews.component.css']
})
export class HrInterviewsComponent implements OnInit {
  interviews: any[] = [];
  filteredInterviews: any[] = [];
  pagedInterviews: any[] = [];
  loading = true;
  searchTerm = '';

  currentPage = 1;
  pageSize = 5;
  totalPages = 1;

  constructor(
    private interviewService: InterviewService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.fetchInterviews();
  }

  fetchInterviews() {
    this.loading = true;
    this.interviewService.getAllInterviews().subscribe({
      next: (res: any) => {
        this.interviews = res.data || res;
        this.filterInterviews();
        this.loading = false;
      },
      error: () => {
        console.error('Failed to load interviews');
        this.loading = false;
      }
    });
  }

  filterInterviews() {
    let result = [];
    if (!this.searchTerm.trim()) {
      result = [...this.interviews];
    } else {
      const term = this.searchTerm.toLowerCase();
      result = this.interviews.filter(i => 
        i.title?.toLowerCase().includes(term) || 
        i.description?.toLowerCase().includes(term)
      );
    }
    
    this.totalPages = Math.ceil(result.length / this.pageSize);
    this.currentPage = 1;
    this.filteredInterviews = result;
    this.updatePagedList();
  }

  updatePagedList() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedInterviews = this.filteredInterviews.slice(startIndex, endIndex);
  }

  setPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.updatePagedList();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  stopInterview(token: string) {
    if (!token) return;
    
    if (confirm('Are you sure you want to stop this interview? The candidate will no longer be able to code.')) {
      this.interviewService.stopSession(token).subscribe({
        next: () => {
          toast.success('Interview stopped successfully');
          this.fetchInterviews(); // Refresh list to update statuses
        },
        error: (err) => {
          console.error('Failed to stop interview', err);
          toast.error('Error stopping interview');
        }
      });
    }
  }

  monitorInterview(token: string) {
    this.router.navigate(['/admin/interview-monitor', token]);
  }

  removeInterview(id: string) {
    if (!id) return;
    if (confirm('Are you sure you want to remove this interview? This action cannot be undone.')) {
      this.interviewService.deleteInterview(id).subscribe({
        next: () => {
          toast.success('Interview removed successfully');
          this.fetchInterviews();
        },
        error: (err) => {
          console.error('Failed to remove interview', err);
          toast.error('Error removing interview');
        }
      });
    }
  }

  copyLink(token: string) {
    if (!token) return;
    const link = `${window.location.origin}/auth/register?token=${token}`;
    navigator.clipboard.writeText(link).then(() => {
      toast.success('Invite link copied to clipboard!');
    });
  }

  viewResult(candidateId: string, interviewId: string) {
    if (!candidateId || !interviewId) return;
    this.router.navigate(['/admin/candidate-result', candidateId, interviewId]);
  }
}
