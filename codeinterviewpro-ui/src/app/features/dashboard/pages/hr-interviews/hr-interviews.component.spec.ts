import { ComponentFixture, TestBed } from '@angular/core/testing';

import { HrInterviewsComponent } from './hr-interviews.component';

describe('HrInterviewsComponent', () => {
  let component: HrInterviewsComponent;
  let fixture: ComponentFixture<HrInterviewsComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [HrInterviewsComponent]
    });
    fixture = TestBed.createComponent(HrInterviewsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
