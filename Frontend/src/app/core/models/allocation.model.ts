export interface Allocation {
  allocationId: number;
  employeeCode: string;
  employeeName: string;
  projectId: number;
  projectName: string;
  role: string;
  allocationDate: string;
  releaseDate: string | null;
  isActive: boolean;
  allocatedBy: string;
}

export interface CreateAllocation {
  employeeCode: string;
  projectId: number;
  role: string;
  allocatedBy: string;
}

export interface ReleaseAllocation {
  releaseDate: string;
}
