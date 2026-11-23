
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ProjectResponse,
  CreateProject,
  UpdateProject
} from '../models/project.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectService {

  private readonly apiUrl = '/api/Projects';

  constructor(private http: HttpClient) { }

  // GET all projects
  getProjects(): Observable<ProjectResponse[]> {
    return this.http.get<ProjectResponse[]>(this.apiUrl);
  }

  // POST create project
  createProject(project: CreateProject): Observable<unknown> {
    return this.http.post(this.apiUrl, project);
  }

  // PUT update project
  updateProject(
    projectId: number,
    project: UpdateProject
  ): Observable<unknown> {
    return this.http.put(`${this.apiUrl}/${projectId}`, project);
  }

  // GET project by ID
  getProjectById(projectId: number): Observable<ProjectResponse> {
    return this.http.get<ProjectResponse>(
      `${this.apiUrl}/${projectId}`
    );
  }

  // DELETE project
  deleteProject(projectId: number): Observable<unknown> {
    return this.http.delete(`${this.apiUrl}/${projectId}`);
  }
}
