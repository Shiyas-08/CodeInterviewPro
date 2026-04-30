import { toast } from 'src/app/core/services/toast';
import { Component, OnInit } from '@angular/core';
import { QuestionService } from 'src/app/core/services/question.service';

@Component({
  selector: 'app-question-bank',
  templateUrl: './question-bank.component.html'
})
export class QuestionBankComponent implements OnInit {
  questions: any[] = [];
  filteredQuestions: any[] = [];
  pagedQuestions: any[] = [];
  loading = true;
  showCreateForm = false;
  isSaving = false;
  isEditing = false;
  searchTerm = '';
  
  currentPage = 1;
  pageSize = 6;
  totalPages = 1;
  
  newQuestion: any = {
    title: '',
    description: '',
    starterCode: '',
    testCases: '',
    language: 0,
    methodName: ''
  };

  languageMap: any = {
    0: 'C#',
    1: 'Python',
    2: 'JavaScript',
    3: 'Java',
    4: 'Go'
  };

  constructor(private questionService: QuestionService) {}

  ngOnInit(): void {
    this.fetchQuestions();
  }

  fetchQuestions() {
    this.loading = true;
    this.questionService.getAll().subscribe({
      next: (res) => {
        this.questions = res;
        this.filterQuestions();
        this.loading = false;
      },
      error: () => {
        this.loading = false;
      }
    });
  }

  filterQuestions() {
    let result = [];
    if (!this.searchTerm.trim()) {
      result = [...this.questions];
    } else {
      const term = this.searchTerm.toLowerCase();
      result = this.questions.filter(q => 
        q.title?.toLowerCase().includes(term) || 
        q.description?.toLowerCase().includes(term) ||
        this.languageMap[q.language]?.toLowerCase().includes(term)
      );
    }

    this.totalPages = Math.ceil(result.length / this.pageSize);
    this.currentPage = 1;
    this.filteredQuestions = result;
    this.updatePagedList();
  }

  updatePagedList() {
    const startIndex = (this.currentPage - 1) * this.pageSize;
    const endIndex = startIndex + this.pageSize;
    this.pagedQuestions = this.filteredQuestions.slice(startIndex, endIndex);
  }

  setPage(page: number) {
    if (page < 1 || page > this.totalPages) return;
    this.currentPage = page;
    this.updatePagedList();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  toggleForm() {
    this.showCreateForm = !this.showCreateForm;
    if (!this.showCreateForm) {
      this.resetForm();
    }
  }

  editQuestion(q: any) {
    this.isEditing = true;
    this.showCreateForm = true;
    this.newQuestion = { ...q };
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  saveQuestion() {
    if (!this.newQuestion.title || !this.newQuestion.description) {
      toast.warning('Title and description are required.');
      return;
    }

    this.isSaving = true;
    const obs = this.isEditing 
      ? this.questionService.update(this.newQuestion)
      : this.questionService.create(this.newQuestion);

    obs.subscribe({
      next: () => {
        this.isSaving = false;
        toast.success(this.isEditing ? 'Question updated' : 'Question added to bank');
        this.showCreateForm = false;
        this.fetchQuestions();
        this.resetForm();
      },
      error: (err) => {
        this.isSaving = false;
        toast.error(err?.error?.message || 'Failed to save question');
      }
    });
  }

  deleteQuestion(id: string) {
    if (confirm('Are you sure you want to delete this question?')) {
      this.questionService.delete(id).subscribe(() => this.fetchQuestions());
    }
  }

  resetForm() {
    this.isEditing = false;
    this.newQuestion = {
      title: '',
      description: '',
      starterCode: '',
      testCases: '',
      language: 0,
      methodName: ''
    };
  }
}
