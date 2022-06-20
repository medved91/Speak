import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinSetupComponent } from './join-setup.component';

describe('JoinSetupComponent', () => {
  let component: JoinSetupComponent;
  let fixture: ComponentFixture<JoinSetupComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ JoinSetupComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(JoinSetupComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
