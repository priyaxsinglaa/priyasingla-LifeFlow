import { TestBed } from '@angular/core/testing';

import { dashBoardService } from './dash-board';

describe('dashBoardService', () => {
  let service: dashBoardService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(dashBoardService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
