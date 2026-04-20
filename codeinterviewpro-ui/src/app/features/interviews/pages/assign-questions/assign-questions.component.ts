import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InterviewService } from 'src/app/core/services/interview.service';

@Component({
  selector: 'app-assign-questions',
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
      alert('Please select at least one question');
      return;
    }

    const payload = {
      questions: this.selectedQuestions
    };

    this.service.assignQuestions(
      this.interviewId,
      payload
    ).subscribe((res:any) => {

      alert(res?.message || 'Questions Assigned');

      this.router.navigate([
        '/dashboard/interviews',
        this.interviewId,
        'schedule'
      ]);
    });
  }
}