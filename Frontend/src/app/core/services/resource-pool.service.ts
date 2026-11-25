
import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import { ResourcePoolEmployee } from '../models/resource-pool.model';

@Injectable({
  providedIn: 'root'
})
export class ResourcePoolService {

  private readonly apiUrl = '/api/ResourcePool';

  constructor(private http: HttpClient) { }

  getAvailableEmployees(
    projectId?: number | null
  ): Observable<ResourcePoolEmployee[]> {

    let params = new HttpParams();

    if (projectId !== null && projectId !== undefined) {
      params = params.set('projectId', projectId);
    }

    return this.http.get<ResourcePoolEmployee[]>(
      this.apiUrl,
      { params }
    );
  }
}
