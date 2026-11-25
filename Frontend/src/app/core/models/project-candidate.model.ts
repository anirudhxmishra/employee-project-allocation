
export type CandidateStatus =
  | 'Selected'
  | 'Rejected'
  | 'InterviewScheduled';

export interface ProjectCandidate {
  candidateId: number;
  employeeCode: string;
  employeeName: string;
  projectId: number;
  projectCode: string;
  projectName: string;
  status: CandidateStatus | string;
  shortlistedOn: string;
  interviewDate: string | null;
  remarks: string | null;
}

export interface CreateProjectCandidate {
  employeeCode: string;
  projectId: number;
  remarks: string;
}

export interface UpdateCandidateStatus {
  status: CandidateStatus;
  role: string;
  interviewDate: string | null;
  remarks: string;
}
