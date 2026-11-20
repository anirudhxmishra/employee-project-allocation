import {
  Component,
  ElementRef,
  OnInit,
  ViewChild,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';

import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeResponse } from '../../core/models/employee-response.model';
import { CreateEmployee } from '../../core/models/create-employee.model';
import { UpdateEmployee } from '../../core/models/update-employee.model';

import {
  ActionModal,
  ModalState
} from '../../shared/components/action-modal/action-modal';

type ModalPurpose = 'delete' | 'form' | null;

type EmployeeFormField =
  | 'employeeCode'
  | 'fullName'
  | 'email'
  | 'location'
  | 'mobileNumber'
  | 'trainingName';

// Field-level validation messages
const FIELD_ERROR_MESSAGES: Record<EmployeeFormField, Record<string, string>> = {
  employeeCode: {
    required: 'Employee code is required.',
    maxlength: 'Employee code cannot exceed 8 characters.'
  },
  fullName: {
    required: 'Full name is required.',
    minlength: 'Full name must be at least 2 characters.',
    maxlength: 'Full name cannot exceed 100 characters.'
  },
  email: {
    required: 'Email address is required.',
    email: 'Enter a valid email address.',
    maxlength: 'Email address cannot exceed 150 characters.'
  },
  location: {
    required: 'Location is required.',
    maxlength: 'Location cannot exceed 100 characters.'
  },
  mobileNumber: {
    required: 'Mobile number is required.',
    pattern: 'Mobile number must be exactly 10 digits.'
  },
  trainingName: {
    required: 'Training name is required.',
    maxlength: 'Training name cannot exceed 150 characters.'
  }
};

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    ActionModal
  ],
  templateUrl: './employees.html',
  styleUrl: './employees.scss'
})
export class Employees implements OnInit {

  private employeeService = inject(EmployeeService);
  private cdr = inject(ChangeDetectorRef);
  private fb = inject(FormBuilder);

  // Scrollable table area (used to jump back to the top on page change)
  @ViewChild('tableWrapper') private tableWrapper?: ElementRef<HTMLDivElement>;

  employees: EmployeeResponse[] = [];

  searchTerm = '';
  isLoading = true;
  errorMessage = '';

  // Pagination
  readonly pageSize = 10;
  currentPage = 1;

  showAddForm = false;
  isSaving = false;
  formError = '';
  successMessage = '';

  isEditMode = false;
  selectedEmployeeCode = '';

  // Reusable modal properties
  modalVisible = false;
  modalState: ModalState = 'confirm';
  modalTitle = '';
  modalMessage = '';
  modalEmployeeName = '';
  modalEmployeeCode = '';
  modalConfirmText = 'Yes, Delete';
  isModalProcessing = false;

  private modalPurpose: ModalPurpose = null;

  private selectedEmployeeForDeletion: EmployeeResponse | null = null;

  selectedEmployeeForView: EmployeeResponse | null = null;

