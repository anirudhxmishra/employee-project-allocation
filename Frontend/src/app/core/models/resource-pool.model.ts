
export interface ResourcePoolEmployee {
  employeeCode: string;
  fullName: string;
  email: string;
  location: string | null;
  mobileNumber: string | null;
  trainingName: string | null;
  trainingCompletedOn: string | null;
  skills: string[];
  shortlistedProjects: ResourcePoolShortlistedProject[];
}

export interface ResourcePoolShortlistedProject {
  projectId: number;
  projectName: string;
  status: string;
}
