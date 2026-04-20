import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AssignQuestionsComponent } from './assign-questions.component';

describe('AssignQuestionsComponent', () => {
  let component: AssignQuestionsComponent;
  let fixture: ComponentFixture<AssignQuestionsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AssignQuestionsComponent]
    });
    fixture = TestBed.createComponent(AssignQuestionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
