import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CandidateResultComponent } from './candidate-result.component';

describe('CandidateResultComponent', () => {
  let component: CandidateResultComponent;
  let fixture: ComponentFixture<CandidateResultComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [CandidateResultComponent]
    });
    fixture = TestBed.createComponent(CandidateResultComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
