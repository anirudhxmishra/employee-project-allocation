
import {
  Component,
  OnInit,
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

  employees: EmployeeResponse[] = [];

  searchTerm = '';
  isLoading = true;
  errorMessage = '';

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
