/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { MemebrCardComponent } from './member-card.component';

describe('MemebrCardComponent', () => {
  let component: MemebrCardComponent;
  let fixture: ComponentFixture<MemebrCardComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ MemebrCardComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(MemebrCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
