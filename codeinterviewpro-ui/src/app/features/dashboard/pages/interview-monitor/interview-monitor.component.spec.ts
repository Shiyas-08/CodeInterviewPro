import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InterviewMonitorComponent } from './interview-monitor.component';

describe('InterviewMonitorComponent', () => {
  let component: InterviewMonitorComponent;
  let fixture: ComponentFixture<InterviewMonitorComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InterviewMonitorComponent]
    });
    fixture = TestBed.createComponent(InterviewMonitorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
