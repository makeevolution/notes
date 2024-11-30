3# notes

## Notes on shortcuts/lessons learned for different topics
 import { TestBed } from '@angular/core/testing';
import { of, throwError } from 'rxjs';
import { RootCauseComponent } from './root-cause.component';
import { ApiService } from '../services/api.service';
import { MatDialog } from '@angular/material/dialog';
import { NotificationService } from '../services/notification.service';

describe('RootCauseComponent', () => {
  let component: RootCauseComponent;
  let mockApiService: jasmine.SpyObj<ApiService>;
  let mockDialog: jasmine.SpyObj<MatDialog>;
  let mockNotification: jasmine.SpyObj<NotificationService>;

  beforeEach(() => {
    mockApiService = jasmine.createSpyObj('ApiService', ['getRootCauses', 'createRootCause', 'deleteRootCause']);
    mockDialog = jasmine.createSpyObj('MatDialog', ['open']);
    mockNotification = jasmine.createSpyObj('NotificationService', ['show']);

    TestBed.configureTestingModule({
      declarations: [RootCauseComponent],
      providers: [
        { provide: ApiService, useValue: mockApiService },
        { provide: MatDialog, useValue: mockDialog },
        { provide: NotificationService, useValue: mockNotification },
      ],
    });

    const fixture = TestBed.createComponent(RootCauseComponent);
    component = fixture.componentInstance;
  });

  it('should call populateTableWithData on ngOnInit', () => {
    spyOn(component, 'populateTableWithData');
    component.ngOnInit();
    expect(component.loading).toBeTrue();
    expect(component.populateTableWithData).toHaveBeenCalled();
  });

  it('should handle populateTableWithData observable success', () => {
    const mockResponse = { results: [{ root_cause: 'Test Cause' }] };
    mockApiService.getRootCauses.and.returnValue(of(mockResponse));

    component.populateTableWithData();

    expect(component.loading).toBeTrue(); // Loading starts
    expect(mockApiService.getRootCauses).toHaveBeenCalled();
    expect(component.response).toEqual(mockResponse); // Success response
  });

  it('should handle populateTableWithData observable error', () => {
    mockApiService.getRootCauses.and.returnValue(throwError(() => new Error('API Error')));

    component.populateTableWithData();

    expect(component.loading).toBeFalse(); // Loading stops on error
    expect(mockApiService.getRootCauses).toHaveBeenCalled();
  });

  it('should call getRootCausesComplete on observable complete', () => {
    spyOn(component, 'getRootCausesComplete');
    mockApiService.getRootCauses.and.returnValue(of({ results: [] }));

    component.populateTableWithData();

    expect(component.getRootCausesComplete).toHaveBeenCalled();
  });

  it('should populate tableContents in getRootCausesComplete', () => {
    component.response = { results: [{ root_cause: 'Test Cause' }] };

    component.getRootCausesComplete();

    expect(component.tableContents).toEqual([
      { param_id: 0, root_cause: 'Test Cause', action: { delete: true } },
    ]);
    expect(component.loading).toBeFalse(); // Loading ends
  });

  it('should open dialog and call addRootCause on close', () => {
    const dialogRefSpy = jasmine.createSpyObj({ afterClosed: of({ name: 'New Cause' }) });
    mockDialog.open.and.returnValue(dialogRefSpy);
    spyOn(component, 'addRootCause');

    component.openAddRootCauseDialog();

    expect(mockDialog.open).toHaveBeenCalled();
    expect(component.addRootCause).toHaveBeenCalledWith({ name: 'New Cause' });
  });

  it('should not call addRootCause if dialog closes without valid data', () => {
    const dialogRefSpy = jasmine.createSpyObj({ afterClosed: of(null) });
    mockDialog.open.and.returnValue(dialogRefSpy);
    spyOn(component, 'addRootCause');

    component.openAddRootCauseDialog();

    expect(mockDialog.open).toHaveBeenCalled();
    expect(component.addRootCause).not.toHaveBeenCalled();
  });

  it('should handle addRootCause observable success', () => {
    mockApiService.createRootCause.and.returnValue(of({}));
    spyOn(component, 'populateTableWithData');

    component.addRootCause({ name: 'Test Cause' });

    expect(mockApiService.createRootCause).toHaveBeenCalledWith({ root_cause: 'Test Cause' });
    expect(component.populateTableWithData).toHaveBeenCalled();
    expect(mockNotification.show).toHaveBeenCalledWith('Root Cause added successfully', 'Close', 'success');
  });

  it('should handle addRootCause observable error', () => {
    mockApiService.createRootCause.and.returnValue(throwError(() => new Error('API Error')));

    component.addRootCause({ name: 'Test Cause' });

    expect(mockApiService.createRootCause).toHaveBeenCalled();
    expect(mockNotification.show).toHaveBeenCalledWith('Failed to add Root Cause', 'Close', 'fail');
    expect(component.loading).toBeFalse(); // Loading stops on error
  });

  it('should handle deleteRootCause success', () => {
    component.tableContents = [{ param_id: 0, root_cause: 'Test Cause', action: { delete: true } }];
    mockApiService.deleteRootCause.and.returnValue(of({}));
    spyOn(component, 'populateTableWithData');

    component.deleteRootCause({ id: '0' });

    expect(mockApiService.deleteRootCause).toHaveBeenCalledWith('Test Cause');
    expect(component.populateTableWithData).toHaveBeenCalled();
    expect(mockNotification.show).toHaveBeenCalledWith('Root Cause deleted successfully', 'Close', 'success');
  });

  it('should handle deleteRootCause error', () => {
    component.tableContents = [{ param_id: 0, root_cause: 'Test Cause', action: { delete: true } }];
    mockApiService.deleteRootCause.and.returnValue(throwError(() => new Error('API Error')));

    component.deleteRootCause({ id: '0' });

    expect(mockApiService.deleteRootCause).toHaveBeenCalledWith('Test Cause');
    expect(mockNotification.show).toHaveBeenCalledWith('Failed to delete Root Cause', 'Close', 'fail');
    expect(component.loading).toBeFalse(); // Loading stops on error
  });

  it('should not delete if root cause is invalid', () => {
    component.tableContents = [];

    component.deleteRootCause({ id: '1' });

    expect(mockApiService.deleteRootCause).not.toHaveBeenCalled();
    expect(mockNotification.show).toHaveBeenCalledWith('Failed to delete Root Cause', 'Close', 'fail');
  });
});

