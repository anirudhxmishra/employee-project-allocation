
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  ProjectCandidate,
  CreateProjectCandidate,
  UpdateCandidateStatus
} from '../models/project-candidate.model';

@Injectable({
  providedIn: 'root'
})
export class ProjectCandidateService {
  private readonly apiUrl = '/api/ProjectCandidates';

  constructor(private http: HttpClient) { }

  getCandidatesByProject(
    projectId: number
  ): Observable<ProjectCandidate[]> {
    return this.http.get<ProjectCandidate[]>(
      `${this.apiUrl}/project/${projectId}`
    );
  }

  addCandidate(
    candidate: CreateProjectCandidate
  ): Observable<ProjectCandidate> {
    return this.http.post<ProjectCandidate>(
      this.apiUrl,
      candidate
    );
  }

  updateCandidateStatus(
    candidateId: number,
    data: UpdateCandidateStatus
  ): Observable<ProjectCandidate> {
    return this.http.put<ProjectCandidate>(
      `${this.apiUrl}/${candidateId}/status`,
      data
    );
  }
}
