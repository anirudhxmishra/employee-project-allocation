
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  SkillResponse,
  CreateSkill
} from '../models/skill.model';

@Injectable({
  providedIn: 'root'
})
export class SkillService {

  private http = inject(HttpClient);
  private apiUrl = '/api/Skills';

  // Get all skills
  getSkills(): Observable<SkillResponse[]> {
    return this.http.get<SkillResponse[]>(this.apiUrl);
  }

  // Create a new skill
  createSkill(skill: CreateSkill): Observable<unknown> {
    return this.http.post<unknown>(this.apiUrl, skill);
  }

  // Get skills assigned to an employee
  getEmployeeSkills(employeeCode: string): Observable<SkillResponse[]> {
    return this.http.get<SkillResponse[]>(
      `${this.apiUrl}/employee/${encodeURIComponent(employeeCode)}`
    );
  }


  // Assign skills to an employee
  assignSkills(
    employeeCode: string,
    skillIds: number[]
  ): Observable<unknown> {
    return this.http.post<unknown>(
      `${this.apiUrl}/assign/${encodeURIComponent(employeeCode)}`,
      { skillIds }
    );
  }

}
