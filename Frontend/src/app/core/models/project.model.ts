
export interface ProjectResponse {
  projectId: number;
  projectCode: string;
  projectName: string;
  clientName: string;
  technology: string | null;
  requiredEmployees: number;
  location: string | null;
  startDate: string | null;
  endDate: string | null;
  status: string;
}

export interface CreateProject {
  projectCode: string;
  projectName: string;
  clientName: string;
  technology?: string | null;
  requiredEmployees: number;
  location?: string | null;
  startDate?: string | null;
  endDate?: string | null;
}

export interface UpdateProject {
  projectName: string;
  clientName: string;
  technology?: string | null;
  requiredEmployees: number;
  location?: string | null;
  startDate?: string | null;
  endDate?: string | null;
  status: 'Active' | 'Inactive' | 'Completed';
}
