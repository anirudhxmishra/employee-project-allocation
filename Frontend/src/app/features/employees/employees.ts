
import {
  Component,
  OnInit,
  inject,
  ChangeDetectorRef
} from '@angular/core';

import { EmployeeService } from '../../core/services/employee.service';
import { EmployeeResponse } from '../../core/models/employee-response.model';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [],
  templateUrl: './employees.html',
  styleUrl: './employees.scss'
})
export class Employees implements OnInit {

  private employeeService = inject(EmployeeService);
  private cdr = inject(ChangeDetectorRef);

  employees: EmployeeResponse[] = [];

  searchTerm = '';
  isLoading = true;
  errorMessage = '';

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

}
