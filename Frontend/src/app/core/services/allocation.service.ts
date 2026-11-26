import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Allocation,
  CreateAllocation,
  ReleaseAllocation
} from '../models/allocation.model';

@Injectable({
  providedIn: 'root'
})
export class AllocationService {

  private readonly apiUrl = '/api/Allocations';

  constructor(private http: HttpClient) { }

  getAllAllocations(): Observable<Allocation[]> {
    return this.http.get<Allocation[]>(this.apiUrl);
  }

  getAllocationById(id: number): Observable<Allocation> {
    return this.http.get<Allocation>(`${this.apiUrl}/${id}`);
  }

  getEmployeeAllocations(
    employeeCode: string
  ): Observable<Allocation[]> {
    return this.http.get<Allocation[]>(
      `${this.apiUrl}/employee/${employeeCode}`
    );
  }

  createAllocation(
    allocation: CreateAllocation
  ): Observable<Allocation> {
    return this.http.post<Allocation>(this.apiUrl, allocation);
  }

  releaseAllocation(
    id: number,
    data: ReleaseAllocation
  ): Observable<Allocation> {
    return this.http.put<Allocation>(
      `${this.apiUrl}/release/${id}`,
      data
    );
  }
}
