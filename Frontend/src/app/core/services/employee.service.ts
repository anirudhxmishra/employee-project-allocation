
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { EmployeeResponse } from '../models/employee-response.model';
import { CreateEmployee } from '../models/create-employee.model';
import { UpdateEmployee } from '../models/update-employee.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {

  private http = inject(HttpClient);
  private apiUrl = '/api/Employees';

  getEmployees(): Observable<EmployeeResponse[]> {
    return this.http.get<EmployeeResponse[]>(this.apiUrl);
  }

  createEmployee(employee: CreateEmployee): Observable<unknown> {
    return this.http.post(this.apiUrl, employee);
  }

  updateEmployee(
    employeeCode: string,
    employee: UpdateEmployee
  ): Observable<unknown> {
    return this.http.put(
      `${this.apiUrl}/${encodeURIComponent(employeeCode)}`,
      employee
    );
  }

  deleteEmployee(employeeCode: string): Observable<unknown> {
    return this.http.delete(
      `${this.apiUrl}/${encodeURIComponent(employeeCode)}`
    );
  }
}
