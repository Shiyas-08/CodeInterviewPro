import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { ResultService } from 'src/app/core/services/result.service';
import { toast } from 'src/app/core/services/toast';
import * as html2pdf from 'html2pdf.js';
import { marked } from 'marked';

@Component({
  selector: 'app-candidate-result',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './candidate-result.component.html',
  styleUrls: ['./candidate-result.component.css']
})
export class CandidateResultComponent implements OnInit {
  result: any = null;
  loading = true;
  isDownloading = false;

  constructor(
    private resultService: ResultService,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.route.paramMap.subscribe(params => {
      const candidateId = params.get('candidateId');
      const interviewId = params.get('interviewId');

      if (candidateId && interviewId) {
        // HR View
        this.fetchCandidateResult(candidateId, interviewId);
      } else if (interviewId) {
        // Candidate View
        this.fetchResult(interviewId);
      }
    });
  }

  fetchResult(interviewId: string) {
    this.loading = true;
    this.resultService.getMyResult(interviewId).subscribe({
      next: (res: any) => {
        this.result = res.data || res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching result:', err);
        toast.error('Failed to load interview results.');
        this.loading = false;
      }
    });
  }

  fetchCandidateResult(candidateId: string, interviewId: string) {
    this.loading = true;
    this.resultService.getCandidateResult(candidateId, interviewId).subscribe({
      next: (res: any) => {
        this.result = res.data || res;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error fetching candidate result:', err);
        toast.error('Failed to load candidate results.');
        this.loading = false;
      }
    });
  }

  renderMarkdown(text: string): string {
    if (!text) return '';
    return marked.parse(text) as string;
  }

  downloadPDF() {
    if (this.isDownloading) return;
    
    this.isDownloading = true;
    const wrapper = document.getElementById('pdf-report-template');
    if (!wrapper) {
      toast.error('PDF template not ready.');
      this.isDownloading = false;
      return;
    }

    const toastId = toast.info('Preparing high-quality PDF report...', { duration: 10000 });

    // Show the template as a top-layer overlay temporarily
    wrapper.classList.remove('hidden');
    
    // Target the inner content for capture
    const content = wrapper.firstElementChild as HTMLElement;

    const html2pdfCall = (html2pdf as any).default || html2pdf;

    const opt = {
      margin: 10,
      filename: `Interview_Report_${this.result.candidateEmail || 'Candidate'}.pdf`,
      image: { type: 'jpeg', quality: 0.98 },
      html2canvas: { 
        scale: 2, 
        useCORS: true, 
        logging: true,
        letterRendering: true,
        backgroundColor: '#ffffff'
      },
      jsPDF: { unit: 'mm', format: 'a4', orientation: 'portrait' },
      pagebreak: { mode: ['avoid-all', 'css', 'legacy'] }
    };

    // Give the browser 1 second to fully render the template
    setTimeout(() => {
      html2pdfCall().from(content).set(opt).save().then(() => {
        wrapper.classList.add('hidden');
        this.isDownloading = false;
        toast.success('Report downloaded successfully!');
      }).catch((err: any) => {
        wrapper.classList.add('hidden');
        console.error('PDF Error:', err);
        toast.error('Failed to generate PDF.');
        this.isDownloading = false;
      });
    }, 1000);
  }
}
