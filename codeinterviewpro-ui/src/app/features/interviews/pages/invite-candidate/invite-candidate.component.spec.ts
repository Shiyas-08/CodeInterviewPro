import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InviteCandidateComponent } from './invite-candidate.component';

describe('InviteCandidateComponent', () => {
  let component: InviteCandidateComponent;
  let fixture: ComponentFixture<InviteCandidateComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [InviteCandidateComponent]
    });
    fixture = TestBed.createComponent(InviteCandidateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