  employeeForm = this.fb.group({
    employeeCode: [
      '',
      [Validators.required, Validators.maxLength(8)]
    ],
    fullName: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(100)
      ]
    ],
    email: [
      '',
      [
        Validators.required,
        Validators.email,
        Validators.maxLength(150)
      ]
    ],
    location: [
      '',
      [
        Validators.required,
        Validators.maxLength(100)
      ]
    ],
    mobileNumber: [
      '',
      [
        Validators.required,
        Validators.pattern(/^[0-9]{10}$/)
      ]
    ],
    trainingName: [
      '',
      [
        Validators.required,
        Validators.maxLength(150)
      ]
    ],
    trainingCompletedOn: ['']
  });

  // Search (name, employee code, email - case-insensitive)
  get filteredEmployees(): EmployeeResponse[] {
    const search = this.searchTerm.toLowerCase().trim();

    if (!search) {
      return this.employees;
    }

    return this.employees.filter(employee =>
      employee.fullName.toLowerCase().includes(search) ||
      employee.employeeCode.toLowerCase().includes(search) ||
      employee.email.toLowerCase().includes(search)
    );
  }

  // Pagination getters
  get totalPages(): number {
    return Math.max(
      1,
      Math.ceil(this.filteredEmployees.length / this.pageSize)
    );
  }

  get paginatedEmployees(): EmployeeResponse[] {
    const startIndex = (this.currentPage - 1) * this.pageSize;

    return this.filteredEmployees.slice(
      startIndex,
      startIndex + this.pageSize
    );
  }

  get startRecord(): number {
    if (this.filteredEmployees.length === 0) {
      return 0;
    }

    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get endRecord(): number {
    return Math.min(
      this.currentPage * this.pageSize,
      this.filteredEmployees.length
    );
  }

  ngOnInit(): void {
    this.loadEmployees();
  }

  // Load employees
  loadEmployees(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.employeeService.getEmployees().subscribe({
      next: (response) => {
        this.employees = response;
        this.isLoading = false;

        // Handles the case where the current page becomes empty (e.g. after delete)
        this.ensureValidCurrentPage();

        this.cdr.markForCheck();
      },
      error: (error) => {
        console.error('Employees API error:', error);

        this.errorMessage = 'Unable to load employees.';
        this.isLoading = false;

        this.cdr.markForCheck();
      }
    });
  }

  // Search changed - always return to page 1
  onSearchChange(value: string): void {
    this.searchTerm = value;
    this.currentPage = 1;
    this.scrollTableToTop();
  }

  // Pagination controls
  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.scrollTableToTop();
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.scrollTableToTop();
    }
  }

  // Reset the internal table scroll position
  private scrollTableToTop(): void {
    this.tableWrapper?.nativeElement.scrollTo({ top: 0 });
  }

  // Keep current page within the valid range
  private ensureValidCurrentPage(): void {
    if (this.currentPage > this.totalPages) {
      this.currentPage = this.totalPages;
    }

    if (this.currentPage < 1) {
      this.currentPage = 1;
    }
  }

  // Format date as "20 November 2026"
  formatTrainingDate(value: string | null | undefined): string {
    if (!value) {
      return 'Not provided';
    }

    // Use only the date part to avoid time zone shifts
    const [year, month, day] = value
      .substring(0, 10)
      .split('-')
      .map(Number);

    const date = new Date(year, month - 1, day);

    if (!year || !month || !day || isNaN(date.getTime())) {
      return 'Not provided';
    }

    return date.toLocaleDateString('en-GB', {
      day: 'numeric',
      month: 'long',
      year: 'numeric'
    });
  }

  // Field-level validation message (empty string when valid)
  getFieldError(field: EmployeeFormField): string {
    const control = this.employeeForm.controls[field];

    if (!control || !control.errors || !(control.touched || control.dirty)) {
      return '';
    }

    const messages = FIELD_ERROR_MESSAGES[field];
    const errorKey = Object.keys(control.errors)[0];

    return messages[errorKey] ?? 'Invalid value.';
  }

  // Open Employee Details
  viewEmployee(employee: EmployeeResponse): void {
    this.selectedEmployeeForView = employee;
  }

  // Close Employee Details
  closeEmployeeDetails(): void {
    this.selectedEmployeeForView = null;
  }

  // Open Add Employee form
  openAddForm(): void {
    this.resetFormMode();

    this.showAddForm = true;
    this.formError = '';
    this.successMessage = '';
  }

  // Open Edit Employee form
  editEmployee(employee: EmployeeResponse): void {
    this.isEditMode = true;
    this.selectedEmployeeCode = employee.employeeCode;

    this.showAddForm = true;
    this.formError = '';
    this.successMessage = '';

    this.employeeForm.patchValue({
      employeeCode: employee.employeeCode,
      fullName: employee.fullName,
      email: employee.email,
      location: employee.location,
      mobileNumber: employee.mobileNumber,
      trainingName: employee.trainingName ?? '',
      trainingCompletedOn: employee.trainingCompletedOn
        ? employee.trainingCompletedOn.substring(0, 10)
        : ''
    });

    this.employeeForm.controls.employeeCode.disable();
  }

  // Close Add/Edit form
  closeAddForm(): void {
    this.showAddForm = false;
    this.formError = '';
    this.successMessage = '';

    this.resetFormMode();
  }

  // Reset form
  resetFormMode(): void {
    this.isEditMode = false;
    this.selectedEmployeeCode = '';

    this.employeeForm.reset();
    this.employeeForm.controls.employeeCode.enable();
  }

  // Submit Add/Edit form
  submitEmployee(): void {

    if (this.employeeForm.invalid) {
      this.employeeForm.markAllAsTouched();

      this.formError =
        'Please fill in all required fields correctly before saving.';

      this.cdr.markForCheck();
      return;
    }

    const formValue = this.employeeForm.getRawValue();

    this.isSaving = true;
    this.formError = '';
    this.successMessage = '';

    if (this.isEditMode) {

      const employee: UpdateEmployee = {
        fullName: formValue.fullName!.trim(),
        email: formValue.email!.trim(),
        location: formValue.location!.trim(),
        mobileNumber: formValue.mobileNumber!,
        trainingName: formValue.trainingName!.trim(),
        trainingCompletedOn: formValue.trainingCompletedOn
          ? new Date(
            `${formValue.trainingCompletedOn}T00:00:00`
          ).toISOString()
          : null
      };

      this.employeeService.updateEmployee(
        this.selectedEmployeeCode,
        employee
      ).subscribe({

        next: () => {
          this.isSaving = false;

          this.showFormResult(
            'success',
            'Employee Updated Successfully',
            `${employee.fullName}'s details have been updated successfully.`
          );

          this.loadEmployees();
          this.cdr.markForCheck();
        },

        error: (error) => {
          console.error('Update employee API error:', error);

          this.isSaving = false;

          this.showFormResult(
            'error',
            'Unable to Update Employee',
            error.error?.message ||
            'Something went wrong while updating the employee. Please try again.'
          );
        }
      });

    } else {

      const employee: CreateEmployee = {
        employeeCode: formValue.employeeCode!.trim(),
        fullName: formValue.fullName!.trim(),
        email: formValue.email!.trim(),
        location: formValue.location!.trim(),
        mobileNumber: formValue.mobileNumber!,
        trainingName: formValue.trainingName!.trim(),
        trainingCompletedOn: formValue.trainingCompletedOn
          ? new Date(
            `${formValue.trainingCompletedOn}T00:00:00`
          ).toISOString()
          : null
      };

      this.employeeService.createEmployee(employee).subscribe({

        next: () => {
          this.isSaving = false;

          this.showFormResult(
            'success',
            'Employee Added Successfully',
            `${employee.fullName} has been added to the employee directory.`
          );

          this.loadEmployees();
          this.cdr.markForCheck();
        },

        error: (error) => {
          console.error('Create employee API error:', error);

          this.isSaving = false;

          this.showFormResult(
            'error',
            'Unable to Add Employee',
            error.error?.message ||
            'Something went wrong while adding the employee. Please try again.'
          );
        }
      });
    }
  }

  // Show Add/Edit success or error popup
  private showFormResult(
    state: 'success' | 'error',
    title: string,
    message: string
  ): void {

    this.modalPurpose = 'form';
    this.modalState = state;
    this.modalTitle = title;
    this.modalMessage = message;

    this.modalEmployeeName = '';
    this.modalEmployeeCode = '';
    this.modalConfirmText = 'Done';

    this.modalVisible = true;
    this.isModalProcessing = false;

    this.cdr.markForCheck();
  }

  // Open Delete confirmation popup
  deleteEmployeeRecord(employee: EmployeeResponse): void {

    this.selectedEmployeeForDeletion = employee;

    this.modalPurpose = 'delete';
    this.modalState = 'confirm';

    this.modalTitle = 'Delete Employee?';

    this.modalMessage =
      'Are you sure you want to delete this employee? This action cannot be undone.';

    this.modalEmployeeName = employee.fullName;
    this.modalEmployeeCode = employee.employeeCode;

    this.modalConfirmText = 'Yes, Delete';

    this.modalVisible = true;
    this.isModalProcessing = false;
  }

  // Confirm Delete action
  confirmModalAction(): void {

    if (!this.selectedEmployeeForDeletion) {
      return;
    }

    const employee = this.selectedEmployeeForDeletion;

    this.isModalProcessing = true;

    this.employeeService.deleteEmployee(
      employee.employeeCode
    ).subscribe({

      next: () => {

        this.isModalProcessing = false;

        this.modalState = 'success';
        this.modalTitle = 'Employee Deleted Successfully';

        this.modalMessage =
          `${employee.fullName} has been removed from the employee directory.`;

        this.selectedEmployeeForDeletion = null;

        // Reloads the list and moves back a page if the current page is now empty
        this.loadEmployees();
        this.cdr.markForCheck();
      },

      error: (error) => {

        console.error('Delete employee API error:', error);

        this.isModalProcessing = false;

        this.modalState = 'error';
        this.modalTitle = 'Action Cannot Be Performed';

        this.modalMessage =
          error.error?.message ||
          'Unable to delete this employee. Please try again.';

        this.selectedEmployeeForDeletion = null;

        this.cdr.markForCheck();
      }
    });
  }

  // Close reusable modal
  closeActionModal(): void {

    if (this.isModalProcessing) {
      return;
    }

    const purpose = this.modalPurpose;
    const state = this.modalState;

    this.modalVisible = false;
    this.isModalProcessing = false;

    this.selectedEmployeeForDeletion = null;
    this.modalPurpose = null;

    // Close form only after successful Add/Edit
    if (purpose === 'form' && state === 'success') {
      this.closeAddForm();
    }

    this.cdr.markForCheck();
  }
}
