import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-assign-questions',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './assign-questions.component.html'
})
export class AssignQuestionsComponent implements OnInit {

  interviewId = '';
  questions: any[] = [];
  selectedQuestions: any[] = [];

  languageMap: any = {
    0: 'C#',
    1: 'Python',
    2: 'JavaScript',
    3: 'Java',
    4: 'Go'
  };

  loading = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private service: InterviewService
  ) {}

  ngOnInit(): void {

    this.interviewId =
      this.route.snapshot.paramMap.get('id') || '';

    this.service.getQuestions().subscribe((res:any) => {
      this.questions = res;
    });
  }

  toggleQuestion(question: any, event: any) {

    if (event.target.checked) {
      this.selectedQuestions.push({
        questionId: question.id,
        marks: 10
      });
    } else {
      this.selectedQuestions =
        this.selectedQuestions.filter(
          x => x.questionId !== question.id
        );
    }
  }

  submit() {

    if (this.selectedQuestions.length === 0) {
      toast.warning('Please select at least one question');
      return;
    }

    const payload = {
      questions: this.selectedQuestions
    };

    this.loading = true;
    this.service.assignQuestions(
      this.interviewId,
      payload
    ).subscribe({
      next: (res: any) => {
        this.loading = false;
        toast.success(res?.message || 'Questions Assigned');

        this.router.navigate([
          '/dashboard/interviews',
          this.interviewId,
          'schedule'
        ]);
      },
      error: (err) => {
        this.loading = false;
        toast.error('Failed to assign questions');
      }
    });
  }
}