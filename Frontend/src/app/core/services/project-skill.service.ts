
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ProjectSkillResponse,
  AssignProjectSkills
} from '../models/project-skill.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectSkillService {
  private readonly apiUrl = '/api/ProjectSkills';

  constructor(private http: HttpClient) { }

  getProjectSkills(projectId: number): Observable<ProjectSkillResponse[]> {
    return this.http.get<ProjectSkillResponse[]>(
      `${this.apiUrl}/${projectId}`
    );
  }

  assignProjectSkills(
    projectId: number,
    skillIds: number[]
  ): Observable<ProjectSkillResponse[]> {
    const request: AssignProjectSkills = { skillIds };

    return this.http.post<ProjectSkillResponse[]>(
      `${this.apiUrl}/${projectId}`,
      request
    );
  }
}
